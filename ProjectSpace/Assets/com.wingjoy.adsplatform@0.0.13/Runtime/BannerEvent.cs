using System;

namespace AdsPlatform.Runtime
{
    public class BannerEvent
    {
        /// <summary>
        /// 自动刷新
        /// </summary>
        public event Action<string> AutoRefresh;
        
        /// <summary>
        /// 自动刷新失败
        /// </summary>
        public event Action<string> AutoRefreshFailed;

        /// <summary>
        /// 点击回调
        /// </summary>
        public event Action<string> Click;

        /// <summary>
        /// 展示回调
        /// </summary>
        public event Action<string> Impress;

        /// <summary>
        /// 加载结果
        /// </summary>
        public event Action<string, bool> LoadResult;

        /// <summary>
        /// 关闭回调
        /// </summary>
        public event Action<string> Close;

        /// <summary>
        /// 自动刷新
        /// </summary>
        /// <param name="placeId">广告位</param>
        public virtual void OnAutoRefresh(string placeId)
        {
            AutoRefresh?.Invoke(placeId);
        }

        /// <summary>
        /// 自动刷新失败
        /// </summary>
        /// <param name="placeId">广告位</param>
        public virtual void OnAutoRefreshFailed(string placeId)
        {
            AutoRefreshFailed?.Invoke(placeId);
        }

        /// <summary>
        /// 点击
        /// </summary>
        /// <param name="placeId">广告位</param>
        public virtual void OnClick(string placeId)
        {
            Click?.Invoke(placeId);
        }

        /// <summary>
        /// 展示
        /// </summary>
        /// <param name="placeId">广告位</param>
        public virtual void OnImpress(string placeId)
        {
            Impress?.Invoke(placeId);
        }

        /// <summary>
        /// 加载结果
        /// </summary>
        /// <param name="placeId">广告位</param>
        /// <param name="result">是否成功</param>
        public virtual void OnLoadResult(string placeId, bool result)
        {
            LoadResult?.Invoke(placeId, result);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="placeId">广告位</param>
        public virtual void OnClose(string placeId)
        {
            Close?.Invoke(placeId);
        }
    }
}
