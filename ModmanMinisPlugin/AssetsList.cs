using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bounce.Unmanaged;
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
            ChatManager.SendChatMessage("<size=0> ! " + LocalClient.SelectedCreatureId + " </size> Make me a ", (NGuid)LocalClient.SelectedCreatureId.Value);
        }

        public List<FileInfo> paths = new List<FileInfo>();

        private void AssetsList_Load(object sender, EventArgs e)
        {
            var bundles = ModmanMinisPlugin.GetAssetPaths();
            foreach (var path in bundles)
            {
                paths.Add(path);
                listBox1.Items.Add(path.Name);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = listBox1.SelectedIndex;
            var path = paths[index];
            if (path.Exists)
            {
                var grand = Directory.GetParent(Directory.GetParent(path.FullName).FullName);
                var icon = grand.FullName + "/icon.png";
                Debug.Log(icon);
                pictureBox1.ImageLocation = icon;
            }
        }
    }
}
