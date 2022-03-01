using System;
using UnityEngine.AddressableAssets;
using Framework.Utility.Runtime;

namespace Framework.Runtime.Audio
{
    [Serializable]
    public class AssetReferenceAudioLibrary : AssetReferenceT<AudioLibrary>
    {
        public AssetReferenceAudioLibrary(string guid) : base(guid)
        {
        }

#if UNITY_EDITOR
        public AssetReferenceAudioLibrary(AudioLibrary library) : base(library.GetAssetGuid())
        {
        }
#endif
    }
}