using UnityEngine;

namespace Wingjoy.Framework.Runtime.Splash
{
    public class SplashAnimation : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            WingjoyFrameworkComponent.GetFrameworkComponent<SplashComponent>().AnimationComplete();
        }
    }
}