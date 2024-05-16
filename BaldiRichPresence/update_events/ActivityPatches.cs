using HarmonyLib;

namespace BaldiRPC.ActivityPatches
{

    //INITIALIZE MAIN MENU
    [HarmonyPatch(typeof(MainMenu), "Start")]
    public class InitMainMenu
    {
        static void Postfix()
        {
            BaldiRPC.plugin.UpdateActivity("Main Menu", null);
        }
    }

    //QUIT TO MAIN MENU
    [HarmonyPatch(typeof(CoreGameManager), "Quit")]
    public class QuitToMainMenu
    {
        static void Postfix()
        {
            BaldiRPC.plugin.UpdateActivity("Main Menu", "");
        }
    }

    //INITIALIZE OPTIONS MENU
    [HarmonyPatch(typeof(OptionsMenu), "Awake")]
    public class InitOptionsMenu
    {
        static void Postfix()
        {
            if (Singleton<CoreGameManager>.Instance != null) return; //only change text if not playing game
            BaldiRPC.plugin.UpdateActivity(null, "Options");
        }
    }

    //CLOSE OPTIONS MENU
    [HarmonyPatch(typeof(OptionsMenu), "Close")]
    public class CloseOptionsMenu
    {
        static void Postfix()
        {
            if (Singleton<CoreGameManager>.Instance != null) return; //only change text if not playing game
            BaldiRPC.plugin.UpdateActivity(null, "");
        }
    }

    //FILE SELECT
    [HarmonyPatch(typeof(NameManager), "Awake")]
    public class FileSelectActivity
    {
        static void Postfix()
        {
            BaldiRPC.plugin.UpdateActivity("File Select", null);
        }
    }

    //START ELEVATOR
    [HarmonyPatch(typeof(ElevatorScreen), "Initialize")]
    class OnChangeToElevator  
    {
        static void Postfix()
        {
            BaldiRPC.plugin.UpdateActivity("Elevator", Singleton<CoreGameManager>.Instance.sceneObject.levelTitle);
        }
    }

    //COLLECT NOTEBOOK
    [HarmonyPatch(typeof(BaseGameManager), "CollectNotebooks")]
    class OnCollectNotebook
    {
        static void Postfix(BaseGameManager __instance, int count)
        {
            BaldiRPC.plugin.UpdateActivity(null, count.ToString() + "/" + __instance.NotebookTotal.ToString() + " Notebooks");
        }
    }

    //BEGI
    [HarmonyPatch(typeof(BaseGameManager), "BeginPlay")]
    class OnChangeToFloor
    {
        static void Postfix()
        {
            BaldiRPC.plugin.UpdateActivity(Singleton<CoreGameManager>.Instance.sceneObject.levelTitle, Singleton<BaseGameManager>.Instance.FoundNotebooks.ToString() + "/" + Singleton<BaseGameManager>.Instance.NotebookTotal.ToString() + " Notebooks");
        }
    }

    //PAUSE FLOOR
    [HarmonyPatch(typeof(CoreGameManager), "Pause")]
    class OnPause
    {
        static void Postfix(CoreGameManager __instance)
        {
            if (__instance.Paused)
            {
                BaldiRPC.plugin.UpdateActivity(Singleton<CoreGameManager>.Instance.sceneObject.levelTitle + " (Paused)", null);
            }
            else
            {
                BaldiRPC.plugin.UpdateActivity(Singleton<CoreGameManager>.Instance.sceneObject.levelTitle, null);
            }
        }
    }
}