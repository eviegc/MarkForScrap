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
            inventory.onInventoryChanged += SyncScrapCountWithInventory;
        }

        public void OnDestroy()
        {
            Debug.Log("[SelectForScrap] ScrapCounter.OnDestroy()");

            // Don't think this is strictly necessary, but just in case
            if (inventory) inventory.onInventoryChanged -= SyncScrapCountWithInventory;
        }

        private void SyncScrapCountWithInventory()
        {
            Debug.Log("[SelectForScrap] ScrapCounter.SyncScrapCountWithInventory()");

            List<ItemIndex> tracking = [.. scrapCounter.Keys];
            foreach (var idx in tracking)
            {
                // The Benthic Bloom safeguard. The scrapCount[idx] is only ever updated
                // via IPointerClickHandler events, so if one of our stacks changes under
                // us we'd hold on to a scrap count > 0. If we then reacquire that item
                // it'll have a non-zero scrap count. 
                // We fix by removing (/zero out) our scrap count for any item we own 0 of.
                // All other synchronisation (value clamp) happens in the getter.
                if (inventory.GetItemCount(idx) == 0) scrapCounter.Remove(idx);
            }
        }
    }
}
