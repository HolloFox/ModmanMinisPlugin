using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using Bounce.Unmanaged;
using LordAshes;
using RadialUI;
using UnityEngine;

namespace ThunderMan.ThunderManIntegration
{

    [BepInPlugin(Guid, Name, Version)]
    [BepInDependency(CustomMiniPlugin.Guid)]
    [BepInDependency(StatMessaging.Guid)]
    [BepInDependency(RadialUIPlugin.Guid)]
    public class ThunderManPlugin: BaseUnityPlugin
    {
        // constants
        public const string Guid = "org.hollofox.plugins.ThunderManPlugin";
        private const string Version = "1.1.0.0";
        private const string Name = "HolloFoxes' ThunderMan Plug-In";

        // Need to remove these and use SystemMessageExtensions
        private AssetsList list;
        private AssetsList effectsList;

        // My StatHandler
        private static bool ready = false;

        /// <summary>
        /// Awake plugin
        /// </summary>
        void Awake()
        {
            Debug.Log($"{Name} is Active.");
            ModdingUtils.Initialize(this, Logger);

            StatMessaging.Subscribe(Guid, Request);

            BoardSessionManager.OnStateChange += (s) =>
            {
                if (s.ToString().Contains("+Active"))
                {
                    ready = true;
                    Debug.Log("Stat Messaging started looking for messages.");
                }
                else
                {
                    ready = false;
                    StatMessaging.Reset();
                    Debug.Log("Stat Messaging stopped looking for messages.");
                }
            };

            /* Start */
            RadialUIPlugin.AddOnCharacter(Guid + "RemoveAuras",
                new MapMenu.ItemArgs
                {
                    Title = "Remove Auras",
                    CloseMenuOnActivate = true,
                    Action = RemoveAura
                },
                IsInGmMode
            );

            RadialUIPlugin.AddOnCharacter(Guid + "AddAuras",
                new MapMenu.ItemArgs
                {
                    Title = "Add Auras",
                    CloseMenuOnActivate = true,
                    Action = AddAura
                },
                IsInGmMode
            );
            /* End */

            RadialUIPlugin.AddOnCharacter(Guid + "RevertMini",
                new MapMenu.ItemArgs
                {
                    Title = "Revert Mini",
                    CloseMenuOnActivate = true,
                    Action = RevertMini
                },
                HasChanged
            );

            RadialUIPlugin.AddOnCharacter(Guid + "ChangeMini",
                new MapMenu.ItemArgs
                {
                    Title = "Change Mini",
                    CloseMenuOnActivate = true,
                    Action = ChangeMini
                },
                IsInGmMode
                );
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

        /// <summary>
        /// Looping method run by plugin
        /// </summary>
        void Update()
        {

        }

        public void ChangeMini(MapMenuItem mmi, object o)
        {
            Debug.Log("Minis Called");
            if (list == null || list.IsDisposed) list = new AssetsList("Minis");
            list.Show();
        }

        public void RevertMini(MapMenuItem mmi, object o)
        {
            Debug.Log("Minis Called");
            if (list == null || list.IsDisposed) list = new AssetsList("Minis");
            list.Show();
        }

        public void AddAura(MapMenuItem mmi, object o)
        {
            Debug.Log("Effects Called");
            if (effectsList == null || effectsList.IsDisposed) effectsList = new AssetsList("Effects");
            effectsList.Show();
        }

        public void RemoveAura(MapMenuItem mmi, object o)
        {
            Debug.Log("Effects Called");
            if (effectsList == null || effectsList.IsDisposed) effectsList = new AssetsList("Effects");
            effectsList.Show();
        }

        public static NGuid RadialTargetedMini;

        private static bool IsInGmMode(NGuid selected, NGuid targeted)
        {
            RadialTargetedMini = targeted;
            return LocalClient.IsInGmMode;
        }

        private static bool HasChanged(NGuid selected, NGuid targeted)
        {
            RadialTargetedMini = targeted;
            return false; // LocalClient.IsInGmMode;
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
