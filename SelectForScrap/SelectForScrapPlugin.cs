using BepInEx;
using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using UnityEngine;

namespace SelectForScrap;

[BepInDependency("com.rune580.riskofoptions")]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]

public class SelectForScrapPlugin : BaseUnityPlugin
{
    public void Awake()
    {
        Debug.Log("SelectForScrap: Awake");
        InitConfig();
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
