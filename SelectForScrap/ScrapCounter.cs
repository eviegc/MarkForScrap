using RoR2;
using UnityEngine;
using System.Collections.Generic;

namespace SelectForScrap
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Inventory))]
    public class ScrapCounter : MonoBehaviour
    {
        private Inventory inventory;
        // TODO still need to clean this wrt what is in the inventory, what if we get 
        // benthic'ed 
        private Dictionary<ItemIndex, int> scrapCounter = new Dictionary<ItemIndex, int>();

        public int this[ItemIndex idx]
        {
            get
            {
                scrapCounter.TryGetValue(idx, out var count);
                return Mathf.Clamp(count, 0, inventory.GetItemCount(idx));
            }
            set
            {
                scrapCounter[idx] = Mathf.Clamp(value, 0, inventory.GetItemCount(idx));
            }
        }

        public void Awake()
        {
            Debug.Log("[SelectForScrap] ScrapCounter.Awake()");

            inventory = GetComponent<Inventory>();

            // TODO come back
            // On.RoR2.Inventory.RemoveItem_ItemIndex_int += Inventory_RemoveItem_ItemIndex_int;
        }

        public static ScrapCounter GetFromLocalBody()
        {
            var localUser = LocalUserManager.GetFirstLocalUser();
            var inv = localUser?.cachedMasterController?.master?.inventory;
            if (!inv) return null;

            return inv.gameObject.GetComponent<ScrapCounter>();
        }

        // private void Inventory_RemoveItem_ItemIndex_int(On.RoR2.Inventory.orig_RemoveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        // {
        //     Debug.Log("[SelectForScrap] ScrapCounter.Inventory_RemoveItem_ItemIndex_int()");

        //     orig(self, itemIndex, count);

        //     if (!self) return;

        //     var scrapCounter = self.gameObject.GetComponent<ScrapCounter>();
        //     if (!scrapCounter) return;

        //     scrapCounter[itemIndex] -= count;
        // }
    }
}
