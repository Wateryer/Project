#if ASYNC_MODE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using LuaInterface;
using UObject = UnityEngine.Object;

public class AssetBundleInfo {
    public AssetBundle m_AssetBundle;
    public int m_ReferencedCount;

    public AssetBundleInfo(AssetBundle assetBundle) {
        m_AssetBundle = assetBundle;
        m_ReferencedCount = 0;
    }
}

namespace LuaFramework {

    public class ResourceManager : Manager {
        string m_BaseDownloadingURL = "";
        string[] m_AllManifest = null;
        AssetBundleManifest m_AssetBundleManifest = null;
        Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();
        Dictionary<string, AssetBundleInfo> m_LoadedAssetBundles = new Dictionary<string, AssetBundleInfo>();
        Dictionary<string, List<LoadAssetRequest>> m_LoadRequests = new Dictionary<string, List<LoadAssetRequest>>();

        class LoadAssetRequest {
            public Type assetType;
            public string[] assetNames;
            public LuaFunction luaFunc;
            public Action<UObject[]> sharpFunc;
        }

        // Load AssetBundleManifest.
        public void Initialize(string manifestName, Action initOK) {
            m_BaseDownloadingURL = Util.GetRelativePath();
            LoadAsset<AssetBundleManifest>(manifestName, new string[] { "AssetBundleManifest" }, delegate(UObject[] objs) {
                if (objs.Length > 0) {
                    m_AssetBundleManifest = objs[0] as AssetBundleManifest;
                    m_AllManifest = m_AssetBundleManifest.GetAllAssetBundles();
                }
                if (initOK != null) initOK();
            });
        }

        public void LoadPrefab(string abName, string assetName, Action<UObject[]> func) {
            LoadAsset<GameObject>(abName, new string[] { assetName }, func);
        }

        public void LoadPrefab(string abName, string[] assetNames, Action<UObject[]> func) {
            LoadAsset<GameObject>(abName, assetNames, func);
        }

        public void LoadPrefab(string abName, string[] assetNames, LuaFunction func) {
            LoadAsset<GameObject>(abName, assetNames, null, func);
        }

        string GetRealAssetPath(string abName) {
            if (abName.Equals(AppConst.AssetDir)) {
                return abName;
            }
            abName = abName.ToLower();
            if (!abName.EndsWith(AppConst.ExtName)) {
                abName += AppConst.ExtName;
            }
            if (abName.Contains("/")) {
                return abName;
            }
            //string[] paths = m_AssetBundleManifest.GetAllAssetBundles();  产生GC，需要缓存结果
            for (int i = 0; i < m_AllManifest.Length; i++) {
                int index = m_AllManifest[i].LastIndexOf('/');  
                string path = m_AllManifest[i].Remove(0, index + 1);    //字符串操作函数都会产生GC
                if (path.Equals(abName)) {
                    return m_AllManifest[i];
                }
            }
            LogManager.Error("GetRealAssetPath Error:>>" + abName);
            return null;
        }

        /// <summary>
        /// 载入素材
        /// </summary>
        void LoadAsset<T>(string abName, string[] assetNames, Action<UObject[]> action = null, LuaFunction func = null) where T : UObject {
            abName = GetRealAssetPath(abName);

            LoadAssetRequest request = new LoadAssetRequest();
            request.assetType = typeof(T);
            request.assetNames = assetNames;
            request.luaFunc = func;
            request.sharpFunc = action;

            List<LoadAssetRequest> requests = null;
            if (!m_LoadRequests.TryGetValue(abName, out requests)) {
                requests = new List<LoadAssetRequest>();
                requests.Add(request);
                m_LoadRequests.Add(abName, requests);
                StartCoroutine(OnLoadAsset<T>(abName));
            } else {
                requests.Add(request);
            }
        }

        IEnumerator OnLoadAsset<T>(string abName) where T : UObject {
            AssetBundleInfo bundleInfo = GetLoadedAssetBundle(abName);
            if (bundleInfo == null) {
                yield return StartCoroutine(OnLoadAssetBundle(abName, typeof(T)));

                bundleInfo = GetLoadedAssetBundle(abName);
                if (bundleInfo == null) {
                    m_LoadRequests.Remove(abName);
                    LogManager.Error("OnLoadAsset--->>>" + abName);
                    yield break;
                }
            }
            List<LoadAssetRequest> list = null;
            if (!m_LoadRequests.TryGetValue(abName, out list)) {
                m_LoadRequests.Remove(abName);
                yield break;
            }
            for (int i = 0; i < list.Count; i++) {
                string[] assetNames = list[i].assetNames;
                List<UObject> result = new List<UObject>();

                AssetBundle ab = bundleInfo.m_AssetBundle;
                for (int j = 0; j < assetNames.Length; j++) {
                    string assetPath = assetNames[j];
                    AssetBundleRequest request = ab.LoadAssetAsync(assetPath, list[i].assetType);
                    yield return request;
                    result.Add(request.asset);

                    //T assetObj = ab.LoadAsset<T>(assetPath);
                    //result.Add(assetObj);
                }
                if (list[i].sharpFunc != null) {
                    list[i].sharpFunc(result.ToArray());
                    list[i].sharpFunc = null;
                }
                if (list[i].luaFunc != null) {
                    list[i].luaFunc.Call((object)result.ToArray());
                    list[i].luaFunc.Dispose();
                    list[i].luaFunc = null;
                }
                bundleInfo.m_ReferencedCount++;
            }
            m_LoadRequests.Remove(abName);
        }

        IEnumerator OnLoadAssetBundle(string abName, Type type) {
            string url = m_BaseDownloadingURL + abName;

            WWW download = null;
            if (type == typeof(AssetBundleManifest))
                download = new WWW(url);
            else {
                string[] dependencies = m_AssetBundleManifest.GetAllDependencies(abName);
                if (dependencies.Length > 0) {
                    m_Dependencies.Add(abName, dependencies);
                    for (int i = 0; i < dependencies.Length; i++) {
                        string depName = dependencies[i];
                        AssetBundleInfo bundleInfo = null;
                        if (m_LoadedAssetBundles.TryGetValue(depName, out bundleInfo)) {
                            bundleInfo.m_ReferencedCount++;
                        } else if (!m_LoadRequests.ContainsKey(depName)) {
                            yield return StartCoroutine(OnLoadAssetBundle(depName, type));
                        }
                    }
                }
                download = WWW.LoadFromCacheOrDownload(url, m_AssetBundleManifest.GetAssetBundleHash(abName), 0);
            }
            yield return download;

            AssetBundle assetObj = download.assetBundle;
            if (assetObj != null) {
                m_LoadedAssetBundles.Add(abName, new AssetBundleInfo(assetObj));
            }
        }

        AssetBundleInfo GetLoadedAssetBundle(string abName) {
            AssetBundleInfo bundle = null;
            m_LoadedAssetBundles.TryGetValue(abName, out bundle);
            if (bundle == null) return null;

            // No dependencies are recorded, only the bundle itself is required.
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(abName, out dependencies))
                return bundle;

            // Make sure all dependencies are loaded
            foreach (var dependency in dependencies) {
                AssetBundleInfo dependentBundle;
                m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
                if (dependentBundle == null) return null;
            }
            return bundle;
        }

        /// <summary>
        /// 此函数交给外部卸载专用，自己调整是否需要彻底清除AB
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="isThorough"></param>
        public void UnloadAssetBundle(string abName, bool isThorough = false) {
            abName = GetRealAssetPath(abName);
            LogManager.Debug(m_LoadedAssetBundles.Count + " assetbundle(s) in memory before unloading " + abName);
            UnloadAssetBundleInternal(abName, isThorough);
            UnloadDependencies(abName, isThorough);
            LogManager.Debug(m_LoadedAssetBundles.Count + " assetbundle(s) in memory after unloading " + abName);
        }

        void UnloadDependencies(string abName, bool isThorough) {
            string[] dependencies = null;
            if (!m_Dependencies.TryGetValue(abName, out dependencies))
                return;

            // Loop dependencies.
            foreach (var dependency in dependencies) {
                UnloadAssetBundleInternal(dependency, isThorough);
            }
            m_Dependencies.Remove(abName);
        }

        void UnloadAssetBundleInternal(string abName, bool isThorough) {
            AssetBundleInfo bundle = GetLoadedAssetBundle(abName);
            if (bundle == null) return;

            if (--bundle.m_ReferencedCount <= 0) {
                if (m_LoadRequests.ContainsKey(abName)) {
                    return;     //如果当前AB处于Async Loading过程中，卸载会崩溃，只减去引用计数即可
                }
                bundle.m_AssetBundle.Unload(isThorough);
                m_LoadedAssetBundles.Remove(abName);
                LogManager.Debug(abName + " has been unloaded successfully");
            }
        }
    }
}
#else

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LuaFramework;
using LuaInterface;
using UObject = UnityEngine.Object;
using System;
using UnityEngine.SceneManagement;

namespace LuaFramework {
    public class ResourceManager : Manager {
        private string[] m_Variants = { };
        private AssetBundleManifest manifest;
        private AssetBundle shared, assetbundle;
        private Dictionary<string, AssetBundle> bundles;

        void Awake() {
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize() {
            byte[] stream = null;
            string uri = string.Empty;
            bundles = new Dictionary<string, AssetBundle>();
            uri = Util.DataPath + AppConst.AssetDir;
            if (!File.Exists(uri)) return;
            stream = File.ReadAllBytes(uri);
            assetbundle = AssetBundle.LoadFromMemory(stream);
            manifest = assetbundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        /// <summary>
        /// 载入素材
        /// </summary>
        public T LoadAsset<T>(string abname, string assetname) where T : UnityEngine.Object {
            abname = abname.ToLower();
            AssetBundle bundle = LoadAssetBundle(abname);
            if (bundle.isStreamedSceneAssetBundle)
            {
                SceneManager.LoadSceneAsync(assetname, LoadSceneMode.Single);
                return null;
            }
            return bundle.LoadAsset<T>(assetname);
        }
        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="abName">包文件名</param>
        /// <param name="sceneName">场景名</param>
        /// <param name="sceneMode">加载模式</param>
        public void LoadScene(string abName, string sceneName, LoadSceneMode sceneMode,LuaFunction func) 
        {
            abName = abName.ToLower();
            if (!abName.EndsWith(AppConst.ExtName))
            {
                abName += AppConst.ExtName;
            }
            //string uri = Util.DataPath + abName;
            //WWW www = new WWW(uri + abName);
            //yield return www;
            //AssetBundle bundle = www.assetBundle;
            AssetBundle bundle = LoadAssetBundle(abName);
            if (bundle.isStreamedSceneAssetBundle)
            {
                SceneManager.LoadSceneAsync(sceneName, sceneMode);
            }
            //StartCoroutine(LoadSceneAsync(uri, sceneName, sceneMode));
            if (func != null) func.Call();
        }

        //private IEnumerator LoadSceneAsync(string uri, string sceneName, LoadSceneMode sceneMode)
        //{
        //    WWW www = new WWW(uri);
        //    yield return www;
        //    AssetBundle bundle = www.assetBundle;
        //    if (bundle.isStreamedSceneAssetBundle)
        //    {
        //        SceneManager.LoadSceneAsync(sceneName, sceneMode);
        //    }
        //}

        public void LoadPrefab(string abName, string[] assetNames, LuaFunction func) {
            abName = abName.ToLower();
            List<UObject> result = new List<UObject>();
            for (int i = 0; i < assetNames.Length; i++) {
                UObject go = LoadAsset<UObject>(abName, assetNames[i]);
                if (go != null) result.Add(go);
            }
            if (func != null) func.Call((object)result.ToArray());
        }

        /// <summary>
        /// 载入AssetBundle
        /// </summary>
        /// <param name="abname"></param>
        /// <returns></returns>
        public AssetBundle LoadAssetBundle(string abname) {
            if (!abname.EndsWith(AppConst.ExtName)) {
                abname += AppConst.ExtName;
            }
            AssetBundle bundle = null;
            if (bundles == null)
            {
                //StartCoroutine(LoadAB("StreamingAssets", abname,bundle));
                //while (bundle == null) { }
                //LoadAB("StreamingAssets", abname);
            }
            else if (!bundles.ContainsKey(abname)) {
                byte[] stream = null;
                string uri = Util.DataPath + abname;
                LogManager.Warning("LoadFile::>> " + uri);
                LoadDependencies(abname);

                stream = File.ReadAllBytes(uri);
                bundle = AssetBundle.LoadFromMemory(stream); //关联数据的素材绑定
                bundles.Add(abname, bundle);
            } else {
                bundles.TryGetValue(abname, out bundle);
            }
            return bundle;
        }

        public void LoadFromAB(string abname, string res)
        {
            StartCoroutine(LoadAB("StreamingAssets", abname, res));
        }

        private IEnumerator LoadAB(string path, string assetbunld, string res)
        {
            //AssetBundle bundle=null;
           string Url =
#if UNITY_ANDROID
 "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
			Application.dataPath + "/Raw/";
#elif UNITY_EDITOR || UNITY_STANDALONE_WIN
			"file://" + Application.dataPath + "/StreamingAssets/";
#endif
            //加载目录名的ab文件（StreamingAssets）
           string mUrl = Url + path;
            WWW www = new WWW(mUrl);
            while (!www.isDone)
            {
                yield return new WaitForEndOfFrame();
            }
            //while (!www.isDone) { }
            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.Log(www.error);
            }
            else
            {
                //获取ab文件对应的配置文件（manifest）
                AssetBundle mab = www.assetBundle;
                AssetBundleManifest manifest =
                    (AssetBundleManifest)mab.LoadAsset("AssetBundleManifest");
                mab.Unload(false);
                //根据配置文件获取我们需要的（ab文件）依赖关系
                string[] dps = manifest.GetAllDependencies(assetbunld);
                AssetBundle[] abs = new AssetBundle[dps.Length];
                //加载依赖的ab文件
                for (int i = 0; i < dps.Length; i++)
                {
                    string dUrl = Url + dps[i];
                   WWW wwwd = new WWW(dUrl);
                    while (!wwwd.isDone)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    //while (!www.isDone) { }
                    abs[i] = wwwd.assetBundle;
                }
                //加载ab文件
               WWW wwwab = new WWW(Url + assetbunld);
               while (!wwwab.isDone)
                {
                    yield return new WaitForEndOfFrame();
                }
                //while (!www.isDone) { }
               if (!string.IsNullOrEmpty(wwwab.error))
                {
                    Debug.Log(wwwab.error);
                }
                else
                {
                    //加载资源文件
                    AssetBundle ab = wwwab.assetBundle;
                    GameObject obj = ab.LoadAsset(res) as GameObject;
                    //实例化资源
                    Instantiate(obj);
                    //bundle = ab;
                    ab.Unload(false);
                }

                foreach (AssetBundle ab in abs)
                {
                    ab.Unload(false);
                }
            }
            //return bundle;
            //yield return obj;
        }


        /// <summary>
        /// 载入依赖
        /// </summary>
        /// <param name="name"></param>
        void LoadDependencies(string name) {
            if (manifest == null) {
                LogManager.Error("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
                return;
            }
            // Get dependecies from the AssetBundleManifest object..
            string[] dependencies = manifest.GetAllDependencies(name);
            if (dependencies.Length == 0) return;

            for (int i = 0; i < dependencies.Length; i++)
                dependencies[i] = RemapVariantName(dependencies[i]);

            // Record and load all dependencies.
            for (int i = 0; i < dependencies.Length; i++) {
                LoadAssetBundle(dependencies[i]);
            }
        }

        // Remaps the asset bundle name to the best fitting asset bundle variant.
        string RemapVariantName(string assetBundleName) {
            string[] bundlesWithVariant = manifest.GetAllAssetBundlesWithVariant();

            // If the asset bundle doesn't have variant, simply return.
            if (System.Array.IndexOf(bundlesWithVariant, assetBundleName) < 0)
                return assetBundleName;

            string[] split = assetBundleName.Split('.');

            int bestFit = int.MaxValue;
            int bestFitIndex = -1;
            // Loop all the assetBundles with variant to find the best fit variant assetBundle.
            for (int i = 0; i < bundlesWithVariant.Length; i++) {
                string[] curSplit = bundlesWithVariant[i].Split('.');
                if (curSplit[0] != split[0])
                    continue;

                int found = System.Array.IndexOf(m_Variants, curSplit[1]);
                if (found != -1 && found < bestFit) {
                    bestFit = found;
                    bestFitIndex = i;
                }
            }
            if (bestFitIndex != -1)
                return bundlesWithVariant[bestFitIndex];
            else
                return assetBundleName;
        }

        /// <summary>
        /// 销毁资源
        /// </summary>
        void OnDestroy() {
            if (shared != null) shared.Unload(true);
            if (manifest != null) manifest = null;
            LogManager.Debug("~ResourceManager was destroy!");
        }
        public void Load(string abName, string[] assetNames, LuaFunction func)
        {
            //ObjectPoolManager pool = AppFacade.Instance.GetManager<ObjectPoolManager>(ManagerName.ObjectPool);
            int length = assetNames.Length;
            List<UObject> result = new List<UObject>();
            int index = 0;
            for (int i = 0; i < length; i++)
            {
                GameObject go = ObjPoolManager.Get(assetNames[i]);
                if (go != null)
                {
                    result.Add(go);
                    index++;
                }
                else if (ObjPoolManager.GetPool(assetNames[i])==null)
                {
                    GameObject prefab = LoadAsset<UObject>(abName, assetNames[i]) as GameObject;
                    if (prefab != null)
                    {
                        ObjPoolManager.CreatePool(assetNames[i], 1, 1, prefab);
                    }
                    GameObject newGo = ObjPoolManager.Get(assetNames[i]);
                    if (newGo != null)
                    {
                        result.Add(newGo);
                        index++;
                    }
                }
            }
            if (index < length)
            {
                string[] assetNamesCopy = new string[length - index];
                Array.Copy(assetNames, index, assetNamesCopy, 0, length - index);
                //LoadPrefab(name, assetNamesCopy, result, func);
                abName = abName.ToLower();
                for (int i = 0; i < assetNamesCopy.Length; i++)
                {
                    UObject prefab = LoadAsset<UObject>(abName, assetNames[i]);
                    if (prefab != null)
                    {
                        GameObject go = Instantiate(prefab) as GameObject;
                        ObjPoolManager.Release(assetNamesCopy[i], go);
                        GameObject newGO = ObjPoolManager.Get(assetNamesCopy[i]);
                        if (newGO != null)
                        {
                            result.Add(newGO);
                        }
                    }
                }
            }
            if (func != null) func.Call((object)result.ToArray());
        }
        public void Release(string name, GameObject go)
        {
            if (name.Contains("(Clone)"))
                name= name.Replace("(Clone)", "");
            ObjPoolManager.Release(name, go);
        }
    }
}
#endif
