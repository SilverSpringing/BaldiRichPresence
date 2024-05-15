using UnityEngine.SceneManagement;
using UnityEngine;

namespace BaldiRPC.Updater
{
    public class ActivityUpdater
    {
        public static void UpdateStats()
        {
            if (SceneManager.GetActiveScene().name == "Warnings")
            {
                BaldiRPC.BB_Activity.Details = "Warning Screen";
                BaldiRPC.BB_Activity.State = "";
            }
            else if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                BaldiRPC.BB_Activity.Details = "Main Menu";

                //Name Entry
                if (GameObject.Find("NameEntry").activeSelf)
                {
                    BaldiRPC.BB_Activity.State = "File Select";
                } else
                {
                    BaldiRPC.BB_Activity.State = "";
                }
            }
            else if (SceneManager.GetActiveScene().name == "Game")
            {
                BaldiRPC.BB_Activity.Details = "";
                BaldiRPC.BB_Activity.State = "";
            }
        }
    }
}