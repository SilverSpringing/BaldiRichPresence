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
}