using Framework.Runtime.Core;

namespace Framework.Runtime.Audio
{
    public class MusicHelper : AudioSourceHelper
    {
        public override float SettingVolume => CoreMain.Audio.MusicVolume;
    }
}