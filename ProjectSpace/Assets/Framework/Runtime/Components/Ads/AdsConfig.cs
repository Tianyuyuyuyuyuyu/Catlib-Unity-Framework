using UnityEngine;

namespace Framework.Runtime.Ads
{
    public class AdsConfig : ScriptableObject
    {
        /// <summary>
        /// APPID
        /// </summary>
        public string AppId;

        /// <summary>
        /// APP钥匙
        /// </summary>
        public string AppKey;

        /// <summary>
        /// 视频广告位
        /// </summary>
        public string Video;

        /// <summary>
        /// 插屏广告位
        /// </summary>
        public string Interstitial;

        /// <summary>
        /// 横幅广告位
        /// </summary>
        public string Banner;
    }
}