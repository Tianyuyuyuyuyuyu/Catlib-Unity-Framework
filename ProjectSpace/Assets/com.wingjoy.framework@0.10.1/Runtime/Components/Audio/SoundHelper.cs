namespace Wingjoy.Framework.Runtime.Audio
{
    public class SoundHelper : AudioSourceHelper
    {
        public override float SettingVolume => CoreMain.Audio.SoundVolume;
    }
}