using BepInEx.Configuration;

namespace MarkForScrap
{
    public class PluginConfig
    {
        public static ConfigEntry<KeyboardShortcut> ToggleScrapKey;

        public static ConfigEntry<bool> DebugLogs;
    }
}
