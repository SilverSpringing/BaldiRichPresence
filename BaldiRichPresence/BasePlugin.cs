using BepInEx;
using System;
using MTM101BaldAPI.OptionsAPI;
using UnityEngine;
using HarmonyLib;
using BepInEx.Configuration;
using MTM101BaldAPI;
using System.Threading;
using Discord;


namespace BaldiRPC
{
    [BepInPlugin(ModInfo.GUID, ModInfo.Name, ModInfo.Version)]
    [BepInProcess("BALDI.exe")]
    public class BaldiRPC : BaseUnityPlugin
    {
        //DISCORD VARIABLES
        public static Discord.Discord BB_Discord = null;
        public static ActivityManager BB_ActivityManager = null;
        public static Discord.Activity BB_Activity = new Discord.Activity { Instance = true };

        public static ConfigEntry<bool> RichPresenceEnabled;
        public static ConfigEntry<int> WhichIcon;

        public static AdjustmentBars iconChangeBar;
        public static TextLocalizer iconNameText;

        public string state_string;
        public string detail_string;
        public string extra_state_string;
        public string extra_detail_string;

        public static BaldiRPC plugin;

        internal string[] iconNames = {
                "Placeholder",
                "Refined",
                "Plus"
            };

        internal static class ModInfo
        {
            internal const string GUID = "silverspringing.baldiplus.baldirichpresence";
            internal const string Name = "Baldi Discord Stats";
            internal const string Version = "1.0.0.0";
        }

        internal static class RPInfo
        {
            internal const long client_id = 1240119965185478656;
        }

        void Awake()
        {
            Harmony harmony = new Harmony(ModInfo.GUID);
            harmony.PatchAllConditionals();
            plugin = this;

            var discord = new Discord.Discord(RPInfo.client_id, (UInt64)Discord.CreateFlags.Default);
            CustomOptionsCore.OnMenuInitialize += AddRPCOptions;

            RichPresenceEnabled = plugin.Config.Bind("DiscordRPC", "enabled", true, "If enabled, stats about your Baldi's Basics Plus gameplay will appear on your Discord profile.");
            WhichIcon = plugin.Config.Bind("DiscordRPC", "which_icon", 0, "Which icon should be used for the large image of the Rich Presence visualizer. 0 = Placeholder, 1 = Refined, 2 = Plus");

            //only start if it's enabled
            if (RichPresenceEnabled.Value)
            {
                Thread BB_THREAD = new Thread(DiscordGo);
                BB_THREAD.Start();
            }
        }
        private static void DiscordGo()
        {
            BB_Discord = new Discord.Discord(1240119965185478656, (UInt64)CreateFlags.NoRequireDiscord);
            BB_ActivityManager = BB_Discord.GetActivityManager();
            if (BB_ActivityManager == null) return;
            BB_ActivityManager.RegisterSteam(1275890);
            
            //CHOOSE ICON
            if (WhichIcon.Value == 0)
            {
                BB_Activity.Assets.LargeImage = "icon_placeholder";
            } 
            else if (WhichIcon.Value == 1)
            {
                BB_Activity.Assets.LargeImage = "icon_refined";
            }
            else if (WhichIcon.Value == 2)
            {
                BB_Activity.Assets.LargeImage = "icon_plus";
            }
            else
            {
                BB_Activity.Assets.LargeImage = "icon_badsum"; //oooooh it's a secret!!!!
            }

            //set name
            BB_Activity.Assets.LargeText = Application.version;

            plugin.UpdateActivity("Warning Screen", null);

            try
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(500);  
                        BB_Discord.RunCallbacks();
                    }
                    catch (ResultException e)
                    {
                        UnityEngine.Debug.LogError("Discord throws a ResultException: " + e.Message);
                    }
                }
            }
            finally
            {
                BB_Discord.Dispose();
            }
        }

        public void AddRPCOptions(OptionsMenu __instance)
        {
            GameObject customHeader = CustomOptionsCore.CreateNewCategory(__instance, "Discord RPC");

            if (Singleton<CoreGameManager>.Instance != null)
            {
                TextLocalizer warningText = CustomOptionsCore.CreateText(__instance, new Vector2(-60f, 0f), "Discord RPC settings cannot be\nchanged during gameplay.");
                warningText.transform.SetParent(customHeader.transform, false);
            }
            else
            {
                MenuToggle RPCToggle = CustomOptionsCore.CreateToggleButton(__instance, new Vector2(50f, 50f), "Enabled", RichPresenceEnabled.Value, "If enabled, stats about your Baldi's Basics Plus\ngameplay will appear on your <color=#5865F2>Discord profile</color>.\n\n<color=red>Requires a restart to take effect!</color>");
                RPCToggle.GetComponentInChildren<StandardMenuButton>().OnPress.AddListener(() =>
                {
                    RichPresenceEnabled.Value = !RichPresenceEnabled.Value;
                });

                iconNameText = CustomOptionsCore.CreateText(__instance, new Vector2(-40f, -90f), iconNames[WhichIcon.Value]);
                iconChangeBar = CustomOptionsCore.CreateAdjustmentBar(__instance, new Vector2(50f, -60f), "Icon: " + iconNames[WhichIcon.Value], 2, "Change which icon should be used for the large\nimage of the Rich Presence visualizer.\n\n<color=red>Requires a restart to take effect!</color>", WhichIcon.Value, () =>
                {
                    WhichIcon.Value = iconChangeBar.GetRaw();
                    iconNameText.GetLocalizedText(iconNames[WhichIcon.Value]);
                });

                RPCToggle.transform.SetParent(customHeader.transform, false);
                iconChangeBar.transform.SetParent(customHeader.transform, false);
                iconNameText.transform.SetParent(customHeader.transform, false);
            }
        }

        public void UpdateActivity(string D, string S)
        {
            if (BB_ActivityManager != null && RichPresenceEnabled.Value)
            {
                if (D != null) BB_Activity.Details = D;
                if (S != null) BB_Activity.State = S;
            }
            try
            {
                BB_ActivityManager.UpdateActivity(BB_Activity, result => { });
            }
            catch (Exception e) { UnityEngine.Debug.LogError("Discord::UpdateActivity throws a " + e.GetType() + ":\n" + e.Message); }
        }
    }
}
