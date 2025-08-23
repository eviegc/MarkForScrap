using System.Security.Permissions;
using BepInEx;
using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using UnityEngine;
using RoR2;

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission( SecurityAction.RequestMinimum, SkipVerification = true )]

namespace SelectForScrap;

[BepInDependency("com.rune580.riskofoptions")]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class SelectForScrapPlugin : BaseUnityPlugin
{
    public void Awake()
    {
        Debug.Log("[SelectForScrap] SelectForScrapPlugin.Awake()");
        InitConfig();
        On.RoR2.UI.ItemInventoryDisplay.UpdateDisplay += ItemInventoryDisplay_UpdateDisplay;
    }

    public void OnDestroy()
    {
        Debug.Log("[SelectForScrap] SelectForScrapPlugin.OnDestroy()");
        On.RoR2.UI.ItemInventoryDisplay.UpdateDisplay -= ItemInventoryDisplay_UpdateDisplay;
    }

    private void ItemInventoryDisplay_UpdateDisplay(On.RoR2.UI.ItemInventoryDisplay.orig_UpdateDisplay orig, RoR2.UI.ItemInventoryDisplay self)
    {
        Debug.Log("[SelectForScrap] SelectForScrapPlugin.ItemInventoryDisplay_UpdateDisplay()");

        orig(self);

        foreach (var icon in self.itemIcons)
        {
            // TODO Filter by local user?
            if (!icon || icon.itemIndex == ItemIndex.None) continue;
            if (!icon.gameObject.GetComponent<Scrappable>())
                icon.gameObject.AddComponent<Scrappable>();
        }
    }

    private void InitConfig()
    {
        // Init config
        ModSettingsManager.SetModDescription("Allows you to pre-select items from your inventory you want to automatically scrap the next time you use a scrapper.");
        //ModSettingsManager.SetModIcon(null);

        // Key bind config
        var select_item_key = Config.Bind(
            "Input",
            "Select item",
            new KeyboardShortcut(KeyCode.S, modifiers: KeyCode.LeftShift),
            "Key to select an item to scrap"
        );
        ModSettingsManager.AddOption(new KeyBindOption(select_item_key));

        // Client config

        // Server config
    }
}
