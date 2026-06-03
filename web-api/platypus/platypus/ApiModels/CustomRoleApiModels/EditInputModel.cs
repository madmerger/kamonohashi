using System.ComponentModel.DataAnnotations;

namespace Nssol.Platypus.ApiModels.CustomRoleApiModels
{
    /// <summary>
    /// カスタムロール編集入力モデル
    /// </summary>
    public class EditInputModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
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
