namespace Nssol.Platypus.Infrastructure
{
    /// <summary>
    /// プロジェクト内のロール種別
    /// </summary>
    public enum ProjectRoleType
    {
        /// <summary>
        /// 閲覧のみ
        /// </summary>
        Viewer = 1,

        /// <summary>
        /// 実行（学習・推論の実行が可能）
        /// </summary>
        Executor = 2,

        /// <summary>
        /// 管理（プロジェクト設定の変更が可能）
        /// </summary>
        Manager = 3,
    }
}
