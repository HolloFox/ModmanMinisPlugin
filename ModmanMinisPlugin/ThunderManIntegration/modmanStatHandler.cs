using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;
using static ThunderMan.CustomMinisPlugin.rCMiniPlugin;

namespace ThunderMan.ThunderManIntegration
{
    public static class ModmanStatHandler
    {

        public static void LoadCustomContent(CreatureBoardAsset asset, string source)
        { 
            Debug.Log("Received ror2mm Message:");
            Debug.Log(source);
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pluginsFolder = Directory.GetParent(assemblyFolder).FullName;
            var data = JsonConvert.DeserializeObject<LoadAsset>(source);
            Debug.Log($"data: {data}");
            var folder = data.transformName.Substring(0, data.transformName.IndexOf("\\"));
            if (Directory.Exists(pluginsFolder + "\\" + folder))
            {
                ModRequestHandler.LoadCustomContent(asset, pluginsFolder + "\\" + data.transformName);
            }
            else
            {
                SystemMessage.SendSystemMessage("Download Asset",
                    $"Would you like to download {data.MiniName} for TaleSpire?",
                    $"Download {data.MiniName}",
                    () =>
                    {
                        System.Diagnostics.Process.Start(data.Ror2mm).WaitForExit();
                        ModRequestHandler.LoadCustomContent(asset, pluginsFolder + "\\" + data.transformName);
                    }, "Don't Download");
            }
        }
    }
}
