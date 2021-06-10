﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using Bounce.Unmanaged;
using LordAshes;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;

namespace ModmanMinis
{

    [BepInPlugin(Guid, "ThunderMan Plugin", Version)]
    [BepInDependency("org.lordashes.plugins.custommini")]
    [BepInDependency(StatMessaging.Guid)]
    public class ThunderManPlugin: BaseUnityPlugin
    {
        // constants
        public const string Guid = "org.hollofox.plugins.ThunderManPlugin";
        private const string Version = "1.0.0.0";

        // Configuration
        private ConfigEntry<KeyboardShortcut>[] triggerKeyBasic = new ConfigEntry<KeyboardShortcut>[2];

        // 
        private AssetsList list;
        private AssetsList effectsList;
        
        // My StatHandler


        /// <summary>
        /// Awake plugin
        /// </summary>
        void Awake()
        {
            Debug.Log("HolloFox's Modman Minis Plugin is Active.");

            triggerKeyBasic[0] = Config.Bind("Hotkeys", "Transform Mini", new KeyboardShortcut(KeyCode.Alpha1, KeyCode.LeftControl));
            triggerKeyBasic[1] = Config.Bind("Hotkeys", "Apply Aura", new KeyboardShortcut(KeyCode.Alpha2, KeyCode.LeftControl));

            ModdingTales.ModdingUtils.Initialize(this, Logger);

            StatMessaging.Subscribe(Guid, Request);
        }

        public void Request(StatMessaging.Change[] changes)
        {
            // Process all changes
            foreach (StatMessaging.Change change in changes)
            {
                // Find a reference to the indicated mini
                CreaturePresenter.TryGetAsset(change.cid, out var asset);
                if (asset != null)
                {
                    ModmanStatHandler.LoadCustomContent(asset, change.value);
                }
            }
        }

        private bool isBoardLoaded()
        {
            return (BoardSessionManager.HasInstance &&
                    BoardSessionManager.HasBoardAndIsInNominalState &&
                    !BoardSessionManager.IsLoading);
        }

        /// <summary>
        /// Looping method run by plugin
        /// </summary>
        void Update()
        {
            if (isBoardLoaded())
            {
                if (Extensions.StrictKeyCheck(triggerKeyBasic[0].Value))
                {
                    Debug.Log("Minis Called");
                    if (list == null || list.IsDisposed) list = new AssetsList("Minis");
                    list.Show();
                } else if (Extensions.StrictKeyCheck(triggerKeyBasic[1].Value))
                {
                    Debug.Log("Effects Called");
                    if (effectsList == null || effectsList.IsDisposed) effectsList = new AssetsList("Effects");
                    effectsList.Show();
                }
            }
        }

        public static ParallelQuery<FileInfo> GetAssetPaths(string assetType = "Minis")
        {
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pluginsFolder = Directory.GetParent(assemblyFolder).FullName;
            Debug.Log($"Assembly: {assemblyFolder}");
            Debug.Log($"Plugins: {pluginsFolder}");

            DirectoryInfo dInfo = new DirectoryInfo(pluginsFolder);
            var subdirs = dInfo.GetDirectories()
                .AsParallel()
                .EnumerateFiles(".mini");
            return subdirs;
        }
    }
}
