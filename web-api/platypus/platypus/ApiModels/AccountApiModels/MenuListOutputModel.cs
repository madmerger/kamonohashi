using Nssol.Platypus.Infrastructure.Infos;

namespace Nssol.Platypus.ApiModels.AccountApiModels
{
    public class MenuListOutputModel
    {
        public MenuListOutputModel(MenuItemInfo menu, string lang)
        {
            if (lang == "es" && !string.IsNullOrEmpty(menu.NameEs))
                this.Name = menu.NameEs;
            else if (lang == "en" && !string.IsNullOrEmpty(menu.NameEn))
                this.Name = menu.NameEn;
            else
                this.Name = menu.Name;

            if (lang == "es" && !string.IsNullOrEmpty(menu.DescriptionEs))
                this.Description = menu.DescriptionEs;
            else if (lang == "en" && !string.IsNullOrEmpty(menu.DescriptionEn))
                this.Description = menu.DescriptionEn;
            else
                this.Description = menu.Description;
            this.Url = menu.Url;
            this.Category = menu.Category;
        }

        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 説明
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// メニューカテゴリ。
        /// 表示側でアイコンや色などを種別ごとに変更する際などに使用される想定。
        /// </summary>
        public string Category { get; set; }
    }
}
