using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Framework.Utility.Runtime;

namespace Framework.Runtime.Audio
{
    [Serializable]
    public class AssetReferenceAudioClip : AssetReferenceT<AudioClip>
    {
        public AssetReferenceAudioClip(string guid) : base(guid)
        {
        }

#if UNITY_EDITOR
        public AssetReferenceAudioClip(AudioClip clip) : base(clip.GetAssetGuid())
        {
        }
#endif
    }
}