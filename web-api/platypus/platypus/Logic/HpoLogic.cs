using Nssol.Platypus.DataAccess.Core;
using Nssol.Platypus.DataAccess.Repositories.Interfaces.TenantRepositories;
using Nssol.Platypus.Logic.Interfaces;
using Nssol.Platypus.Models.TenantModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nssol.Platypus.Logic
{
    /// <summary>
    /// HPO（ハイパーパラメータ最適化）ロジック
    /// </summary>
    public class HpoLogic : PlatypusLogicBase, IHpoLogic
    {
        private readonly IHpoJobRepository hpoJobRepository;
        private readonly IHpoTrialRepository hpoTrialRepository;
        private readonly IUnitOfWork unitOfWork;
        private static readonly ThreadLocal<Random> random =
            new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

        public HpoLogic(
            IHpoJobRepository hpoJobRepository,
            IHpoTrialRepository hpoTrialRepository,
            IUnitOfWork unitOfWork,
            ICommonDiLogic commonDiLogic) : base(commonDiLogic)
        {
            this.hpoJobRepository = hpoJobRepository;
            this.hpoTrialRepository = hpoTrialRepository;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 探索空間から次のトライアルのパラメータセットを生成する
        /// </summary>
        public Dictionary<string, string> GenerateNextParameters(HpoJob hpoJob, IEnumerable<HpoTrial> completedTrials, int trialNo)
        {
            switch (hpoJob.Algorithm.ToLower())
            {
                case "grid":
                    return GenerateGridParameters(hpoJob.SearchSpaceParsed, trialNo);
                case "random":
                    return GenerateRandomParameters(hpoJob.SearchSpaceParsed);
                case "bayes":
                    return GenerateBayesianParameters(hpoJob.SearchSpaceParsed, completedTrials, hpoJob.OptimizationDirection);
                default:
                    return GenerateRandomParameters(hpoJob.SearchSpaceParsed);
            }
        }

        /// <summary>
        /// HPOジョブを停止する
        /// </summary>
        public async Task StopHpoJobAsync(HpoJob hpoJob)
        {
            await hpoJobRepository.UpdateStatusAsync(hpoJob.Id, "Cancelled");
            hpoJob.CompletedAt = DateTime.Now;

            // 未完了のトライアルをキャンセルする
            var pendingTrials = hpoTrialRepository.GetByHpoJobId(hpoJob.Id)
                .Where(t => t.Status == "Pending" || t.Status == "Running");

            foreach (var trial in pendingTrials)
            {
                await hpoTrialRepository.UpdateTrialResultAsync(trial.Id, "Cancelled", null);
            }

            unitOfWork.Commit();

            // 並行リクエストで生成された可能性のある残存Pendingトライアルを再チェック
            var remainingPending = hpoTrialRepository.GetByHpoJobId(hpoJob.Id)
                .Where(t => t.Status == "Pending" || t.Status == "Running")
                .ToList();

            if (remainingPending.Any())
            {
                foreach (var trial in remainingPending)
                {
                    await hpoTrialRepository.UpdateTrialResultAsync(trial.Id, "Cancelled", null);
                }
                unitOfWork.Commit();
            }
        }

        /// <summary>
        /// グリッドサーチ：パラメータ空間のグリッド上のポイントを順番に返す
        /// </summary>
        private Dictionary<string, string> GenerateGridParameters(List<HpoSearchSpaceParameter> searchSpace, int trialNo)
        {
            var parameters = new Dictionary<string, string>();
            var gridSizes = searchSpace.Select(p => GetGridSize(p)).ToList();
            long totalCombinationsLong = gridSizes.Aggregate(1L, (acc, val) => acc * val);
            int totalCombinations = totalCombinationsLong > int.MaxValue ? int.MaxValue : (int)totalCombinationsLong;
            int index = trialNo % totalCombinations;

            for (int i = 0; i < searchSpace.Count; i++)
            {
                var param = searchSpace[i];
                int gridSize = gridSizes[i];
                int paramIndex = index % gridSize;
                index /= gridSize;

                parameters[param.Name] = GetGridValue(param, paramIndex, gridSize);
            }

            return parameters;
        }

        /// <summary>
        /// ランダムサーチ：パラメータ空間からランダムにサンプリングする
        /// </summary>
        private Dictionary<string, string> GenerateRandomParameters(List<HpoSearchSpaceParameter> searchSpace)
        {
            var parameters = new Dictionary<string, string>();

            foreach (var param in searchSpace)
            {
                switch (param.Type.ToLower())
                {
                    case "int":
                        int intMin = (int)(param.Min ?? 0);
                        int intMax = (int)(param.Max ?? 100);
                        int exclusiveMax = intMax == int.MaxValue ? int.MaxValue : intMax + 1;
                        parameters[param.Name] = random.Value.Next(intMin, exclusiveMax).ToString();
                        break;
                    case "float":
                        double floatMin = param.Min ?? 0.0;
                        double floatMax = param.Max ?? 1.0;
                        double value = floatMin + random.Value.NextDouble() * (floatMax - floatMin);
                        parameters[param.Name] = value.ToString("G6");
                        break;
                    case "categorical":
                        if (param.Values != null && param.Values.Count > 0)
                        {
                            parameters[param.Name] = param.Values[random.Value.Next(param.Values.Count)];
                        }
                        break;
                }
            }

            return parameters;
        }

        /// <summary>
        /// ベイズ最適化：完了済みトライアルの結果に基づいてパラメータを推薦する
        /// （簡易実装：EI（Expected Improvement）に基づくガウシアンプロセスの近似）
        /// </summary>
        private Dictionary<string, string> GenerateBayesianParameters(
            List<HpoSearchSpaceParameter> searchSpace,
            IEnumerable<HpoTrial> completedTrials,
            string direction)
        {
            var completedList = completedTrials?.Where(t => t.MetricValue.HasValue).ToList();

            // 完了済みトライアルが少ない場合はランダムサーチにフォールバック
            if (completedList == null || completedList.Count < 3)
            {
                return GenerateRandomParameters(searchSpace);
            }

            // 簡易ベイズ最適化：ベストパラメータの周辺をExploit + ランダムにExplore
            var bestTrial = direction == "minimize"
                ? completedList.OrderBy(t => t.MetricValue).First()
                : completedList.OrderByDescending(t => t.MetricValue).First();

            var bestParams = bestTrial.ParametersDic;
            var parameters = new Dictionary<string, string>();

            // Exploitation (70%) vs Exploration (30%)
            bool exploit = random.Value.NextDouble() < 0.7;

            foreach (var param in searchSpace)
            {
                if (exploit && bestParams.ContainsKey(param.Name))
                {
                    parameters[param.Name] = PerturbParameter(param, bestParams[param.Name]);
                }
                else
                {
                    // Explore: ランダムサンプリング
                    switch (param.Type.ToLower())
                    {
                        case "int":
                            int intMin = (int)(param.Min ?? 0);
                            int intMax = (int)(param.Max ?? 100);
                            int exclusiveMax = intMax == int.MaxValue ? int.MaxValue : intMax + 1;
                            parameters[param.Name] = random.Value.Next(intMin, exclusiveMax).ToString();
                            break;
                        case "float":
                            double floatMin = param.Min ?? 0.0;
                            double floatMax = param.Max ?? 1.0;
                            double value = floatMin + random.Value.NextDouble() * (floatMax - floatMin);
                            parameters[param.Name] = value.ToString("G6");
                            break;
                        case "categorical":
                            if (param.Values != null && param.Values.Count > 0)
                            {
                                parameters[param.Name] = param.Values[random.Value.Next(param.Values.Count)];
                            }
                            break;
                    }
                }
            }

            return parameters;
        }

        /// <summary>
        /// ベストパラメータの近傍値を生成する（Exploitation）
        /// </summary>
        private string PerturbParameter(HpoSearchSpaceParameter param, string bestValue)
        {
            switch (param.Type.ToLower())
            {
                case "int":
                    if (int.TryParse(bestValue, out int intVal))
                    {
                        int intMin = (int)(param.Min ?? 0);
                        int intMax = (int)(param.Max ?? 100);
                        int range = Math.Max(1, (intMax - intMin) / 5); // 探索範囲の20%、最低1
                        int perturbedInt = intVal + random.Value.Next(-range, range + 1);
                        return Math.Max(intMin, Math.Min(intMax, perturbedInt)).ToString();
                    }
                    return bestValue;
                case "float":
                    if (double.TryParse(bestValue, out double floatVal))
                    {
                        double floatMin = param.Min ?? 0.0;
                        double floatMax = param.Max ?? 1.0;
                        double floatRange = (floatMax - floatMin) * 0.2; // 探索範囲の20%
                        double perturbedFloat = floatVal + (random.Value.NextDouble() * 2 - 1) * floatRange;
                        return Math.Max(floatMin, Math.Min(floatMax, perturbedFloat)).ToString("G6");
                    }
                    return bestValue;
                case "categorical":
                    // カテゴリカルの場合はランダムに選択
                    if (param.Values != null && param.Values.Count > 0)
                    {
                        return param.Values[random.Value.Next(param.Values.Count)];
                    }
                    return bestValue;
                default:
                    return bestValue;
            }
        }

        /// <summary>
        /// グリッドサイズを計算する
        /// </summary>
        private int GetGridSize(HpoSearchSpaceParameter param)
        {
            switch (param.Type.ToLower())
            {
                case "int":
                    int intMin = (int)(param.Min ?? 0);
                    int intMax = (int)(param.Max ?? 10);
                    int intStep = (int)(param.Step ?? 1);
                    if (intStep <= 0) intStep = 1;
                    return ((intMax - intMin) / intStep) + 1;
                case "float":
                    double floatMin = param.Min ?? 0.0;
                    double floatMax = param.Max ?? 1.0;
                    double floatStep = param.Step ?? 0.1;
                    if (floatStep <= 0) floatStep = 0.1;
                    return (int)((floatMax - floatMin) / floatStep) + 1;
                case "categorical":
                    return param.Values?.Count ?? 1;
                default:
                    return 1;
            }
        }

        /// <summary>
        /// グリッド上の特定インデックスの値を取得する
        /// </summary>
        private string GetGridValue(HpoSearchSpaceParameter param, int index, int gridSize)
        {
            switch (param.Type.ToLower())
            {
                case "int":
                    int intMin = (int)(param.Min ?? 0);
                    int intStep = (int)(param.Step ?? 1);
                    return (intMin + index * intStep).ToString();
                case "float":
                    double floatMin = param.Min ?? 0.0;
                    double floatStep = param.Step ?? 0.1;
                    return (floatMin + index * floatStep).ToString("G6");
                case "categorical":
                    if (param.Values != null && index < param.Values.Count)
                    {
                        return param.Values[index];
                    }
                    return "";
                default:
                    return "";
            }
        }
    }
}
