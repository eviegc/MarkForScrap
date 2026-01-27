using RoR2;

namespace MarkForScrap.Utils
{
    public static class ItemUtils
    {
        public static bool IsScrappable(ItemIndex idx)
        {
            ItemDef def = ItemCatalog.GetItemDef(idx);
            ItemTierDef tierDef = ItemTierCatalog.GetItemTierDef(def.tier);

            if (
                !def.canRemove
                || !tierDef.canScrap
                || def.ContainsTag(ItemTag.Scrap)
                || def.ContainsTag(ItemTag.ObjectiveRelated)
            )
                return false;

            return true;
        }
    }
}
