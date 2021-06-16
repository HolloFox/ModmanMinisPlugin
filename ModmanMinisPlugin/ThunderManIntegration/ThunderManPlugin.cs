using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using Bounce.Unmanaged;
using LordAshes;
using Newtonsoft.Json;
using RadialUI;
using ThunderMan.Manifests;
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

            /*
            RadialUIPlugin.AddOnCharacter(Guid + "SetAuras",
                new MapMenu.ItemArgs
                {
                    Title = "Set Auras",
                    CloseMenuOnActivate = true,
                    Action = AddAura,
                    Icon = sprite("Aura.png")
                },
                IsInGmMode
            );
            */

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

        private static Sprite sprite(string FileName)
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Texture2D tex = new Texture2D(32, 32);
            tex.LoadImage(File.ReadAllBytes(dir + "\\"  + FileName));
            return Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
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

        public static ParallelQuery<FileInfo> GetAuras()
        {
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pluginsFolder = Directory.GetParent(assemblyFolder).FullName;
            Debug.Log($"Assembly: {assemblyFolder}");
            Debug.Log($"Plugins: {pluginsFolder}");

            DirectoryInfo dInfo = new DirectoryInfo(pluginsFolder);
            var subdirs = dInfo.GetDirectories()
                .AsParallel();
            var i = new List<FileInfo>();
            foreach (var subdir in subdirs.Where(s => File.Exists(s.FullName + "\\assets.json")))
            {
                var obj = JsonConvert.DeserializeObject<assetManifest>(File.ReadAllText(subdir.FullName + "\\assets.json"));
                i.AddRange(obj.Auras.Select(m => new FileInfo(subdir.FullName + "\\" + m)));
            }
            return i.AsParallel();
        }

        public static ParallelQuery<FileInfo> GetMinis()
        {
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pluginsFolder = Directory.GetParent(assemblyFolder).FullName;
            Debug.Log($"Assembly: {assemblyFolder}");
            Debug.Log($"Plugins: {pluginsFolder}");

            DirectoryInfo dInfo = new DirectoryInfo(pluginsFolder);
            var subdirs = dInfo.GetDirectories()
                .AsParallel();
            var i = new List<FileInfo>();
            foreach (var subdir in subdirs.Where(s => File.Exists(s.FullName + "\\assets.json")))
            {
                var obj = JsonConvert.DeserializeObject<assetManifest>(File.ReadAllText(subdir.FullName + "\\assets.json"));
                i.AddRange(obj.Minis.Select(m => new FileInfo(subdir.FullName + "\\" + m)));
            }
            return i.AsParallel();
        }
    }
}
