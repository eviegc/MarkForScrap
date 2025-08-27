using RoR2;

namespace MarkForScrap.Utils
{
    public static class LocalUser
    {
        public static NetworkUser networkUser
        {
            // This won't work with local split screen, but oh well
            get { return LocalUserManager.GetFirstLocalUser()?.currentNetworkUser; }
        }

        public static CharacterMaster master
        {
            get { return networkUser?.master; }
        }

        public static InventoryScrapCounter scrapCounter
        {
            get { return networkUser?.GetComponent<InventoryScrapCounter>(); }
        }
    }
}
