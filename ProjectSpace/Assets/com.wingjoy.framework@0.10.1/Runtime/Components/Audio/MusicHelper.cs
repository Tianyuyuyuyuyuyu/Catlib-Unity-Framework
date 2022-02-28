namespace Wingjoy.Framework.Runtime.Audio
{
    public class MusicHelper : AudioSourceHelper
    {
        public override float SettingVolume => CoreMain.Audio.MusicVolume;
    }
}