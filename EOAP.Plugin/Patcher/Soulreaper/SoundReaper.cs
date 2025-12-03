using HarmonyLib;

namespace EOAP.Plugin.Patcher
{
    [HarmonyPatch(typeof(SoundManager), nameof(SoundManager.playSE), [typeof(string), typeof(float), typeof(bool)])]
    public class SoundReaper
    {
        public static void Prefix(string LACDFBMILKJ, float JKMLPDLCGIE, bool PJNLLHLBNGP)
        {
            //GDebug.Log($"SFX <{LACDFBMILKJ}> (a1:{JKMLPDLCGIE}) (a2:{PJNLLHLBNGP})");
        }
    }
}
