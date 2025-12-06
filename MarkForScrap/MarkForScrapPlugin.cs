using System.Security.Permissions;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace MarkForScrap;

[BepInDependency("com.rune580.riskofoptions")]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class MarkForScrapPlugin : BaseUnityPlugin
{
    internal static ManualLogSource Log;

    public void Awake()
    {
        if (PluginConfig.DebugLogs.Value)
            Logger.LogDebug("MarkForScrapPlugin.Awake()");

        Log = Logger;
        Resources.Assets.LoadAssets();
        InitConfig();

        // Make sure we mark the inventory list in the top-bar for later use
        On.RoR2.UI.HUD.Awake += HUD_Awake;

        // Since we can mark the target inventory, we can check on every ItemIcon creation
        // whether we're in the top bar, and attach our components if we are.
        On.RoR2.UI.ItemIcon.Awake += ItemIcon_Awake;

        // Attach our scrap counter to the NetworkUser so it syncs properly
        On.RoR2.NetworkUser.OnEnable += NetworkUser_OnEnable;

        // Intercept scrapper logic to pull from our own list
        On.RoR2.Interactor.PerformInteraction += Interactor_PerformInteraction;
    }

    public void OnDestroy()
    {
        if (PluginConfig.DebugLogs.Value)
            Logger.LogDebug("MarkForScrapPlugin.OnDestroy()");

        On.RoR2.UI.HUD.Awake -= HUD_Awake;
        On.RoR2.UI.ItemIcon.Awake -= ItemIcon_Awake;
        On.RoR2.NetworkUser.OnEnable -= NetworkUser_OnEnable;
        On.RoR2.Interactor.PerformInteraction -= Interactor_PerformInteraction;
    }

    private void Interactor_PerformInteraction(
        On.RoR2.Interactor.orig_PerformInteraction orig,
        Interactor activator,
        GameObject interactableObject
    )
    {
        if (!NetworkServer.active)
        {
            orig(activator, interactableObject);
            return;
        }

        if (PluginConfig.DebugLogs.Value)
            Logger.LogDebug("MarkForScrapPlugin.Interactor_PerformInteraction()");

        var controller = interactableObject.GetComponentInParent<ScrapperController>();
        if (!controller)
        {
            orig(activator, interactableObject);
            return;
        }

        var characterBody = activator.GetComponent<CharacterBody>();
        var networkUser = characterBody ? Util.LookUpBodyNetworkUser(characterBody) : null;
        var scrapCounter = networkUser?.GetComponent<InventoryScrapCounter>();
        if (!(scrapCounter && scrapCounter.HasItemsToScrap()))
        {
            orig(activator, interactableObject);
            return;
        }

        ItemIndex itemToScrap = scrapCounter.Take();
        if (PluginConfig.DebugLogs.Value)
            Logger.LogDebug(
                $"MarkForScrapPlugin.Interactor_PerformInteraction() | Took {itemToScrap}"
            );

        var pickup = PickupCatalog.FindPickupIndex(itemToScrap);
        controller.AssignPotentialInteractor(activator);
        controller.BeginScrapping(pickup.value);
    }

    private void NetworkUser_OnEnable(On.RoR2.NetworkUser.orig_OnEnable orig, NetworkUser self)
    {
        orig(self);

        if (PluginConfig.DebugLogs.Value)
            Logger.LogDebug("MarkForScrapPlugin.NetworkUser_OnEnable()");

        if (!self.GetComponent<InventoryScrapCounter>())
        {
            if (PluginConfig.DebugLogs.Value)
                Logger.LogDebug(
                    "MarkForScrapPlugin.NetworkUser_OnEnable() | Adding InventoryScrapCounter"
                );
            self.gameObject.AddComponent<InventoryScrapCounter>();
        }
    }

    private void HUD_Awake(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
    {
        orig(self);
        if (!NetworkClient.active)
            return;

        if (PluginConfig.DebugLogs.Value)
            Logger.LogDebug("MarkForScrapPlugin.HUD_Awake()");

        var invDisplay = self.itemInventoryDisplay;

        if (!invDisplay.gameObject.GetComponent<MainInventoryMarker>())
        {
            invDisplay.gameObject.AddComponent<MainInventoryMarker>();
        }
    }

    private void ItemIcon_Awake(On.RoR2.UI.ItemIcon.orig_Awake orig, RoR2.UI.ItemIcon self)
    {
        orig(self);
        if (!NetworkClient.active)
            return;

        if (PluginConfig.DebugLogs.Value)
            Logger.LogDebug("MarkForScrapPlugin.ItemIcon_Awake()");

        if (!self.GetComponentInParent<MainInventoryMarker>())
            return;

        if (PluginConfig.DebugLogs.Value)
            Logger.LogDebug(
                "MarkForScrapPlugin.ItemIcon_Awake() | Got ItemIcon the main inventory"
            );

        if (self.GetComponent<ItemIconScrapSelector>())
            return;

        self.gameObject.AddComponent<ItemIconScrapSelector>();
    }

    private void InitConfig()
    {
        // Init config
        ModSettingsManager.SetModDescription(
            "Allows you to pre-select items from your inventory you want to automatically scrap the next time you use a scrapper."
        );

        Sprite modIcon = Resources.Assets.Load<Sprite>("Assets/icon.png");
        ModSettingsManager.SetModIcon(modIcon);

        PluginConfig.ToggleScrapKey = Config.Bind(
            "Input",
            "Toggle item for scrap",
            new KeyboardShortcut(KeyCode.T),
            "When mouse hovering over an item in the top bar, pressing this key will (un)mark it for scrapping."
        );
        ModSettingsManager.AddOption(new KeyBindOption(PluginConfig.ToggleScrapKey));

        PluginConfig.DebugLogs = Config.Bind(
            "Debug",
            "Print debug logs to console",
            false,
            "When true, logs will be printed to the console"
        );
        ModSettingsManager.AddOption(new CheckBoxOption(PluginConfig.DebugLogs));
    }

    // Used only so we can find the top-bar inventory later on
    private sealed class MainInventoryMarker : MonoBehaviour { }
}
