using Nssol.Platypus.Models;

namespace Nssol.Platypus.ApiModels.CustomRoleApiModels
{
    /// <summary>
    /// カスタムロール出力モデル
    /// </summary>
    public class IndexOutputModel : Components.OutputModelBase
    {
        public IndexOutputModel(CustomRole role) : base(role)
        {
            Id = role.Id;
            Name = role.Name;
            DisplayName = role.DisplayName;
            Description = role.Description;
            CanManageData = role.CanManageData;
            CanManageDataSet = role.CanManageDataSet;
            CanRunTraining = role.CanRunTraining;
            CanRunInference = role.CanRunInference;
            CanUseNotebook = role.CanUseNotebook;
            CanManageProject = role.CanManageProject;
            CanEditTenantSetting = role.CanEditTenantSetting;
            CanManageResourcePermission = role.CanManageResourcePermission;
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool CanManageData { get; set; }
        public bool CanManageDataSet { get; set; }
        public bool CanRunTraining { get; set; }
        public bool CanRunInference { get; set; }
        public bool CanUseNotebook { get; set; }
        public bool CanManageProject { get; set; }
        public bool CanEditTenantSetting { get; set; }
        public bool CanManageResourcePermission { get; set; }
    }
}
