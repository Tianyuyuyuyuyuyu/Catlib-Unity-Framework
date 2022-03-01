using Framework.Runtime.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Runtime.Audio
{
    [RequireComponent(typeof(Button))]
    public class AudioButtonClick : MonoBehaviour
    {
        /// <summary>
        /// 音频文件
        /// </summary>
        [SerializeField]
        private AudioClipObject m_AudioClipObject;


        private void OnEnable()
        {
            var button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener((Call));
            }
        }

        private void Call()
        {
            CoreMain.Audio.PlaySound(m_AudioClipObject);
        }

        private void OnDisable()
        {
            var button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveListener(Call);
            }
        }
    }
}