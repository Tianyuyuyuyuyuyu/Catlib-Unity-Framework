using AdsPlatform.Runtime;
using Debug = UnityEngine.Debug;
namespace Wingjoy.Framework.Runtime.Ads
{
    public class AdsComponent : WingjoyFrameworkComponent
    {
        /// <summary>
        /// 广告平台
        /// </summary>
        private IAdsPlatform m_AdsPlatform;
        public IAdsPlatform AdsPlatform => m_AdsPlatform;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="adsPlatform">广告平台</param>
        public void Init(IAdsPlatform adsPlatform)
        {
            m_AdsPlatform = adsPlatform;
        }

        /// <summary>
        /// 加载激励视频
        /// </summary>
        /// <param name="placeId">广告位</param>
        /// <param name="loadEvent">加载事件</param>
        public void LoadRewardVideo(string placeId, RewardVideoLoadEvent loadEvent)
        {
            m_AdsPlatform.LoadRewardedVideoAd(placeId, loadEvent);
        }

        /// <summary>
        /// 播放激励视频，如果没有广告则自动加载
        /// </summary>
        /// <param name="placeId">广告位</param>
        /// <param name="playEvent">播放事件</param>
        public void ShowRewardVideo(string placeId, RewardVideoPlayEvent playEvent)
        {
            if (IsRewardVideoReady(placeId))
            {
                m_AdsPlatform.ShowRewardedVideoAd(placeId, playEvent);
            }
            else
            {
                Debug.Log("广告没有准备好，开始加载");
                RewardVideoLoadEvent loadEvent = new RewardVideoLoadEvent();
                loadEvent.LoadResult += (s, b) =>
                {
                    if (b)
                    {
                        ShowRewardVideo(s, playEvent);
                    }
                };
                LoadRewardVideo(placeId, loadEvent);
            }
        }

        /// <summary>
        /// 广告是否准备好
        /// </summary>
        /// <param name="placeId">广告位</param>
        /// <returns>是否</returns>
        public bool IsRewardVideoReady(string placeId)
        {
            return m_AdsPlatform.IsRewardVideoReady(placeId);
        }
    }
}