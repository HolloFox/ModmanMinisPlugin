using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using LordAshes;
using Newtonsoft.Json;
using ThunderMan.Manifests;
using UnityEngine;

namespace ThunderMan.ThunderManIntegration
{
    public partial class AssetsList : Form
    {
        private string AssetType;
        public AssetsList(string search)
        {
            AssetType = search;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var model = paths[listBox1.SelectedIndex];

            var modFolder = model.transformName.Substring(0, model.transformName.IndexOf("\\"));
            
            modManifest obj = JsonConvert.DeserializeObject<modManifest>(File.ReadAllText(pluginsFolder+"\\" + modFolder + "\\manifest.json"));

            // Don't bother loading json file until needed
            var author = modFolder.Substring(0,modFolder.IndexOf("-"));
            var mod_name = obj.name;
            var version = obj.version_number;
            
            model.Ror2mm = $"ror2mm://v1/install/talespire.thunderstore.io/{author}/{mod_name}/{version}/";
            /* GET	https://talespire.thunderstore.io/api/experimental/package/{Author}/{PluginName}/ => 
                latest.version_number
            */
            // var message = $"<size=0>{mod_name}/{author}</size>{model.MiniName}";
            if (AssetType == "Effects") model.transformName = $"#{model.transformName}";
            StatMessaging.SetInfo(LocalClient.SelectedCreatureId, ThunderManPlugin.Guid, JsonConvert.SerializeObject(model));

            // CreatureManager.SetCreatureName(LocalClient.SelectedCreatureId, $"Make me a: {message}");
        }

        public List<LoadAsset> paths = new List<LoadAsset>();
        private string pluginsFolder;

        private void AssetsList_Load(object sender, EventArgs e)
        {
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            pluginsFolder = Directory.GetParent(assemblyFolder).FullName;

            var bundles = ThunderManPlugin.GetAssetPaths(AssetType);

            Dictionary<string, LoadAsset> assets = new Dictionary<string, LoadAsset>();

            foreach (var path in bundles)
            {
                var relative = path.FullName.Replace(pluginsFolder+"\\", "");
                Debug.Log($"Relative: {relative}");
                Debug.Log($"Name: {path.Name}");

                var asset = new LoadAsset
                {
                    MiniName = path.Name,
                    transformName = relative
                };
                assets.Add(path.Name,asset);
                paths.Add(asset);
                listBox1.Items.Add(path.Name);
            }

            // List<String> paths = assets.Keys.ToList().or;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = listBox1.SelectedIndex;
            var path = paths[index];
            if (Directory.Exists(pluginsFolder + "/icon.png"))
            {
                var icon = pluginsFolder + "/icon.png";
                Debug.Log(icon);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
