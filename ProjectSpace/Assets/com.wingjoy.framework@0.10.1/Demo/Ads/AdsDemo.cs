using AdsPlatform.Runtime;
#if USE_ANYTHINK
//using AnyThinkAds;
#endif
using UnityEngine;
using Wingjoy.Framework.Runtime;

namespace WinjoyFramework.Demo.Ads
{
//     public class AdsDemo : MonoBehaviour
//     {
//         private IAdsPlatform AdsPlatform;
//
//         public void Init()
//         {
//             var initEvent = new InitEvent();
// #if USE_ANYTHINK
//             var AdsPlatform= new AnyThinkPlatform();
//             AdsPlatform.Init("a606c59d3540c3", "56865ac7ed373be0820ebde6bf41f024", initEvent);
// #endif
//             CoreBase.Ads.Init(AdsPlatform);
//         }
//
//         public void ShowReward()
//         {
//             var rewardVideoLoadEvent = new RewardVideoLoadEvent();
//             rewardVideoLoadEvent.LoadResult += RewardVideoLoadEventOnLoadResult;
//             AdsPlatform.LoadRewardedVideoAd("b606c59db83a50", rewardVideoLoadEvent);
//         }
//
//         private void RewardVideoLoadEventOnLoadResult(string arg1, bool arg2)
//         {
//             if (arg2)
//             {
//                 var rewardVideoLoadEvent = new RewardVideoPlayEvent();
//                 rewardVideoLoadEvent.Reward += s =>
//                 {
//                     Debug.Log("OnReward");
//                 };
//                 AdsPlatform.ShowRewardedVideoAd(arg1, rewardVideoLoadEvent);
//             }
//         }
//     }
}
