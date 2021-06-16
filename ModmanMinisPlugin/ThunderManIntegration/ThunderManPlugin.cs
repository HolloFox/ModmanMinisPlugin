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

    [BepInPlugin(Guid, "Hollo Foxes' ThunderMan Plug-In", Version)]
    [BepInDependency("org.lordashes.plugins.custommini")]
    [BepInDependency(StatMessaging.Guid)]
    [BepInDependency(RadialUIPlugin.Guid)]
    public class ThunderManPlugin: BaseUnityPlugin
    {
        // constants
        public const string Guid = "org.hollofox.plugins.ThunderManPlugin";
        private const string Version = "1.1.0.0";

        // Configuration
        private ConfigEntry<KeyboardShortcut>[] triggerKeyBasic = new ConfigEntry<KeyboardShortcut>[2];

        // 
        private AssetsList list;
        private AssetsList effectsList;

        // My StatHandler
        private static bool ready = false;

        /// <summary>
        /// Awake plugin
        /// </summary>
        void Awake()
        {
            Debug.Log("HolloFox's Modman Minis Plugin is Active.");

            triggerKeyBasic[0] = Config.Bind("Hotkeys", "Transform Mini", new KeyboardShortcut(KeyCode.Alpha1, KeyCode.LeftControl));
            triggerKeyBasic[1] = Config.Bind("Hotkeys", "Apply Aura", new KeyboardShortcut(KeyCode.Alpha2, KeyCode.LeftControl));

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

            RadialUIPlugin.AddOnCharacter(Guid + "ChangeMini",
                new MapMenu.ItemArgs
                {
                    Title = "Set Group",
                    CloseMenuOnActivate = true,
                    Action = ChangeMini
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

            RadialUIPlugin.AddOnCharacter(Guid + "RemoveAuras",
                new MapMenu.ItemArgs
                {
                    Title = "Remove Auras",
                    CloseMenuOnActivate = true,
                    Action = RemoveAura
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

        }

        public void ChangeMini(MapMenuItem mmi, object o)
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

        private bool IsInGmMode(NGuid selected, NGuid targeted)
        {
            RadialTargetedMini = targeted;
            return LocalClient.IsInGmMode;
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
