namespace Wingjoy.Framework.Runtime.Localization
{
    public interface ILocalizedCom
    {
        /// <summary>
        /// 是否使能
        /// </summary>
        bool EnableLocalization
        {
            get;
        }

        /// <summary>
        /// 本地化键值
        /// </summary>
        string GetLocalizationKey();

        /// <summary>
        /// 本地化内容
        /// </summary>
        string GetContent();

        /// <summary>
        /// 本地化
        /// </summary>
        void DoLocalize();
    }
}