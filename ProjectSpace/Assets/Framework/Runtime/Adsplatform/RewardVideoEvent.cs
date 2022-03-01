using System;

namespace AdsPlatform.Runtime
{
    public class RewardVideoLoadEvent
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

    public class RewardVideoPlayEvent
    {
        /// <summary>
        /// 播放开始
        /// </summary>
        public event Action<string> PlayStart;

        /// <summary>
        /// 播放结束
        /// </summary>
        public event Action<string> PlayEnd;

        /// <summary>
        /// 播放失败
        /// </summary>
        public event Action<string> PlayFail;

        /// <summary>
        /// 播放点击
        /// </summary>
        public event Action<string> PlayClick;

        /// <summary>
        /// 关闭
        /// </summary>
        public event Action<string> Close;

        /// <summary>
        /// 奖励发放
        /// </summary>
        public event Action<string> Reward;

        /// <summary>
        /// 发放奖励
        /// </summary>
        /// <param name="placementId">广告位</param>
        public virtual void OnReward(string placementId)
        {
            Reward?.Invoke(placementId);
        }

        /// <summary>
        /// 播放开始回调
        /// </summary>
        /// <param name="placeId">广告位</param>
        public virtual void OnPlay(string placeId)
        {
            PlayStart?.Invoke(placeId);
        }

        /// <summary>
        /// 播放结束回调
        /// </summary>
        /// <param name="placeId">广告位</param>
        public virtual void OnPlayEnd(string placeId)
        {
            PlayEnd?.Invoke(placeId);
        }

        /// <summary>
        /// 播放失败回调
        /// </summary>
        /// <param name="placeId">广告位</param>
        public virtual void OnPlayFail(string placeId)
        {
            PlayFail?.Invoke(placeId);
        }

        /// <summary>
        /// 点击关闭回调
        /// </summary>
        /// <param name="placeId">广告位</param>
        public virtual void OnClose(string placeId)
        {
            Close?.Invoke(placeId);
        }

        /// <summary>
        /// 播放点击回调
        /// </summary>
        /// <param name="placeId">广告位</param>
        public virtual void OnPlayClick(string placeId)
        {
            PlayClick?.Invoke(placeId);
        }
    }
}
