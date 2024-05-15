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
        public static BaldiRPC plugin;

        internal static class ModInfo
        {
            internal const string GUID = "silverspringing.baldiplus.baldidiscordstats";
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
            CustomOptionsCore.OnMenuInitialize += AddOptions;

            RichPresenceEnabled = plugin.Config.Bind("DiscordRPC", "enabled", true, "If enabled, stats about your Baldi's Basics Plus gameplay will appear on your Discord profile.");

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
            BB_Activity.Assets.LargeImage = "icon_placeholder";
            BB_Activity.Details = "I'm just testing rich presence";
            BB_Activity.State = "mod's not done yet sorry";
            UpdateActivity();

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

        public void AddOptions(OptionsMenu __instance)
        {
            GameObject customHeader = CustomOptionsCore.CreateNewCategory(__instance, "Discord RPC");

            MenuToggle RPCToggle = CustomOptionsCore.CreateToggleButton(__instance, new Vector2(50f, 20f), "Enabled", RichPresenceEnabled.Value, "If enabled, stats about your Baldi's Basics Plus\ngameplay will appear on your <color=#5865F2>Discord profile</color>.");
            RPCToggle.GetComponentInChildren<StandardMenuButton>().OnPress.AddListener(() =>
            {
                RichPresenceEnabled.Value = !RichPresenceEnabled.Value;
            });

            RPCToggle.transform.SetParent(customHeader.transform, false);
        }

        private static void UpdateActivity()
        {
            if (BB_ActivityManager != null && RichPresenceEnabled.Value)
            {
            }
            try
            {
                BB_ActivityManager.UpdateActivity(BB_Activity, result => { });
            }
            catch (Exception e) { UnityEngine.Debug.LogError("Discord::UpdateActivity throws a " + e.GetType() + ":\n" + e.Message); }
        }
    }
}
