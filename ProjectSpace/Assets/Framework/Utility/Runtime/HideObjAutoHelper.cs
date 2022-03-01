using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

/// <summary>
/// 自动隐藏物体
/// </summary>
namespace Framework.Utility.Runtime
{
    public class HideObjAutoHelper : MonoBehaviour
    {
        [LabelText("时间")]
        public RuntimeUtilities.WaitFor.WaitForSelector selector;

        private void OnEnable()
        {
            StopAllCoroutines();
            if (gameObject.activeInHierarchy) StartCoroutine(hide());
        }

        IEnumerator hide()
        {
            switch (selector)
            {
                case RuntimeUtilities.WaitFor.WaitForSelector.Second_0_0_5:
                    yield return RuntimeUtilities.WaitFor.Second_0_0_5;
                    break;
                case RuntimeUtilities.WaitFor.WaitForSelector.Second_0_1:
                    yield return RuntimeUtilities.WaitFor.Second_0_1;
                    break;
                case RuntimeUtilities.WaitFor.WaitForSelector.Second_0_1_5:
                    yield return RuntimeUtilities.WaitFor.Second_0_1_5;
                    break;
                case RuntimeUtilities.WaitFor.WaitForSelector.Second_0_2:
                    yield return RuntimeUtilities.WaitFor.Second_0_2;
                    break;
                case RuntimeUtilities.WaitFor.WaitForSelector.Second_0_3:
                    yield return RuntimeUtilities.WaitFor.Second_0_3;
                    break;
                case RuntimeUtilities.WaitFor.WaitForSelector.Second_0_4:
                    yield return RuntimeUtilities.WaitFor.Second_0_4;
                    break;
                case RuntimeUtilities.WaitFor.WaitForSelector.Second_0_5:
                    yield return RuntimeUtilities.WaitFor.Second_0_5;
                    break;
                case RuntimeUtilities.WaitFor.WaitForSelector.Second_1:
                    yield return RuntimeUtilities.WaitFor.Second_1;
                    break;
                case RuntimeUtilities.WaitFor.WaitForSelector.Sceond_1_5:
                    yield return RuntimeUtilities.WaitFor.Second_1_5;
                    break;
                case RuntimeUtilities.WaitFor.WaitForSelector.Second_2:
                    yield return RuntimeUtilities.WaitFor.Second_2;
                    break;
                case RuntimeUtilities.WaitFor.WaitForSelector.Second_2_5:
                    yield return RuntimeUtilities.WaitFor.Second_2_5;
                    break;
                case RuntimeUtilities.WaitFor.WaitForSelector.Second_3:
                    yield return RuntimeUtilities.WaitFor.Second_3;
                    break;
                case RuntimeUtilities.WaitFor.WaitForSelector.Second_5:
                    yield return RuntimeUtilities.WaitFor.Second_5;
                    break;
                case RuntimeUtilities.WaitFor.WaitForSelector.Second_10:
                    yield return RuntimeUtilities.WaitFor.Second_10;
                    break;

                case RuntimeUtilities.WaitFor.WaitForSelector.Second_0_0_5_r:
                    yield return RuntimeUtilities.WaitFor.RealtimeSecond_0_0_5;
                    break;
                case RuntimeUtilities.WaitFor.WaitForSelector.Second_0_1_r:
                    yield return RuntimeUtilities.WaitFor.RealtimeSecond_0_1;
                    break;
                case RuntimeUtilities.WaitFor.WaitForSelector.Second_0_5_r:
                    yield return RuntimeUtilities.WaitFor.RealtimeSecond_0_5;
                    break;
                case RuntimeUtilities.WaitFor.WaitForSelector.Second_1_r:
                    yield return RuntimeUtilities.WaitFor.RealtimeSecond_1;
                    break;
                case RuntimeUtilities.WaitFor.WaitForSelector.Second_2_r:
                    yield return RuntimeUtilities.WaitFor.RealtimeSecond_2;
                    break;
                case RuntimeUtilities.WaitFor.WaitForSelector.Second_3_r:
                    yield return RuntimeUtilities.WaitFor.RealtimeSecond_3;
                    break;
            }
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            gameObject.SetActive(false);
        }
    }
}
