using System;
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

    [BepInPlugin(Guid, "ThunderManPlugin", Version)]
    [BepInDependency(CustomMiniPlugin.Guid)]
    [BepInDependency(StatMessaging.Guid)]
    public class ThunderManPlugin: BaseUnityPlugin
    {
        // constants
        public const string Guid = "org.hollofox.plugins.ThunderManPlugin";
        private const string Version = "0.9.0.0";

        // Configuration
        private ConfigEntry<KeyboardShortcut> triggerKeyBasic { get; set; }

        // 
        private AssetsList list;
        private static string dir = UnityEngine.Application.dataPath.Substring(0, UnityEngine.Application.dataPath.LastIndexOf("/")) + "/TaleSpire_CustomData/";

        // My StatHandler


        /// <summary>
        /// Awake plugin
        /// </summary>
        void Awake()
        {
            Debug.Log("HolloFox's Modman Minis Plugin is Active.");

            triggerKeyBasic = Config.Bind("Hotkeys", "Transform Mini", new KeyboardShortcut(KeyCode.Alpha1, KeyCode.LeftControl));

            var bundles = GetAssetPaths();
            if (bundles.Count() > 0) Debug.Log("Plugin Found Minis:");
            foreach (var path in bundles)
            {
                Debug.Log($"- {path.FullName}");
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
            StatMessaging.Check(Callback);
            if (isBoardLoaded())
            {
                if (Extensions.StrictKeyCheck(triggerKeyBasic.Value))
                {
                    Debug.Log("called");
                    if (list == null || list.IsDisposed) list = new AssetsList();
                    list.Show();
                }

                // var requestHandler = CustomMiniPlugin.GetRequestHander();
                
            }
        }

        public void Callback(StatMessaging.Change[] name)
        {
            Debug.Log("callback:");
            foreach (var change in name)
            {
                Debug.Log(JsonConvert.SerializeObject(change));
            }
            
        }

        public static ParallelQuery<FileInfo> GetAssetPaths()
        {
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pluginsFolder = Directory.GetParent(assemblyFolder).FullName;
            Debug.Log($"Assembly: {assemblyFolder}");
            Debug.Log($"Plugins: {pluginsFolder}");

            DirectoryInfo dInfo = new DirectoryInfo(pluginsFolder);
            var subdirs = dInfo.GetDirectories()
                .AsParallel()
                .GetSubfolders("TaleSpire_CustomData")
                .GetSubfolders("Minis")
                .EnumerateFiles();
            return subdirs;
        }
    }
}
