using RoR2;

namespace SelectForScrap.Utils
{
    public static class LocalUser
    {
        public static CharacterMaster master
        {
            // This won't work with local split screen, but oh well
            get { return LocalUserManager.GetFirstLocalUser()?.cachedMasterController?.master; }
        }

        public static ScrapCounter scrapCounter
        {
            get { return master?.GetComponent<ScrapCounter>(); }
        }

        public static bool IsLocal(CharacterMaster master)
        {
            var pcmc = master ? master.playerCharacterMasterController : null;
            var nu = pcmc ? pcmc.networkUser : null;
            return nu && nu.localUser != null;
        }
    }
}