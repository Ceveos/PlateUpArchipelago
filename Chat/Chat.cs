using Controllers;
using HarmonyLib;
using Kitchen;
using Kitchen.Modules;
using KitchenSmartNoClip;
using UnityEngine.InputSystem;

namespace KitchenArchipelago.Chat
{
    public static class Chat
    {
        public static bool show = false;
        public const string ChatInputActionName = "ArchipelagoChat"; // Not quite sure how efficient this is saved by plateup itself. So dont change this string under any circumstances for this mod. (No errors when this mod is uninstalled.
        
        public static void TextInputCallback(TextInputView.TextInputState state, string value)
        {
            KitchenArchipelago.Logger.LogInfo("TEXT CALLBACK: " + value);
        }

        public static void CheckIfShouldPerformTextInput(InputAction.CallbackContext obj)
        {
            show = TextInputView.Main.State != TextInputView.TextInputState.EnteringText;
        }
        public static void PerformTextInput(InputAction.CallbackContext obj)
        {
            if (show)
            {
                TextInputView.RequestTextInput("Chat", "", 15, TextInputCallback);
            }
        }
    }

    [HarmonyPatch(typeof(Maps), nameof(Maps.NewGamepad))]
    public class InputActionsPatch_Gamepad
    {

        [HarmonyPostfix]
        public static void Actions_AddChatAction_Keyboard(ref InputActionMap __result)
        {
            var action = __result.AddAction(Chat.ChatInputActionName, InputActionType.Button);
            action.AddBinding("<Gamepad>/select");
            action.performed += Chat.CheckIfShouldPerformTextInput;
            action.canceled += Chat.PerformTextInput;
        }
    }

    [HarmonyPatch(typeof(Maps), nameof(Maps.NewKeyboard))]
    public class InputActionsPatch_Keyboard
    {

        [HarmonyPostfix]
        public static void Actions_AddChatAction_Keyboard(ref InputActionMap __result)
        {
            var action = __result.AddAction(Chat.ChatInputActionName, InputActionType.Button);
            action.AddBinding("<Keyboard>/enter");
            action.performed += Chat.CheckIfShouldPerformTextInput;
            action.canceled += Chat.PerformTextInput;
        }
    }

    /// <summary>
    /// Adds the chat rebind menu
    /// </summary>
    [HarmonyPatch(typeof(ControlRebindElement), nameof(ControlRebindElement.Setup))]
    public class InputActionsPatch_Rebind
    {
        [HarmonyPostfix]
        public static void Rebind_AddRebind(ControlRebindElement __instance, PanelElement ___Panel, ModuleList ___ModuleList)
        {
            SmartNoClip.LogWarning("Localization error is from mod Archipelago. It is not easily avoidable, but it also doesn't cause any problems.");
            __instance.AddRebindOption("Chat", Chat.ChatInputActionName);
            ___Panel.SetTarget(___ModuleList);
        }
    }
}
