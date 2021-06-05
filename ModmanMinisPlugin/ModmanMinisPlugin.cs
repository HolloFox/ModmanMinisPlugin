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

    [BepInPlugin(Guid, "ImageToPlane", Version)]
    [BepInDependency(CustomMiniPlugin.Guid)]
    public class ModmanMinisPlugin: BaseUnityPlugin
    {
        // constants
        private const string Guid = "org.hollofox.plugins.ModmanMinisPlugin";
        private const string Version = "1.0.0.0";

        // Configuration
        private ConfigEntry<KeyboardShortcut> triggerKeyBasic { get; set; }

        // Speech font name
        private bool properInitialization = false;

        /// <summary>
        /// Awake plugin
        /// </summary>
        void Awake()
        {
            Debug.Log("HolloFoxes Modman Minis Plugin Active.");

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
            if (isBoardLoaded())
            {
                if (properInitialization && Extensions.StrictKeyCheck(triggerKeyBasic.Value))
                {
                    
                }
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
