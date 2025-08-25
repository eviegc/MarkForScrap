using RoR2;

namespace SelectForScrap.Utils
{
    public static class ItemUtils
    {
        public static bool IsScrappable(ItemIndex idx)
        {
            ItemDef def = ItemCatalog.GetItemDef(idx);
            if (def == null) return false;

            if (def.ContainsTag(ItemTag.Scrap)) return false;
            if (def.ContainsTag(ItemTag.WorldUnique)) return false;
            if (def.hidden) return false;
            
            switch (def.tier)
            {
                case ItemTier.Tier1:
                case ItemTier.Tier2:
                case ItemTier.Tier3:
                case ItemTier.Boss:
                    return true;
                default:
                    return false;
            }
        }
    }
}