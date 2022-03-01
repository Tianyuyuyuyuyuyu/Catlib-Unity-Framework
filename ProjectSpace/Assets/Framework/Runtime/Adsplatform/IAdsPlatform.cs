namespace AdsPlatform.Runtime
{
    public interface IAdsPlatform
    {
        /// <summary>
        /// 初始化SDK
        /// </summary>
        /// <param name="id">初始化ID</param>
        /// <param name="appKey">App密钥</param>
        /// <param name="initEvent">初始化事件</param>
        /// <param name="param">参数</param>
        void Init(string id, string appKey, InitEvent initEvent, Param param = null);

        /// <summary>
        /// 加载插屏广告
        /// </summary>
        /// <param name="placeId">广告位</param>
        /// <param name="interstitialLoadEvent">插屏加载事件</param>
        /// <param name="param">参数</param>
        void LoadInterstitialAd(string placeId, InterstitialLoadEvent interstitialLoadEvent, Param param = null);

        /// <summary>
        /// 展示开屏广告
        /// </summary>
        /// <param name="placeId">广告位</param>
        /// <param name="interstitialPlayEvent">插屏播放事件</param>
        /// <param name="param">参数</param>
        void ShowInterstitialAd(string placeId, InterstitialPlayEvent interstitialPlayEvent, Param param = null);

        /// <summary>
        /// 加载激励视频广告
        /// </summary>
        /// <param name="placeId">广告位</param>
        /// <param name="rewardVideoLoadEvent">激励视频加载事件</param>
        /// <param name="param">参数</param>
        void LoadRewardedVideoAd(string placeId, RewardVideoLoadEvent rewardVideoLoadEvent, Param param = null);

        /// <summary>
        /// 展示激励视频广告
        /// </summary>
        /// <param name="placeId">广告位</param>
        /// <param name="rewardVideoLoadEvent">激励视频播放事件</param>
        /// <param name="param">参数</param>
        void ShowRewardedVideoAd(string placeId, RewardVideoPlayEvent rewardVideoLoadEvent, Param param = null);

        /// <summary>
        /// 广告是否准备好,如果部分渠道没有此功能，则返回TRUE
        /// </summary>
        /// <param name="placeId">广告位</param>
        /// <returns>是否</returns>
        bool IsRewardVideoReady(string placeId);

        /// <summary>
        /// 加载横幅广告
        /// </summary>
        /// <param name="placeId">广告位</param>
        /// <param name="bannerEvent">横幅事件</param>
        /// <param name="param">参数</param>
        void LoadBannerAd(string placeId, BannerEvent bannerEvent, Param param = null);

        /// <summary>
        /// 移除横幅广告
        /// </summary>
        /// <param name="placeId">广告位</param>
        /// <param name="param">参数</param>
        void RemoveBannerAd(string placeId, Param param = null);
    }
}