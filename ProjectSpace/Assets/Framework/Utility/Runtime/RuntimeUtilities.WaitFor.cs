using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Utility.Runtime
{
    public static partial class RuntimeUtilities
    {
        public static class WaitFor
        {
            public enum WaitForSelector
            {
                [LabelText("0.05s")]
                Second_0_0_5,
                [LabelText("0.1s")]
                Second_0_1,
                [LabelText("0.15s")]
                Second_0_1_5,
                [LabelText("0.2s")]
                Second_0_2,
                [LabelText("0.3s")]
                Second_0_3,
                [LabelText("0.4s")]
                Second_0_4,
                [LabelText("0.5s")]
                Second_0_5,
                [LabelText("1s")]
                Second_1,
                [LabelText("1.5s")]
                Sceond_1_5,
                [LabelText("2s")]
                Second_2,
                [LabelText("2.5s")]
                Second_2_5,
                [LabelText("3s")]
                Second_3,
                [LabelText("5s")]
                Second_5,
                [LabelText("10s")]
                Second_10,

                [LabelText("0.05s - real")]
                Second_0_0_5_r,
                [LabelText("0.1s - real")]
                Second_0_1_r,
                [LabelText("0.5s - real")]
                Second_0_5_r,
                [LabelText("1s - real")]
                Second_1_r,
                [LabelText("2s - real")]
                Second_2_r,
                [LabelText("3s - real")]
                Second_3_r,
            }

            public static WaitForSeconds Second_60 = new WaitForSeconds(60);
            public static WaitForSeconds Second_10 = new WaitForSeconds(10f);
            public static WaitForSeconds Second_5 = new WaitForSeconds(5f);
            public static WaitForSeconds Second_3 = new WaitForSeconds(3f);
            public static WaitForSeconds Second_2_5 = new WaitForSeconds(2.5f);
            public static WaitForSeconds Second_2 = new WaitForSeconds(2f);
            public static WaitForSeconds Second_1_5 = new WaitForSeconds(1.5f);
            public static WaitForSeconds Second_1 = new WaitForSeconds(1f);
            public static WaitForSeconds Second_0_8 = new WaitForSeconds(0.8f);
            public static WaitForSeconds Second_0_5 = new WaitForSeconds(0.5f);
            public static WaitForSeconds Second_0_4 = new WaitForSeconds(0.4f);
            public static WaitForSeconds Second_0_3 = new WaitForSeconds(0.3f);
            public static WaitForSeconds Second_0_2 = new WaitForSeconds(0.2f);
            public static WaitForSeconds Second_0_1_5 = new WaitForSeconds(0.15f);
            public static WaitForSeconds Second_0_1 = new WaitForSeconds(0.1f);
            public static WaitForSeconds Second_0_0_5 = new WaitForSeconds(0.05f);

            public static WaitForSecondsRealtime RealtimeSecond_3 = new WaitForSecondsRealtime(3f);
            public static WaitForSecondsRealtime RealtimeSecond_2 = new WaitForSecondsRealtime(2f);
            public static WaitForSecondsRealtime RealtimeSecond_1 = new WaitForSecondsRealtime(1f);
            public static WaitForSecondsRealtime RealtimeSecond_0_5 = new WaitForSecondsRealtime(0.5f);
            public static WaitForSecondsRealtime RealtimeSecond_0_1 = new WaitForSecondsRealtime(0.1f);
            public static WaitForSecondsRealtime RealtimeSecond_0_0_5 = new WaitForSecondsRealtime(0.05f);
            public static WaitForSecondsRealtime RealtimeSecond_0_0_3 = new WaitForSecondsRealtime(0.03f);

            public static WaitForEndOfFrame EndOfFrame = new WaitForEndOfFrame();
        }
    }
}
