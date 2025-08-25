using System.Security.Permissions;
using BepInEx;
using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using UnityEngine;
using RoR2;
using UnityEngine.Networking;

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

        // Make sure we mark the inventory list in the top-bar for later use
        On.RoR2.UI.HUD.Awake += HUD_Awake;

        // Since we can mark the target inventory, we can check on every ItemIcon creation
        // whether we're in the top bar, and attach our components if we are.
        On.RoR2.UI.ItemIcon.Awake += ItemIcon_Awake;

        // Attach our scrap counter to the NetworkUser so it syncs properly
        On.RoR2.NetworkUser.Start += NetworkUser_Start;

        // Intercept scrapper logic to pull from our own list
        On.RoR2.ScrapperController.AssignPotentialInteractor += ScrapperController_AssignPotentialInteractor;
    }

    public void OnDestroy()
    {
        Debug.Log("[SelectForScrap] SelectForScrapPlugin.OnDestroy()");

        On.RoR2.UI.HUD.Awake -= HUD_Awake;
        On.RoR2.UI.ItemIcon.Awake -= ItemIcon_Awake;
        On.RoR2.NetworkUser.Start -= NetworkUser_Start;
        On.RoR2.ScrapperController.AssignPotentialInteractor -= ScrapperController_AssignPotentialInteractor;
    }

    private void ScrapperController_AssignPotentialInteractor(On.RoR2.ScrapperController.orig_AssignPotentialInteractor orig, ScrapperController self, Interactor activator)
    {
        orig(self, activator);
        if (!NetworkServer.active) return;

        Debug.Log("[SelectForScrap] SelectForScrapPlugin.ScrapperController_AssignPotentialInteractor()");

        var characterBody = activator.GetComponent<CharacterBody>();
        var networkUser = characterBody ? Util.LookUpBodyNetworkUser(characterBody) : null;
        var scrapCounter = networkUser?.GetComponent<InventoryScrapCounter>();
        if (!(scrapCounter && scrapCounter.HasItemsToScrap())) return;

        ItemIndex itemToScrap = scrapCounter.Take();
        Debug.Log($"[SelectForScrap] SelectForScrapPlugin.ScrapperController_AssignPotentialInteractor() | Took {itemToScrap}");

        var pickup = PickupCatalog.FindPickupIndex(itemToScrap);
        var ctrl = self.GetComponent<ScrapperController>();

        ctrl.BeginScrapping(pickup.value);
    }

    private void NetworkUser_Start(On.RoR2.NetworkUser.orig_Start orig, NetworkUser self)
    {
        orig(self);

        Debug.Log("[SelectForScrap] SelectForScrapPlugin.NetworkUser_Start()");

        if (!self.GetComponent<InventoryScrapCounter>())
        {
            Debug.Log("[SelectForScrap] SelectForScrapPlugin.NetworkUser_Start() | Adding InventoryScrapCounter");
            self.gameObject.AddComponent<InventoryScrapCounter>();
        }
    }

    private void HUD_Awake(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
    {
        orig(self);
        if (!NetworkClient.active) return;

        Debug.Log("[SelectForScrap] SelectForScrapPlugin.HUD_Awake()");

        var invDisplay = self.itemInventoryDisplay;

        if (!invDisplay.gameObject.GetComponent<MainInventoryMarker>())
        {
            invDisplay.gameObject.AddComponent<MainInventoryMarker>();
        }
    }

    private void ItemIcon_Awake(On.RoR2.UI.ItemIcon.orig_Awake orig, RoR2.UI.ItemIcon self)
    {
        orig(self);
        if (!NetworkClient.active) return;

        // Debug.Log("[SelectForScrap] SelectForScrapPlugin.ItemIcon_Awake()");

        if (!self.GetComponentInParent<MainInventoryMarker>()) return;

        Debug.Log("[SelectForScrap] SelectForScrapPlugin.ItemIcon_Awake() | Got ItemIcon the main inventory");

        if (self.GetComponent<ItemIconScrapSelector>()) return;

        self.gameObject.AddComponent<ItemIconScrapSelector>();
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
