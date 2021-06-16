using System;
using BepInEx;
using Dummiesman;
using LordAshes;
using UnityEngine;

namespace ThunderMan.CustomMinisPlugin
{

    public partial class rCMiniPlugin : BaseUnityPlugin
    {
        public class ModRequestHandler
        {

            /// <summary>
            /// Constructor taking in the content directory and identifiers
            /// </summary>
            /// <param name="requestIdentifiers"></param>
            /// <param name="path"></param>
            public ModRequestHandler()
            {
            }


            /// <summary>
            /// Adds a custom mesh game object to the indicated asset remove any previous attached mesh objects
            /// </summary>
            /// <param name="asset">Parent asset to whom the custom mesh will be attached</param>
            /// <param name="source">Path and name of the content file</param>
            public static void LoadCustomContent(CreatureBoardAsset asset, string source)
            {
                try
                {
                    UnityEngine.Debug.Log("Customizing Mini '" + asset.Creature.Name + "' Using '" + source + "'...");

                    // Effects are prefixed by # tag
                    bool effect = (source.IndexOf("#") > -1);
                    source = source.Replace("#", "");
                    string prefix = (effect) ? "CustomEffect:" : "CustomContent:";
                    Debug.Log("Effect = " + effect);

                    // Look up the content name to see if the actual file has an extenion or not
                    if (System.IO.Path.GetFileNameWithoutExtension(source) != "")
                    {
                        // Obtain file name of the content
                        if (System.IO.File.Exists(source))
                        {
                            // Asset Bundle
                        }
                        else if (System.IO.File.Exists(source + ".OBJ"))
                        {
                            // OBJ File
                            source = source + ".OBJ";
                        }
                    }
                    else
                    {
                        // Source is blank meaning this is a remove request
                        if (!effect)
                        {
                            Debug.Log("Destorying '" + asset.Creature.Name + "' mesh...");
                            asset.CreatureLoader.LoadedAsset.GetComponent<MeshFilter>().mesh.triangles = new int[0];
                        }
                        else
                        {
                            Debug.Log("Destorying '" + asset.Creature.Name + "' effect...");
                            GameObject.Destroy(GameObject.Find(prefix + asset.Creature.CreatureId));
                        }
                        return;
                    }

                    if (System.IO.File.Exists(source))
                    {
                        GameObject content = null;
                        // Determine which type of content it is 
                        switch (System.IO.Path.GetExtension(source).ToUpper())
                        {
                            case "": // AssetBundle Source
                                UnityEngine.Debug.Log("Using AssetBundle Loader");
                                string assetBundleName = System.IO.Path.GetFileNameWithoutExtension(source);
                                AssetBundle assetBundle = null;
                                foreach (AssetBundle ab in AssetBundle.GetAllLoadedAssetBundles())
                                {
                                    // Debug.Log("Checking Existing AssetBundles: Found '" + ab.name + "'. Seeking '"+assetBundleName+"'");
                                    if (ab.name == assetBundleName) { UnityEngine.Debug.Log("AssetBundle Is Already Loaded. Reusing."); assetBundle = ab; break; }
                                }
                                if (assetBundle == null) { UnityEngine.Debug.Log("AssetBundle Is Not Already Loaded. Loading."); assetBundle = AssetBundle.LoadFromFile(source); }
                                content = GameObject.Instantiate(assetBundle.LoadAsset<GameObject>(System.IO.Path.GetFileNameWithoutExtension(source)));
                                break;
                            case ".MINI": // AssetBundle Source
                                UnityEngine.Debug.Log("Using AssetBundle Loader");
                                string massetBundleName = System.IO.Path.GetFileNameWithoutExtension(source);
                                AssetBundle massetBundle = null;
                                foreach (AssetBundle ab in AssetBundle.GetAllLoadedAssetBundles())
                                {
                                    // Debug.Log("Checking Existing AssetBundles: Found '" + ab.name + "'. Seeking '"+assetBundleName+"'");
                                    if (ab.name == massetBundleName) { UnityEngine.Debug.Log("AssetBundle Is Already Loaded. Reusing."); massetBundle = ab; break; }
                                }
                                if (massetBundle == null) { UnityEngine.Debug.Log("AssetBundle Is Not Already Loaded. Loading."); massetBundle = AssetBundle.LoadFromFile(source); }
                                content = GameObject.Instantiate(massetBundle.LoadAsset<GameObject>(System.IO.Path.GetFileNameWithoutExtension(source)));
                                break;
                            case ".OBJ": // OBJ/MTL Source
                                UnityEngine.Debug.Log("Using OBJ/MTL Loader");
                                if (!System.IO.File.Exists(System.IO.Path.GetDirectoryName(source) + "/" + System.IO.Path.GetFileNameWithoutExtension(source) + ".mtl"))
                                {
            
                                }
                                UnityExtension.ShaderDetector.Reference(System.IO.Path.GetDirectoryName(source) + "/" + System.IO.Path.GetFileNameWithoutExtension(source) + ".mtl");
                                content = new OBJLoader().Load(source);
                                break;
                            default: // Unrecognized Source
                                Debug.Log("Content Type '" + System.IO.Path.GetExtension(source).ToUpper() + "' is not supported. Use OBJ/MTL or FBX.");
                                break;
                        }
                        content.name = prefix + asset.Creature.CreatureId;

                        if (!effect)
                        {
                            try
                            {
                                asset.CreatureLoader.transform.position = new Vector3(0, 0, 0);
                                asset.CreatureLoader.transform.rotation = Quaternion.Euler(0, 0, 0);
                                asset.CreatureLoader.transform.eulerAngles = new Vector3(0, 0, 0);
                                asset.CreatureLoader.transform.localPosition = new Vector3(0, 0, 0);
                                asset.CreatureLoader.transform.localRotation = Quaternion.Euler(0, 180, 0);
                                asset.CreatureLoader.transform.localEulerAngles = new Vector3(0, 180, 0);
                                asset.CreatureLoader.transform.localScale = new Vector3(1f, 1f, 1f);
                                var handler = new CustomMiniPlugin.RequestHandler(null, null);
                                handler.ReplaceGameObjectMesh(content, asset.CreatureLoader.LoadedAsset);
                            }
                            catch (Exception) {; }
                            UnityEngine.Debug.Log("Destroying Template...");
                            GameObject.Destroy(GameObject.Find(prefix + asset.Creature.CreatureId));
                        }
                    }
                    else
                    {
                        SystemMessage.DisplayInfoText("I don't know about\r\n" + System.IO.Path.GetFileNameWithoutExtension(source));
                    }
                }
                catch (Exception) {; }
            }
        }
    }
}