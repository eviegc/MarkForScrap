using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace MarkForScrap
{
    [DisallowMultipleComponent]
    public class InventoryScrapCounter : NetworkBehaviour
    {
        private Inventory inventory;
        private readonly SyncListBool markedForScrap = new SyncListBool();

        public void Awake()
        {
            CharacterMaster.onStartGlobal += OnCharacterMasterStart;
        }

        public void OnDestroy()
        {
            CharacterMaster.onStartGlobal -= OnCharacterMasterStart;
        }

        // We want to do this in OnStartServer() but because of our setup that doesn't
        // seem to get called. Maybe we can hack it with a Start ServerCallback
        [ServerCallback]
        public void Start()
        {
            if (isServer)
            {
                markedForScrap.Clear();
                for (int i = 0; i < ItemCatalog.itemCount; i++)
                    markedForScrap.Add(false);
            }

            if (PluginConfig.DebugLogs.Value)
                MarkForScrapPlugin.Log.LogDebug($"Found {ItemCatalog.itemCount} total items");
        }

        public void OnCharacterMasterStart(CharacterMaster master)
        {
            if (!isServer)
                return;

            var pcmc = master.playerCharacterMasterController;
            if (pcmc == null || pcmc.networkUser == null || master.inventory == null)
                return;

            inventory = master.inventory;
            inventory.onInventoryChanged += SyncScrapCountWithInventory;

            if (PluginConfig.DebugLogs.Value)
                MarkForScrapPlugin.Log.LogDebug(
                    "Added SyncScrapCountWithInventory hook to player inventory"
                );
        }

        public void MarkItem(ItemIndex idx)
        {
            if (PluginConfig.DebugLogs.Value)
                MarkForScrapPlugin.Log.LogDebug($"Marking ItemIndex {idx} for scrap");
            CmdSetItemMark(idx, true);
        }

        public void UnmarkItem(ItemIndex idx)
        {
            if (PluginConfig.DebugLogs.Value)
                MarkForScrapPlugin.Log.LogDebug($"Unmarking ItemIndex {idx} for scrap");
            CmdSetItemMark(idx, false);
        }

        public void FlipMark(ItemIndex idx)
        {
            if (IsMarked(idx))
                UnmarkItem(idx);
            else
                MarkItem(idx);
        }

        public bool IsMarked(ItemIndex idx)
        {
            if ((int)idx >= markedForScrap.Count)
                return false;
            return markedForScrap[(int)idx];
        }

        public bool HasItemsToScrap()
        {
            for (int i = 0; i < markedForScrap.Count; i++)
            {
                if (markedForScrap[i])
                {
                    if (PluginConfig.DebugLogs.Value)
                        MarkForScrapPlugin.Log.LogDebug("We have marked items to scrap");
                    return true;
                }
            }

            if (PluginConfig.DebugLogs.Value)
                MarkForScrapPlugin.Log.LogDebug("There are no marked items to scrap");
            return false;
        }

        [Command]
        private void CmdSetItemMark(ItemIndex idx, bool marked)
        {
            if (!Utils.ItemUtils.IsScrappable(idx) || inventory.GetItemCountPermanent(idx) <= 0)
                return;

            int intIdx = (int)idx;
            while (intIdx >= markedForScrap.Count)
                markedForScrap.Add(false);

            if (markedForScrap[intIdx] != marked)
                markedForScrap[intIdx] = marked;

            if (PluginConfig.DebugLogs.Value)
                MarkForScrapPlugin.Log.LogDebug($"ItemIndex {idx} has been set to marked={marked}");
        }

        [Server]
        private void SyncScrapCountWithInventory()
        {
            // TODO I just don't like it
            for (int i = 0; i < markedForScrap.Count; ++i)
            {
                if (!markedForScrap[i])
                    continue;

                if (inventory.GetItemCountPermanent((ItemIndex)i) <= 0)
                    markedForScrap[i] = false;
            }
        }

        [Server]
        public ItemIndex Take()
        {
            if (!HasItemsToScrap())
                throw new System.Exception("No items marked for scrap");

            int markedItemIdx;
            for (markedItemIdx = 0; markedItemIdx < markedForScrap.Count; markedItemIdx++)
            {
                if (markedForScrap[markedItemIdx])
                    break;
            }

            UnmarkItem((ItemIndex)markedItemIdx);
            return (ItemIndex)markedItemIdx;
        }
    }
}
