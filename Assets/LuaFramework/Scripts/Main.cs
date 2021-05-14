using UnityEngine;
using System.Collections;
using System.IO;

namespace LuaFramework {

    /// <summary>
    /// </summary>
    public class Main : MonoBehaviour {

        void Start() {
            InitUI();
            //yield return new WaitForSeconds(1f);
            AppFacade.Instance.StartUp();   //启动游戏 
        }
        private void InitUI()
        {
            bool isExists = Directory.Exists(Util.DataPath) &&
             Directory.Exists(Util.DataPath + "lua/") && File.Exists(Util.DataPath + "files.txt");
            if (!isExists)
            {
                GameObject prefabs = Resources.Load("Canvas") as GameObject;
                Instantiate(prefabs);
            }
            else
            {
                string url = Util.DataPath + "StreamingAssets";
                //WWW www = WWW.LoadFromCacheOrDownload(url, 0, 0);
                AssetBundle bundle = AssetBundle.LoadFromFile(url);
                //WWW www = new WWW(url);
                //yield return www;
                //AssetBundle bundle = www.assetBundle;
                AssetBundleManifest manifest =
                        (AssetBundleManifest)bundle.LoadAsset("AssetBundleManifest");
                bundle.Unload(false);
                string[] dps = manifest.GetAllDependencies("Canvas.unity3d");
                AssetBundle[] abs = new AssetBundle[dps.Length];
                for (int i = 0; i < dps.Length; i++)
                {
                    string dUrl = Util.DataPath + dps[i];
                    //WWW wwwd = new WWW(dUrl);
                    //while (!wwwd.isDone)
                    //{
                    //    yield return null;
                    //}
                    abs[i] = AssetBundle.LoadFromFile(dUrl);
                }
                AssetBundle asset = AssetBundle.LoadFromFile(Util.DataPath + "Canvas.unity3d");
                GameObject obj = asset.LoadAsset("Canvas") as GameObject;
                Instantiate(obj);
                //bundle.Unload(false);
                asset.Unload(false);
                //for (int i = 0,length=abs.Length; i < length; i++)
                //{
                //    abs[i].Unload(false);
                //}
            }
        }
    }
}