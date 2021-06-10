using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dummiesman;
using LordAshes;
using Newtonsoft.Json;
using UnityEngine;

namespace ModmanMinis
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
                CustomMiniPlugin.RequestHandler.LoadCustomContent(asset, pluginsFolder + "\\" + data.transformName);
            }
            else
            {
                SystemMessage.SendSystemMessage("Download Asset",
                    $"Would you like to download {data.MiniName} for TaleSpire?",
                    $"Download {data.MiniName}",
                    () =>
                    {
                        System.Diagnostics.Process.Start(data.Ror2mm).WaitForExit();
                        CustomMiniPlugin.RequestHandler.LoadCustomContent(asset, pluginsFolder + "\\" + data.transformName);
                    }, "Don't Download");
            }

        }

        /// <summary>
        /// Method to detect character Stat3 requests 
        /// </summary>
        /*public void CheckStatRequests()
        {
            if (!transformationInProgress)
            {
                foreach (CreatureBoardAsset asset in CreaturePresenter.AllCreatureAssets)
                {
                    // Get transform namne if any
                    string transformName = (asset.Creature.Name.Contains(":")) ? asset.Creature.Name.Substring(asset.Creature.Name.IndexOf(":") + 1).Trim() : "";

                    // If creature doesn't have a current transformName add an empty one to the transform dictionary
                    if (!transformations.ContainsKey(asset.Creature.CreatureId)) { transformations.Add(asset.Creature.CreatureId, ""); }

                    // If the transform has changed
                    if (transformations[asset.Creature.CreatureId] != transformName)
                    {
                        Debug.Log("Creature '" + asset.Creature.Name + "' (" + asset.Creature.CreatureId + ") has changed from '" + transformations[asset.Creature.CreatureId] + "' to '" + transformName + "'");

                        // Prevent transformation for being applied multiple times
                        transformations[asset.Creature.CreatureId] = transformName;


                        Debug.Log("Received Message:");
                        Debug.Log(transformName);
                        if (transformName.Contains("ror2mm"))
                        {
                            Debug.Log("Received ror2mm Message:");
                            Debug.Log(transformName);
                            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                            var pluginsFolder = Directory.GetParent(assemblyFolder).FullName;

                            var data = JsonConvert.DeserializeObject<LoadAsset>(transformName);
                            Debug.Log($"data: {data}");
                            var folder = data.transformName.Substring(0, data.transformName.IndexOf("\\"));

                            if (Directory.Exists(pluginsFolder + "\\" + folder))
                            {
                                LoadCustomContent(asset, pluginsFolder + "\\" + data.transformName);
                            }
                            else
                            {
                                SystemMessage.SendSystemMessage("Download Asset",
                                    $"Would you like to download {data.MiniName} for TaleSpire?",
                                    $"Download {data.MiniName}",
                                    () =>
                                    {
                                        System.Diagnostics.Process.Start(data.Ror2mm).WaitForExit();
                                        LoadCustomContent(asset, pluginsFolder + "/" + data.transformName);
                                    }, "Don't Download");
                            }
                        }
                        else
                        {
                            // Process request
                            LoadCustomContent(asset, dir + "Minis/" + transformName + "/" + transformName);
                        }

                        // Reduce stress on system by processing one transformation per cycle
                        break;
                    }
                }
            }
        }*/
    }
}
