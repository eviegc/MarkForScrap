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

        // Attach our scrap counter to the CharacterMaster's inventory as it wakes up
        CharacterMaster.onStartGlobal += OnCharacterMasterStart;

        // Make sure we mark the inventory list in the top-bar for later use
        On.RoR2.UI.HUD.Awake += HUD_Awake;

        // Since we can mark the target inventory, we can check on every ItemIcon creation
        // whether we're in the top bar, and attach our components if we are.
        On.RoR2.UI.ItemIcon.Awake += ItemIcon_Awake;
    }

    public void OnDestroy()
    {
        Debug.Log("[SelectForScrap] SelectForScrapPlugin.OnDestroy()");

        On.RoR2.UI.HUD.Awake -= HUD_Awake;
        On.RoR2.UI.ItemIcon.Awake -= ItemIcon_Awake;
    }

    private void OnCharacterMasterStart(CharacterMaster master)
    {
        // Debug.Log("[SelectForScrap] SelectForScrapPlugin.OnCharacterMasterStart()");

        if (!Utils.LocalUser.IsLocal(master)) return;

        // Debug.Log("[SelectForScrap] SelectForScrapPlugin.OnCharacterMasterStart() | Got Local Player");

        var inv = master.inventory;
        if (!inv)
        {
            // Debug.Log("[SelectForScrap] SelectForScrapPlugin.OnCharacterMasterStart() | No Inventory Found");
            return;
        }

        if (!inv.gameObject.GetComponent<ScrapCounter>())
        {
            Debug.Log("[SelectForScrap] SelectForScrapPlugin.OnCharacterMasterStart() | Adding ScrapCounter");
            inv.gameObject.AddComponent<ScrapCounter>();
        }
    }

    private void HUD_Awake(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
    {
        Debug.Log("[SelectForScrap] SelectForScrapPlugin.HUD_Awake()");

        orig(self);

        var invDisplay = self.itemInventoryDisplay;

        if (!invDisplay.gameObject.GetComponent<MainInventoryMarker>())
        {
            invDisplay.gameObject.AddComponent<MainInventoryMarker>();
        }
    }

    private void ItemIcon_Awake(On.RoR2.UI.ItemIcon.orig_Awake orig, RoR2.UI.ItemIcon self)
    {
        // Debug.Log("[SelectForScrap] SelectForScrapPlugin.ItemIcon_Awake()");

        orig(self);

        if (!self.GetComponentInParent<MainInventoryMarker>()) return;

        Debug.Log("[SelectForScrap] SelectForScrapPlugin.ItemIcon_Awake() | Got ItemIcon the main inventory");

        if (self.GetComponent<Scrappable>()) return;

        self.gameObject.AddComponent<Scrappable>();
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

    // Used only so we can find the top-bar inventory later on
    private sealed class MainInventoryMarker : MonoBehaviour { }
}
