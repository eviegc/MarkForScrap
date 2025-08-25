using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace MarkForScrap
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkUser))]
    public class InventoryScrapCounter : NetworkBehaviour
    {
        private NetworkUser networkUser;
        private Inventory inventory;
        private readonly SyncListBool markedForScrap = new SyncListBool();

        public void Awake()
        {
            networkUser = GetComponent<NetworkUser>();
            CharacterMaster.onStartGlobal += OnCharacterMasterStart;
        }

        // We want to do this in OnStartServer() but because of our setup that doesn't
        // seem to get called. Maybe we can hack it with a Start ServerCallback
        [ServerCallback]
        public void Start()
        {
            Debug.Log("[MarkForScrap] ScrapCounter.Start()");
            if (isServer)
            {
                markedForScrap.Clear();
                for (int i = 0; i < ItemCatalog.itemCount; i++) markedForScrap.Add(false);
            }

            Debug.Log($"[MarkForScrap] ScrapCounter.Start() | ItemCatalog count: {ItemCatalog.itemCount}");
        }

        public void OnCharacterMasterStart(CharacterMaster master)
        {
            Debug.Log("[MarkForScrap] ScrapCounter.OnCharacterMasterStart()");

            var pcmc = master.playerCharacterMasterController;
            if (pcmc && pcmc.networkUser == networkUser) inventory = pcmc.master.inventory;

            Debug.Log($"[MarkForScrap] ScrapCounter.OnCharacterMasterStart() | Inventory found: {inventory != null}");

            if (inventory && isServer) inventory.onInventoryChanged += SyncScrapCountWithInventory;
        }

        public void MarkItem(ItemIndex idx) { CmdSetItemMark(idx, true); }
        public void UnmarkItem(ItemIndex idx) { CmdSetItemMark(idx, false);  }
        public void FlipMark(ItemIndex idx)
        {
            if (IsMarked(idx)) UnmarkItem(idx); else MarkItem(idx);
        }
        public bool IsMarked(ItemIndex idx)
        {
            if ((int)idx >= markedForScrap.Count) return false;
            return markedForScrap[(int)idx];
        }

        public bool HasItemsToScrap()
        {
            Debug.Log("[MarkForScrap] ScrapCounter.HasItemsToScrap()");
            for (int i = 0; i < markedForScrap.Count; i++) { if (markedForScrap[i]) return true; }
            return false;
        }

        [Command]
        private void CmdSetItemMark(ItemIndex idx, bool marked)
        {
            if (!Utils.ItemUtils.IsScrappable(idx)) return;

            Debug.Log("[MarkForScrap] ScrapCounter.CmdSetItemMark()");

            int intIdx = (int)idx;
            while (intIdx >= markedForScrap.Count) markedForScrap.Add(false);

            if (markedForScrap[intIdx] != marked) markedForScrap[intIdx] = marked;

            Debug.Log($"[MarkForScrap] ScrapCounter.CmdSetItemMark() | {idx} : {marked}");
        }

        [Server]
        private void SyncScrapCountWithInventory()
        {
            Debug.Log("[MarkForScrap] ScrapCounter.SyncScrapCountWithInventory()");

            for (int i = 0; i < markedForScrap.Count; ++i)
            {
                if (!markedForScrap[i]) continue;
                if (inventory.GetItemCount((ItemIndex)i) == 0) markedForScrap[i] = false;
            }
        }

        [Server]
        public ItemIndex Take()
        {
            Debug.Log("[MarkForScrap] ScrapCounter.Take()");

            if (!HasItemsToScrap()) throw new System.Exception("No items marked for scrap");

            int markedItemIdx;
            for (markedItemIdx = 0; markedItemIdx < markedForScrap.Count; markedItemIdx++)
            {
                if (markedForScrap[markedItemIdx]) break;
            }

            return (ItemIndex)markedItemIdx;
        }
    }
}
