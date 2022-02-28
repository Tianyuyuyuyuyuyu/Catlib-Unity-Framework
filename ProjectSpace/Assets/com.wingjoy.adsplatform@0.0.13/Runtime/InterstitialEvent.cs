using System;

namespace AdsPlatform.Runtime
{
    public class InterstitialLoadEvent
    {
        /// <summary>
        /// 加载结果
        /// </summary>
        public event Action<string, bool> LoadResult;

        /// <summary>
        /// 加载结果回调
        /// </summary>
        /// <param name="placeId">广告位ID</param>
        /// <param name="result">结果</param>
        public virtual void OnLoadResult(string placeId, bool result)
        {
            LoadResult?.Invoke(placeId, result);
        }
    }

    public class InterstitialPlayEvent
    {
        /// <summary>
        /// 广告播放
        /// </summary>
        public event Action<string> AdShow;

        /// <summary>
        /// 广告播放失败
        /// </summary>
        public event Action<string> AdFailedToShow;

        /// <summary>
        /// 广告关闭
        /// </summary>
        public event Action<string> AdClose;

        /// <summary>
        /// 广告点击
        /// </summary>
        public event Action<string> AdClick;

        /// <summary>
        /// 开始播放视频
        /// </summary>
        public event Action<string> AdStartPlayingVideo;

        /// <summary>
        /// 播放视频结束
        /// </summary>
        public event Action<string> AdEndPlayingVideo;

        /// <summary>
        /// 播放失败
        /// </summary>
        public event Action<string> AdFailedToPlayVideo;

        /// <summary>
        /// 广告播放
        /// </summary>
        /// <param name="placeId">广告位</param>
        public virtual void OnAdShow(string placeId)
        {
            AdShow?.Invoke(placeId);
        }

        /// <summary>
        /// 播放失败
        /// </summary>
        /// <param name="placeId">广告位</param>
        public virtual void OnAdFailedToShow(string placeId)
        {
            AdFailedToShow?.Invoke(placeId);
        }

        /// <summary>
        /// 开始播放视频
        /// </summary>
        /// <param name="placeId">广告位</param>
        public virtual void OnAdStartPlayingVideo(string placeId)
        {
            AdStartPlayingVideo?.Invoke(placeId);
        }

        /// <summary>
        /// 播放视频结束
        /// </summary>
        /// <param name="placeId">广告位</param>
        public virtual void OnAdClick(string placeId)
        {
            AdClick?.Invoke(placeId);
        }

        /// <summary>
        /// 广告关闭
        /// </summary>
        /// <param name="placeId">广告位</param>
        public virtual void OnAdClose(string placeId)
        {
            AdClose?.Invoke(placeId);
        }

        /// <summary>
        /// 播放视频失败
        /// </summary>
        /// <param name="placeId">广告位</param>
        public virtual void OnAdFailedToPlayVideo(string placeId)
        {
            AdFailedToPlayVideo?.Invoke(placeId);
        }

        /// <summary>
        /// 播放视频结束
        /// </summary>
        /// <param name="placeId">广告位</param>
        public virtual void OnAdEndPlayingVideo(string placeId)
        {
            AdEndPlayingVideo?.Invoke(placeId);
        }
    }
}
