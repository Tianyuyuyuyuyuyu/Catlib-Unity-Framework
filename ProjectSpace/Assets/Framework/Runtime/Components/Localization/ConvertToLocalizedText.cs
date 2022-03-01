using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Runtime.Localization
{
    public class ConvertToLocalizedText : MonoBehaviour
    {
        [Button(ButtonSizes.Large)]
        public void Convert()
        {
            var component = GetComponent<Text>();
            if (component != null)
            {
                var instantiate = Instantiate(component);
                DestroyImmediate(component);
                var localizedText = gameObject.AddComponent<LocalizedText>();
                localizedText.text = instantiate.text;
                localizedText.font = instantiate.font;
                localizedText.fontStyle = instantiate.fontStyle;
                localizedText.fontSize = instantiate.fontSize;
                localizedText.lineSpacing = instantiate.lineSpacing;
                localizedText.supportRichText = instantiate.supportRichText;
                localizedText.alignment = instantiate.alignment;
                localizedText.alignByGeometry = instantiate.alignByGeometry;
                localizedText.horizontalOverflow = instantiate.horizontalOverflow;
                localizedText.verticalOverflow = instantiate.verticalOverflow;
                localizedText.resizeTextForBestFit = instantiate.resizeTextForBestFit;
                localizedText.color = instantiate.color;
                localizedText.material = instantiate.material;
                localizedText.raycastTarget = instantiate.raycastTarget;
                localizedText.maskable = instantiate.maskable;
                localizedText.GenerateLocalizationKey();
                DestroyImmediate(instantiate.gameObject);
                DestroyImmediate(this);
            }
        }

        [Button(ButtonSizes.Large)]
        public void AntiConvert()
        {
            var component = GetComponent<LocalizedText>();
            if (component != null)
            {
                var instantiate = Instantiate(component);
                DestroyImmediate(component);
                var localizedText = gameObject.AddComponent<Text>();
                localizedText.text = instantiate.text;
                localizedText.font = instantiate.font;
                localizedText.fontStyle = instantiate.fontStyle;
                localizedText.fontSize = instantiate.fontSize;
                localizedText.lineSpacing = instantiate.lineSpacing;
                localizedText.supportRichText = instantiate.supportRichText;
                localizedText.alignment = instantiate.alignment;
                localizedText.alignByGeometry = instantiate.alignByGeometry;
                localizedText.horizontalOverflow = instantiate.horizontalOverflow;
                localizedText.verticalOverflow = instantiate.verticalOverflow;
                localizedText.resizeTextForBestFit = instantiate.resizeTextForBestFit;
                localizedText.color = instantiate.color;
                localizedText.material = instantiate.material;
                localizedText.raycastTarget = instantiate.raycastTarget;
                localizedText.maskable = instantiate.maskable;
                DestroyImmediate(instantiate.gameObject);
                DestroyImmediate(this);
            }
        }
    }
}