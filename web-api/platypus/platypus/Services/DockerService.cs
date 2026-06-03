using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Options;
using Nssol.Platypus.Infrastructure;
using Nssol.Platypus.Infrastructure.Infos;
using Nssol.Platypus.Infrastructure.Options;
using Nssol.Platypus.Infrastructure.Types;
using Nssol.Platypus.Logic.Interfaces;
using Nssol.Platypus.Models;
using Nssol.Platypus.ServiceModels.ClusterManagementModels;
using Nssol.Platypus.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KqiContainerStatus = Nssol.Platypus.Infrastructure.ContainerStatus;

namespace Nssol.Platypus.Services
{
    /// <summary>
    /// ローカルDockerデーモンとの通信を行うクラスタ管理サービス実装。
    /// シングルテナント・ローカル実行前提。
    /// </summary>
    public class DockerService : PlatypusServiceBase, IClusterManagementService
    {
        private readonly ContainerManageOptions containerOptions;
        private readonly DockerClient dockerClient;

        private const string KqiManagedLabel = "kqi.managed";
        private const string KqiTenantLabel = "kqi.tenant";
        private const string KqiContainerNameLabel = "kqi.name";

        public DockerService(
            ICommonDiLogic commonDiLogic,
            IOptions<ContainerManageOptions> containerOptions) : base(commonDiLogic)
        {
            this.containerOptions = containerOptions.Value;
            var endpoint = string.IsNullOrEmpty(this.containerOptions.DockerEndpoint)
                ? "unix:///var/run/docker.sock"
                : this.containerOptions.DockerEndpoint;
            dockerClient = new DockerClientConfiguration(new Uri(endpoint)).CreateClient();
        }

        #region コンテナ管理

        /// <summary>
        /// 新規にコンテナを作成・起動する。
        /// </summary>
        public async Task<Result<RunContainerOutputModel, string>> RunContainerAsync(RunContainerInputModel inModel)
        {
            try
            {
                // 環境変数の構築
                var envList = new List<string>();
                if (inModel.PrepareAndFinishContainerEnvList != null)
                {
                    foreach (var kv in inModel.PrepareAndFinishContainerEnvList)
                    {
                        envList.Add($"{kv.Key}={kv.Value}");
                    }
                }
                if (inModel.MainContainerEnvList != null)
                {
                    foreach (var kv in inModel.MainContainerEnvList)
                    {
                        envList.Add($"{kv.Key}={kv.Value}");
                    }
                }

                // Bind mounts の構築
                var binds = new List<string>();
                if (inModel.NfsVolumeMounts != null)
                {
                    foreach (var nfs in inModel.NfsVolumeMounts)
                    {
                        string localPath = ConvertNfsToLocalPath(nfs);
                        string readOnlyFlag = nfs.ReadOnly ? ":ro" : "";
                        string mountPath = string.IsNullOrEmpty(nfs.SubPath)
                            ? nfs.MountPath
                            : nfs.MountPath;
                        binds.Add($"{localPath}:{mountPath}{readOnlyFlag}");
                    }
                }

                // 共有ディレクトリのバインドマウント
                if (inModel.ContainerSharedPath != null)
                {
                    foreach (var kv in inModel.ContainerSharedPath)
                    {
                        string sharedDir = Path.Combine(containerOptions.LocalStorageBasePath, "shared", inModel.Name, kv.Key);
                        binds.Add($"{sharedDir}:{kv.Value}");
                    }
                }

                // ポートマッピングの構築
                var portBindings = new Dictionary<string, IList<PortBinding>>();
                var exposedPorts = new Dictionary<string, EmptyStruct>();
                if (inModel.PortMappings != null)
                {
                    foreach (var pm in inModel.PortMappings)
                    {
                        string containerPort = $"{pm.TargetPort}/tcp";
                        exposedPorts[containerPort] = default;
                        portBindings[containerPort] = new List<PortBinding>
                        {
                            new PortBinding
                            {
                                HostPort = pm.NodePort > 0 ? pm.NodePort.ToString() : pm.Port.ToString()
                            }
                        };
                    }
                }

                // GPU設定
                var deviceRequests = new List<DeviceRequest>();
                if (inModel.Gpu > 0)
                {
                    deviceRequests.Add(new DeviceRequest
                    {
                        Driver = "nvidia",
                        Count = inModel.Gpu,
                        Capabilities = new List<IList<string>> { new List<string> { "gpu" } }
                    });
                }

                // ラベル
                var labels = new Dictionary<string, string>
                {
                    { KqiManagedLabel, "true" },
                    { KqiTenantLabel, inModel.TenantName ?? "" },
                    { KqiContainerNameLabel, inModel.Name ?? "" }
                };

                // レジストリ認証
                AuthConfig authConfig = null;
                if (!string.IsNullOrEmpty(inModel.RegistryTokenName))
                {
                    authConfig = new AuthConfig
                    {
                        Username = inModel.RegistryTokenName,
                        Password = inModel.RegistryTokenName
                    };
                }

                // コンテナのEntryPoint/Cmd構築
                IList<string> cmd = null;
                if (!string.IsNullOrEmpty(inModel.EntryPoint))
                {
                    cmd = new List<string> { "/bin/bash", "-c", inModel.EntryPoint };
                }
                else if (!string.IsNullOrEmpty(inModel.Cmd))
                {
                    cmd = new List<string> { "/bin/bash", "-c", inModel.Cmd };
                }

                var createParams = new CreateContainerParameters
                {
                    Image = inModel.ContainerImage,
                    Name = inModel.Name,
                    Env = envList,
                    Labels = labels,
                    ExposedPorts = exposedPorts.Count > 0 ? exposedPorts : null,
                    Cmd = cmd,
                    HostConfig = new HostConfig
                    {
                        Binds = binds.Count > 0 ? binds : null,
                        PortBindings = portBindings.Count > 0 ? portBindings : null,
                        DeviceRequests = deviceRequests.Count > 0 ? deviceRequests : null,
                        NanoCPUs = inModel.Cpu > 0 ? (long)inModel.Cpu * 1_000_000_000 : 0,
                        Memory = inModel.Memory > 0 ? (long)inModel.Memory * 1024 * 1024 * 1024 : 0
                    }
                };

                // イメージをPull (エラーは無視してcreateに任せる)
                try
                {
                    await dockerClient.Images.CreateImageAsync(
                        new ImagesCreateParameters { FromImage = inModel.ContainerImage },
                        authConfig,
                        new Progress<JSONMessage>());
                }
                catch (Exception ex)
                {
                    LogWarning($"イメージPullに失敗 (ローカルイメージを使用): {ex.Message}");
                }

                // コンテナ作成
                var createResponse = await dockerClient.Containers.CreateContainerAsync(createParams);

                // コンテナ起動
                bool started = await dockerClient.Containers.StartContainerAsync(createResponse.ID, new ContainerStartParameters());
                if (!started)
                {
                    return Result<RunContainerOutputModel, string>.CreateErrorResult("コンテナの起動に失敗しました。");
                }

                var result = new RunContainerOutputModel
                {
                    Name = inModel.Name,
                    Status = KqiContainerStatus.Running,
                    Host = Environment.MachineName,
                    Configuration = $"Docker container: {createResponse.ID}",
                    PortMappings = inModel.PortMappings?.Select(pm => new PortMappingModel
                    {
                        Name = pm.Name,
                        Protocol = pm.Protocol,
                        TargetPort = pm.TargetPort,
                        Port = pm.Port,
                        NodePort = pm.NodePort
                    }).ToList()
                };
                return Result<RunContainerOutputModel, string>.CreateResult(result);
            }
            catch (Exception e)
            {
                LogError($"RunContainerAsync失敗: {e.Message}");
                return Result<RunContainerOutputModel, string>.CreateErrorResult(e.Message);
            }
        }

        /// <summary>
        /// 全コンテナ情報を取得する
        /// </summary>
        public async Task<Result<IEnumerable<ContainerDetailsInfo>, KqiContainerStatus>> GetAllContainerDetailsInfosAsync(string token, string tenantName = null)
        {
            try
            {
                var filters = new Dictionary<string, IDictionary<string, bool>>
                {
                    { "label", new Dictionary<string, bool> { { $"{KqiManagedLabel}=true", true } } }
                };
                if (!string.IsNullOrEmpty(tenantName))
                {
                    filters["label"].Add($"{KqiTenantLabel}={tenantName}", true);
                }

                var containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters
                {
                    All = true,
                    Filters = filters
                });

                var result = containers.Select(c => new ContainerDetailsInfo
                {
                    Name = c.Labels.ContainsKey(KqiContainerNameLabel) ? c.Labels[KqiContainerNameLabel] : c.Names.FirstOrDefault()?.TrimStart('/'),
                    TenantName = c.Labels.ContainsKey(KqiTenantLabel) ? c.Labels[KqiTenantLabel] : "",
                    Status = MapDockerStateToContainerStatus(c.State),
                    NodeName = Environment.MachineName,
                    NodeIpAddress = "localhost",
                    CreatedAt = c.Created,
                    Image = c.Image,
                    Cpu = 0,
                    Memory = 0,
                    Gpu = 0
                });

                return Result<IEnumerable<ContainerDetailsInfo>, KqiContainerStatus>.CreateResult(result);
            }
            catch (Exception e)
            {
                LogError($"GetAllContainerDetailsInfosAsync失敗: {e.Message}");
                return Result<IEnumerable<ContainerDetailsInfo>, KqiContainerStatus>.CreateErrorResult(KqiContainerStatus.Failed);
            }
        }

        /// <summary>
        /// コンテナステータス取得
        /// </summary>
        public async Task<KqiContainerStatus> GetContainerStatusAsync(string containerName, string tenantName, string token)
        {
            try
            {
                var container = await FindContainerByNameAsync(containerName);
                if (container == null)
                {
                    return KqiContainerStatus.None;
                }
                var inspect = await dockerClient.Containers.InspectContainerAsync(container.ID);
                return MapInspectStateToContainerStatus(inspect.State);
            }
            catch (Exception e)
            {
                LogError($"GetContainerStatusAsync失敗: {e.Message}");
                return KqiContainerStatus.Failed;
            }
        }

        /// <summary>
        /// コンテナ詳細情報取得
        /// </summary>
        public async Task<ContainerDetailsInfo> GetContainerDetailsInfoAsync(string jobName, string tenantName, string token)
        {
            try
            {
                var container = await FindContainerByNameAsync(jobName);
                if (container == null)
                {
                    return null;
                }
                var inspect = await dockerClient.Containers.InspectContainerAsync(container.ID);
                return new ContainerDetailsInfo
                {
                    Name = jobName,
                    TenantName = tenantName,
                    Status = MapInspectStateToContainerStatus(inspect.State),
                    NodeName = Environment.MachineName,
                    NodeIpAddress = "localhost",
                    CreatedAt = inspect.Created,
                    Image = inspect.Config?.Image,
                    Cpu = inspect.HostConfig?.NanoCPUs > 0 ? (float)inspect.HostConfig.NanoCPUs / 1_000_000_000 : 0,
                    Memory = inspect.HostConfig?.Memory > 0 ? (float)inspect.HostConfig.Memory / (1024 * 1024 * 1024) : 0,
                    Gpu = (int)(inspect.HostConfig?.DeviceRequests?.Sum(d => d.Count) ?? 0)
                };
            }
            catch (Exception e)
            {
                LogError($"GetContainerDetailsInfoAsync失敗: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// エンドポイント情報取得
        /// </summary>
        public async Task<ContainerEndpointInfo> GetContainerEndpointInfoAsync(string containerName, string tenantName, string token)
        {
            try
            {
                var container = await FindContainerByNameAsync(containerName);
                if (container == null)
                {
                    return null;
                }
                var inspect = await dockerClient.Containers.InspectContainerAsync(container.ID);
                var endpoints = new List<EndPointInfo>();
                if (inspect.NetworkSettings?.Ports != null)
                {
                    foreach (var port in inspect.NetworkSettings.Ports)
                    {
                        if (port.Value != null && port.Value.Count > 0)
                        {
                            var binding = port.Value.First();
                            int hostPort = int.TryParse(binding.HostPort, out int p) ? p : 0;
                            endpoints.Add(new EndPointInfo
                            {
                                Key = port.Key,
                                Host = containerOptions.WebEndPoint ?? "localhost",
                                Port = hostPort
                            });
                        }
                    }
                }

                return new ContainerEndpointInfo
                {
                    Name = containerName,
                    Status = MapInspectStateToContainerStatus(inspect.State),
                    EndPoints = endpoints,
                    StartedAt = inspect.State?.StartedAt != null ? (DateTime?)DateTime.Parse(inspect.State.StartedAt) : null,
                    Node = Environment.MachineName
                };
            }
            catch (Exception e)
            {
                LogError($"GetContainerEndpointInfoAsync失敗: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// コンテナ削除
        /// </summary>
        public async Task<bool> DeleteContainerAsync(ContainerType type, string containerName, string tenantName, string token)
        {
            try
            {
                var container = await FindContainerByNameAsync(containerName);
                if (container == null)
                {
                    LogWarning($"削除対象コンテナが見つかりません: {containerName}");
                    return false;
                }

                // 停止を試みる
                try
                {
                    await dockerClient.Containers.StopContainerAsync(container.ID, new ContainerStopParameters
                    {
                        WaitBeforeKillSeconds = 10
                    });
                }
                catch (Exception)
                {
                    // 既に停止済みの場合は無視
                }

                // 削除
                await dockerClient.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters
                {
                    Force = true
                });
                return true;
            }
            catch (Exception e)
            {
                LogError($"DeleteContainerAsync失敗: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// ログ取得
        /// </summary>
        public async Task<Result<Stream, KqiContainerStatus>> DownloadLogAsync(string containerName, string tenantName, string token)
        {
            try
            {
                var container = await FindContainerByNameAsync(containerName);
                if (container == null)
                {
                    return Result<Stream, KqiContainerStatus>.CreateErrorResult(KqiContainerStatus.None);
                }

                var multiplexedStream = await dockerClient.Containers.GetContainerLogsAsync(container.ID, false, new ContainerLogsParameters
                {
                    ShowStdout = true,
                    ShowStderr = true,
                    Follow = false
                });

                // MultiplexedStreamをMemoryStreamに変換
                var memoryStream = new MemoryStream();
                var buffer = new byte[4096];
                var readResult = await multiplexedStream.ReadOutputAsync(buffer, 0, buffer.Length, CancellationToken.None);
                while (readResult.Count > 0)
                {
                    await memoryStream.WriteAsync(buffer, 0, readResult.Count);
                    readResult = await multiplexedStream.ReadOutputAsync(buffer, 0, buffer.Length, CancellationToken.None);
                }
                memoryStream.Position = 0;

                return Result<Stream, KqiContainerStatus>.CreateResult((Stream)memoryStream);
            }
            catch (Exception e)
            {
                LogError($"DownloadLogAsync失敗: {e.Message}");
                return Result<Stream, KqiContainerStatus>.CreateErrorResult(KqiContainerStatus.Failed);
            }
        }

        /// <summary>
        /// イベント取得（簡易実装: 空リスト返却）
        /// </summary>
        public Task<Result<IEnumerable<ContainerEventInfo>, KqiContainerStatus>> GetEventsAsync(Tenant tenant, string token)
        {
            var emptyList = Enumerable.Empty<ContainerEventInfo>();
            return Task.FromResult(Result<IEnumerable<ContainerEventInfo>, KqiContainerStatus>.CreateResult(emptyList));
        }

        /// <summary>
        /// Pod名取得（Dockerではコンテナ名をそのまま返す）
        /// </summary>
        public Task<Result<string, KqiContainerStatus>> GetPodNameAsync(string tenantName, string appName, int limit, string token)
        {
            return Task.FromResult(Result<string, KqiContainerStatus>.CreateResult(appName));
        }

        /// <summary>
        /// コンテナ内でBashコマンドを実行する
        /// </summary>
        public async Task<bool> ExecBashCommandAsync(string tenantName, string podName, string command, string container, string token, int intervalMillisec, int maxLoopCount)
        {
            try
            {
                var dockerContainer = await FindContainerByNameAsync(podName);
                if (dockerContainer == null)
                {
                    return false;
                }

                var execCreateResponse = await dockerClient.Exec.ExecCreateContainerAsync(dockerContainer.ID, new ContainerExecCreateParameters
                {
                    AttachStdout = true,
                    AttachStderr = true,
                    Cmd = new List<string> { "/bin/bash", "-c", command }
                });

                using (var stream = await dockerClient.Exec.StartAndAttachContainerExecAsync(execCreateResponse.ID, false))
                {
                    // コマンドの完了を待つ
                    for (int i = 0; i < maxLoopCount; i++)
                    {
                        var execInspect = await dockerClient.Exec.InspectContainerExecAsync(execCreateResponse.ID);
                        if (!execInspect.Running)
                        {
                            return execInspect.ExitCode == 0;
                        }
                        await Task.Delay(intervalMillisec);
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                LogError($"ExecBashCommandAsync失敗: {e.Message}");
                return false;
            }
        }

        #endregion

        #region クラスタ管理

        /// <summary>
        /// ノードラベルマップ取得（シングルノードなので空辞書）
        /// </summary>
        public Task<Result<Dictionary<string, string>, string>> GetNodeLabelMapAsync(string labelKey, List<string> registeredNodeNames)
        {
            var result = new Dictionary<string, string>();
            if (registeredNodeNames != null)
            {
                foreach (var name in registeredNodeNames)
                {
                    result[name] = "";
                }
            }
            return Task.FromResult(Result<Dictionary<string, string>, string>.CreateResult(result));
        }

        /// <summary>
        /// 全ノード情報取得（ローカルマシン1台）
        /// </summary>
        public Task<IEnumerable<NodeInfo>> GetAllNodesAsync(List<string> registeredNodeNames)
        {
            var nodes = new List<NodeInfo>
            {
                new NodeInfo
                {
                    Name = Environment.MachineName,
                    Cpu = Environment.ProcessorCount,
                    Memory = 0, // 正確なメモリ量はプラットフォーム依存のため0
                    Gpu = 0,
                    Labels = new Dictionary<string, string>()
                }
            };
            return Task.FromResult<IEnumerable<NodeInfo>>(nodes);
        }

        /// <summary>
        /// ノードラベル設定（no-op）
        /// </summary>
        public Task<bool> SetNodeLabelAsync(string nodeName, string label, string value)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// クォータ設定（no-op）
        /// </summary>
        public Task<bool> SetQuotaAsync(string tenantName, int cpu, int memory, int gpu)
        {
            return Task.FromResult(true);
        }

        #endregion

        #region 権限管理

        /// <summary>
        /// レジストリトークン登録（no-op: Docker側の認証はRunContainerAsync内で処理）
        /// </summary>
        public Task<bool> RegistRegistryTokenyAsync(RegistRegistryTokenInputModel model)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// テナント登録（no-op）
        /// </summary>
        public Task<bool> RegistTenantAsync(string tenantName)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// ユーザ登録（固定トークンを返す）
        /// </summary>
        public Task<string> RegistUserAsync(string tenantName, string userName)
        {
            return Task.FromResult("local-docker-token");
        }

        /// <summary>
        /// テナント抹消（no-op）
        /// </summary>
        public Task<bool> EraseTenantAsync(string tenantName)
        {
            return Task.FromResult(true);
        }

        #endregion

        #region WebSocket通信

        /// <summary>
        /// Docker exec APIを使用してWebSocket接続を確立する。
        /// ClientWebSocketを返すためにDocker exec streamを内部的にラップする。
        /// </summary>
        public async Task<Result<ClientWebSocket, KqiContainerStatus>> ConnectWebSocketAsync(string jobName, string tenantName, string token)
        {
            try
            {
                var container = await FindContainerByNameAsync(jobName);
                if (container == null)
                {
                    return Result<ClientWebSocket, KqiContainerStatus>.CreateErrorResult(KqiContainerStatus.None);
                }

                // Docker exec でシェルセッションを確立
                // 注意: Docker.DotNetはWebSocket型を直接返さないため、
                // この実装ではnullを返し、ClusterManagementLogicでの処理が必要
                // 将来的にはDocker exec streamのWebSocketプロキシ実装が必要
                LogWarning("Docker mode: WebSocket接続は現在限定的なサポートです。");
                return Result<ClientWebSocket, KqiContainerStatus>.CreateErrorResult(KqiContainerStatus.None);
            }
            catch (Exception e)
            {
                LogError($"ConnectWebSocketAsync失敗: {e.Message}");
                return Result<ClientWebSocket, KqiContainerStatus>.CreateErrorResult(KqiContainerStatus.Failed);
            }
        }

        #endregion

        #region プライベートメソッド

        /// <summary>
        /// NfsVolumeMountModelをローカルパスに変換する
        /// </summary>
        private string ConvertNfsToLocalPath(NfsVolumeMountModel nfs)
        {
            string basePath = containerOptions.LocalStorageBasePath ?? "/var/kqi-local";
            string serverPath = nfs.ServerPath?.TrimStart('/') ?? "";
            string subPath = nfs.SubPath ?? "";

            if (!string.IsNullOrEmpty(subPath))
            {
                return Path.Combine(basePath, serverPath, subPath);
            }
            return Path.Combine(basePath, serverPath);
        }

        /// <summary>
        /// コンテナ名からコンテナを検索する
        /// </summary>
        private async Task<ContainerListResponse> FindContainerByNameAsync(string containerName)
        {
            var filters = new Dictionary<string, IDictionary<string, bool>>
            {
                { "label", new Dictionary<string, bool> { { $"{KqiContainerNameLabel}={containerName}", true } } }
            };

            var containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true,
                Filters = filters
            });

            return containers.FirstOrDefault();
        }

        /// <summary>
        /// Dockerのステート文字列をContainerStatusにマッピングする
        /// </summary>
        private static KqiContainerStatus MapDockerStateToContainerStatus(string state)
        {
            switch (state?.ToLower())
            {
                case "running":
                    return KqiContainerStatus.Running;
                case "exited":
                    return KqiContainerStatus.Completed;
                case "created":
                    return KqiContainerStatus.Running;
                case "paused":
                    return KqiContainerStatus.Running;
                case "restarting":
                    return KqiContainerStatus.Running;
                case "dead":
                    return KqiContainerStatus.Killed;
                default:
                    return KqiContainerStatus.None;
            }
        }

        /// <summary>
        /// InspectのContainerStateをContainerStatusにマッピングする
        /// </summary>
        private static KqiContainerStatus MapInspectStateToContainerStatus(ContainerState state)
        {
            if (state == null)
            {
                return KqiContainerStatus.None;
            }
            if (state.Running)
            {
                return KqiContainerStatus.Running;
            }
            if (state.OOMKilled)
            {
                return KqiContainerStatus.OOMKilled;
            }
            if (state.Dead)
            {
                return KqiContainerStatus.Killed;
            }
            // ExitCode 0 = 正常終了
            if (state.ExitCode == 0)
            {
                return KqiContainerStatus.Completed;
            }
            return KqiContainerStatus.Error;
        }

        #endregion
    }
}
