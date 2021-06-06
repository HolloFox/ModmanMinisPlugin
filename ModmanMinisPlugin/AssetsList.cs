using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bounce.Unmanaged;
using LordAshes;
using Newtonsoft.Json;
using UnityEngine;

namespace ModmanMinis
{
    public partial class AssetsList : Form
    {
        public AssetsList()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var model = paths[listBox1.SelectedIndex];

            var modFolder = model.transformName.Substring(0, model.transformName.IndexOf("\\"));
            
            manifest obj = JsonConvert.DeserializeObject<manifest>(File.ReadAllText(pluginsFolder+"\\" + modFolder + "\\manifest.json"));

            // Don't bother loading json file until needed
            var author = modFolder.Substring(0,modFolder.IndexOf("-"));
            var mod_name = obj.name;
            var version = obj.version_number;
            model.Ror2mm = $"ror2mm://v1/install/talespire.thunderstore.io/{author}/{mod_name}/{version}/";

            var message = JsonConvert.SerializeObject(model);
            Debug.Log(message);
            CreatureManager.SetCreatureName(LocalClient.SelectedCreatureId, $"Make me a: {message}");
        }

        public List<LoadAsset> paths = new List<LoadAsset>();
        private string pluginsFolder;

        private void AssetsList_Load(object sender, EventArgs e)
        {
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            pluginsFolder = Directory.GetParent(assemblyFolder).FullName;

            var bundles = ModmanMinisPlugin.GetAssetPaths();
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
                paths.Add(asset);
                listBox1.Items.Add(path.Name);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = listBox1.SelectedIndex;
            var path = paths[index];
            if (Directory.Exists(pluginsFolder + "/icon.png"))
            {
                var icon = pluginsFolder + "/icon.png";
                Debug.Log(icon);
                pictureBox1.ImageLocation = icon;
            }
        }
    }
}
