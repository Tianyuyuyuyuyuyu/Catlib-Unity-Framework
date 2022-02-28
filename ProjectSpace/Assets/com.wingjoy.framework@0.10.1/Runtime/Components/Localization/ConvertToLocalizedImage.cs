using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Wingjoy.Framework.Runtime.Localization
{
    public class ConvertToLocalizedImage : MonoBehaviour
    {
        [Button(ButtonSizes.Large)]
        public void Convert()
        {
            var component = GetComponent<Image>();
            if (component != null)
            {
                var instantiate = Instantiate(component);
                DestroyImmediate(component);
                var localizedImage = gameObject.AddComponent<LocalizedImage>();
                localizedImage.sprite = instantiate.sprite;
                localizedImage.color = instantiate.color;
                localizedImage.material = instantiate.material;
                localizedImage.raycastTarget = instantiate.raycastTarget;
                localizedImage.maskable = instantiate.maskable;
                localizedImage.type = instantiate.type;
                localizedImage.useSpriteMesh = instantiate.useSpriteMesh;
                localizedImage.preserveAspect = instantiate.preserveAspect;
                localizedImage.GenerateLocalizationKey();
                localizedImage.SetNativeSize();
                DestroyImmediate(instantiate.gameObject);
                DestroyImmediate(this);
            }
        }

        [Button(ButtonSizes.Large)]
        public void AntiConvert()
        {
            var component = GetComponent<LocalizedImage>();
            if (component != null)
            {
                var instantiate = Instantiate(component);
                DestroyImmediate(component);
                var image = gameObject.AddComponent<Image>();
                image.sprite = instantiate.sprite;
                image.color = instantiate.color;
                image.material = instantiate.material;
                image.raycastTarget = instantiate.raycastTarget;
                image.maskable = instantiate.maskable;
                image.SetNativeSize();
                DestroyImmediate(instantiate.gameObject);
                DestroyImmediate(this);
            }
        }
    }
}