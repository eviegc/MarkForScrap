using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace MarkForScrap.Utils
{
    public static class PatchedKeyboardShortcut
    {
        public static bool IsDown(KeyboardShortcut shortcut)
        {
            // The original version of this function required that the key combination was
            // the ONLY set of keys pressed. Since we required you to be pressing tab
            // (scoreboard), this already meant you had to put tab in to your keybinding.
            // Worse still, this also meant you couldn't perform any other action while
            // pressing this keybinding, like *moving*.
            // This patcher method copies from KeyboardShortcut.IsDown() and modifies the
            // offending elements. Now, it only requires the bound keys to be pressed,
            // ignoring whether other keys are pressed in addition.

            KeyCode mainKey = shortcut.MainKey;
            if (mainKey == KeyCode.None)
            {
                return false;
            }

            if (UnityInput.Current.GetKeyDown(mainKey))
            {
                foreach (KeyCode code in shortcut.Modifiers)
                {
                    if (code != KeyCode.None && !UnityInput.Current.GetKey(code))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
