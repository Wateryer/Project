using UnityEngine;
using System.Collections;
using LuaFramework;
using System.IO;

public class TestLoad : MonoBehaviour {

    void Start()
    {
        bool isExists = Directory.Exists(Util.DataPath) &&
              Directory.Exists(Util.DataPath + "lua/") && File.Exists(Util.DataPath + "files.txt");
        if (!isExists)
        {
            GameObject prefabs = Resources.Load("Prefabs/Canvas") as GameObject;
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
            bundle = AssetBundle.LoadFromFile(Util.DataPath + "Canvas.unity3d");
            GameObject obj = bundle.LoadAsset("Canvas") as GameObject;
            Instantiate(obj);
            bundle.Unload(true);
            //GameObject asset = bundle.LoadAsset<GameObject>("Canvas");
            //Instantiate(asset);
            //bundle.Unload(true);
        }
        //Caching.CleanCache();
    }
}
