using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using System.Reflection;
using System.IO;
using UObject = UnityEngine.Object;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Networking;

namespace LuaFramework {
    public class GameManager : Manager {
        protected static bool initialize = false;
        private List<string> downloadFiles = new List<string>();

        /// <summary>
        /// 初始化游戏管理器
        /// </summary>
        void Awake() {
            Init();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        void Init()
        {
            //InitUI();
            DontDestroyOnLoad(gameObject);  //防止销毁自己 
            CheckExtractResource(); //释放资源
            //Invoke("CheckExtractResource", 1f);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = AppConst.GameFrameRate;
        }
        private void InitUI()
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
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void CheckExtractResource() {
            bool isExists = Directory.Exists(Util.DataPath) &&
              Directory.Exists(Util.DataPath + "lua/") && File.Exists(Util.DataPath + "files.txt");
            if (isExists || AppConst.DebugMode) {
                StartCoroutine(OnUpdateResource());
                return;   //文件已经解压过了，自己可添加检查文件列表逻辑
            }
            StartCoroutine(OnExtractResource());    //启动释放协成 
        }

        IEnumerator OnExtractResource() {
            string dataPath = Util.DataPath;  //数据目录
            string resPath = Util.AppContentPath(); //游戏包资源目录

            if (Directory.Exists(dataPath)) Directory.Delete(dataPath, true);
            Directory.CreateDirectory(dataPath);

            string infile = resPath + "files.txt";
            string outfile = dataPath + "files.txt";
            if (File.Exists(outfile)) File.Delete(outfile);

            string message = "正在解包文件:>files.txt";
            LogManager.Debug(infile);
            LogManager.Debug(outfile);
            if (Application.platform == RuntimePlatform.Android) {
                WWW www = new WWW(infile);
                yield return www;

                if (www.isDone) {
                    File.WriteAllBytes(outfile, www.bytes);
                }
                yield return 0;
            } else File.Copy(infile, outfile, true);
            yield return new WaitForEndOfFrame();

            //需要解压的文件名字
            List<string> willExtractFileName = new List<string>();

            //释放所有文件到数据目录
            string[] files = File.ReadAllLines(outfile);

            foreach (var file in files)
            {
                string[] fs = file.Split('|');
                infile = resPath + fs[0];
                outfile = dataPath + fs[0];
                willExtractFileName.Add(fs[0]);
            }
            if (willExtractFileName.Count > 0) 
            facade.SendMessageCommand(NotiConst.EXTRACT_ALL_COUNT, willExtractFileName.Count);

            for (int i = 0, length = willExtractFileName.Count; i < length; i++)
            {
                message = "正在解包文件:>"+ willExtractFileName[i];
                LogManager.Debug("正在解包文件:>" + willExtractFileName[i]);
                string from = resPath + willExtractFileName[i];
                string to = dataPath + willExtractFileName[i];
                facade.SendMessageCommand(NotiConst.EXTRACT_FILE_NAME, willExtractFileName[i]);
                string dir = Path.GetDirectoryName(to);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                if (Application.platform == RuntimePlatform.Android)
                {
                    //if (!firstExtractResource) facade.SendMessageCommand(NotiConst.EXTRACT_FILE_NAME, willExtractFileName[i]);
                    WWW www = new WWW(from);
                    yield return www;

                    if (www.isDone)
                    {
                        File.WriteAllBytes(to, www.bytes);
                        facade.SendMessageCommand(NotiConst.EXTRACT_FINISH_ONE, 0);
                    }
                    yield return 0;
                }
                else
                {
                    if (File.Exists(to))
                    {
                        File.Delete(to);
                    }
                    File.Copy(from, to, true);
                    facade.SendMessageCommand(NotiConst.EXTRACT_FINISH_ONE, 0);
                }
                yield return new WaitForEndOfFrame();
            }
            /*
            foreach (var file in files) {
                string[] fs = file.Split('|');
                infile = resPath + fs[0];  //
                outfile = dataPath + fs[0];
                message = "正在解包文件:>"+ fs[0];
                LogManager.Debug("正在解包文件:>"+ infile);
                facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);
                text.text = message;
                string dir = Path.GetDirectoryName(outfile);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                if (Application.platform == RuntimePlatform.Android) {
                    WWW www = new WWW(infile);
                    yield return www;

                    if (www.isDone) {
                        File.WriteAllBytes(outfile, www.bytes);
                    }
                    yield return 0;
                } else {
                    if (File.Exists(outfile)) {
                        File.Delete(outfile);
                    }
                    File.Copy(infile, outfile, true);
                }
                yield return new WaitForEndOfFrame();
                index++;
                loadProgress.value = (float)index / files.Length;
                progressText.text = Mathf.Round(loadProgress.value * 100).ToString() + "/100";
            }*/
            message = "解包完成!!!";
            facade.SendMessageCommand(NotiConst.UPDATE_MESSAGE, message);
            yield return new WaitForSeconds(0.1f);

            message = string.Empty;
            //释放完成，开始启动更新资源
            StartCoroutine(OnUpdateResource());
        }

        /// <summary>
        /// 启动更新下载，这里只是个思路演示，此处可启动线程下载更新
        /// </summary>
        IEnumerator OnUpdateResource() {
            if (!AppConst.UpdateMode) {
                OnResourceInited();
                yield break;
            }
            string dataPath = Util.DataPath;  //数据目录
            string url = AppConst.WebUrl;
            string message = string.Empty;
            string random = DateTime.Now.ToString("yyyymmddhhmmss");
            string listUrl = url + "files.txt?v=" + random;
            //string listUrl = url + "files.txt";
            LogManager.Warning("正在检测更新---->>>" + listUrl);
            message = "正在检测更新---->>>" + listUrl;
            facade.SendMessageCommand(NotiConst.CHECK_UPDATE, message);
            WWW www = new WWW(listUrl); yield return www;
            if (www.error != null) {
                OnUpdateFailed(string.Empty);
                yield break;
            }
            if (!Directory.Exists(dataPath)) {
                Directory.CreateDirectory(dataPath);
            }
            File.WriteAllBytes(dataPath + "files.txt", www.bytes);
            string filesText = www.text;
            string[] files = filesText.Split('\n');

            List<string> willDownLoadUrl = new List<string>();//from
            List<string> willDownLoadFileName = new List<string>();
            List<string> willDownLoadDestination = new List<string>();//to

            
            float countLength = 0;
            System.Net.HttpWebRequest request = null;
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                    delegate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                             System.Security.Cryptography.X509Certificates.X509Chain chain,
                             System.Net.Security.SslPolicyErrors sslPolicyErrors)
                    {
                        return true; // **** Always accept
                    };
            for (int i = 0; i < files.Length; i++) {
                if (string.IsNullOrEmpty(files[i])) continue;
                string[] keyValue = files[i].Split('|');
                string f = keyValue[0];
                string localfile = (dataPath + f).Trim();
                string path = Path.GetDirectoryName(localfile);
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
                string fileUrl = url + f + "?v=" + random;
                //string fileUrl = url + f ;
                bool canUpdate = !File.Exists(localfile);
                
                if (!canUpdate) {
                    string remoteMd5 = keyValue[1].Trim();
                    string localMd5 = Util.md5file(localfile);
                    canUpdate = !remoteMd5.Equals(localMd5);

                    if (canUpdate) File.Delete(localfile);
                }
                if (canUpdate) {   //本地缺少文件

                    //message = "正在检查更新>>" + fileUrl;
                    //facade.SendMessageCommand(NotiConst.CHECK_UPDATE, message);
                    /*
                    www = new WWW(fileUrl);
                    loadProgress.value = www.progress;
                    progressText.text = Mathf.Round(loadProgress.value * 100).ToString() + "/100";
                    yield return www;
                    if (www.error != null) {
                        OnUpdateFailed(path);   //
                        yield break;
                    }
                    File.WriteAllBytes(localfile, www.bytes);
                     */
                    //这里都是资源文件，用线程下载
                    //BeginDownload(fileUrl, localfile);
                    //while (!(IsDownOK(localfile))) { yield return new WaitForEndOfFrame(); }

                    willDownLoadUrl.Add(fileUrl);//下载地址
                    willDownLoadFileName.Add(keyValue[0]);
                    willDownLoadDestination.Add(localfile);//目标文件路径

                    //计算文件大小
                    request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(fileUrl);
                    request.UseDefaultCredentials = true;
                    request.Method = "HEAD";
                    countLength += request.GetResponse().ContentLength;
                }
            }
            
            //System.Net.HttpWebRequest request=null;
            ////计算文件大小
            //for (int i = 0; i < willDownLoadUrl.Count; i++)
            //{
            //    request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(willDownLoadUrl[i]);
            //    request.UseDefaultCredentials = true;
            //    request.Method = "HEAD";
            //    countLength += request.GetResponse().ContentLength;
            //    //UnityWebRequest uwr = UnityWebRequest.Head(willDownLoadUrl[i]);
            //    //yield return uwr.Send();
            //    //string size = uwr.GetResponseHeader("Content-Length");
            //    //if (!string.IsNullOrEmpty(size)) //{ OnUpdateFailed(null); yield break; }
            //    //    countLength += float.Parse(size);
            //}

            string value = string.Format("{0}",Math.Round (countLength / 1024,2 ));
            facade.SendMessageCommand(NotiConst.UPDATE_ALL_COUNTLENGTH, value);
            if (willDownLoadUrl.Count > 0) facade.SendMessageCommand(NotiConst.UPDATE_ALL_COUNT, willDownLoadUrl.Count);
            for (int i = 0; i < willDownLoadUrl.Count; i++)
            {
                LogManager.Debug("要下载的文件：" + willDownLoadUrl[i]);
                //这里都是资源文件，用线程下载
                facade.SendMessageCommand(NotiConst.UPDATE_FILE_NAME, willDownLoadFileName[i]);
                BeginDownload(willDownLoadUrl[i], willDownLoadDestination[i]);
                while (!(IsDownOK(willDownLoadDestination[i]))) { yield return new WaitForEndOfFrame(); }
            }

            yield return new WaitForEndOfFrame();
            //yield return new WaitForSeconds(0.1f);
            message = "更新完成!";
            facade.SendMessageCommand(NotiConst.CHECK_UPDATE, message);
            OnResourceInited();
        }

        void OnUpdateFailed(string file) {
            string message = "更新失败!" + file;
            facade.SendMessageCommand(NotiConst.CHECK_UPDATE, message);
        }

        /// <summary>
        /// 是否下载完成
        /// </summary>
        bool IsDownOK(string file) {
            return downloadFiles.Contains(file);
        }

        /// <summary>
        /// 线程下载
        /// </summary>
        void BeginDownload(string url, string file) {     //线程下载
            object[] param = new object[2] { url, file };

            ThreadEvent ev = new ThreadEvent();
            ev.Key = NotiConst.UPDATE_FINISH_ONE;
            ev.evParams.AddRange(param);
            ThreadManager.AddEvent(ev, OnThreadCompleted);   //线程下载
        }

        /// <summary>
        /// 线程完成
        /// </summary>
        /// <param name="data"></param>
        void OnThreadCompleted(NotiData data) {
            switch (data.evName) {
                case NotiConst.THREAD_EXTRACT:  //解压一个完成
                //
                break;
                case NotiConst.UPDATE_FINISH_ONE: //下载一个完成
                downloadFiles.Add(data.evParam.ToString());
                break;
            }
        }

        /// <summary>
        /// 资源初始化结束
        /// </summary>
        public void OnResourceInited() {
#if ASYNC_MODE
            ResManager.Initialize(AppConst.AssetDir, delegate() {
                LogManager.Debug("Initialize OK!!!");
                this.OnInitialize();
            });
#else
            ResManager.Initialize();
            this.OnInitialize();
#endif
        }

        void OnInitialize() {
            LuaManager.InitStart();
            LuaManager.DoFile("Logic/Game");         //加载游戏
            LuaManager.DoFile("Logic/Network");      //加载网络
            NetManager.OnInit();                     //初始化网络
            Util.CallMethod("Game", "OnInitOK");     //初始化完成

            initialize = true;

            //类对象池测试
            ObjPoolManager.CreatePool<TestObjectClass>(OnPoolGetElement, OnPoolPushElement);
            //方法1
            //objPool.Release(new TestObjectClass("abcd", 100, 200f));
            //var testObj1 = objPool.Get();

            //方法2
            ObjPoolManager.Release<TestObjectClass>(new TestObjectClass("abcd", 100, 200f));
            var testObj1 = ObjPoolManager.Get<TestObjectClass>();

            LogManager.Debug("TestObjectClass--->>>" + testObj1.ToString());

            //cube
            //ResManager.LoadPrefab("prefabs.unity3d", new string[] { "Cube" }, null);
            var prefab = LoadPrefab("prefabs.unity3d", "Cube") as GameObject;
            ObjPoolManager.CreatePool("Cube", 5, 10, prefab);
            //Particle
            //ResManager.LoadPrefab("prefabs.unity3d", new string[] { "Particle" }, null);
            prefab = LoadPrefab("prefabs.unity3d", "CircleParticle") as GameObject;
            ObjPoolManager.CreatePool("CircleParticle", 1, 1, prefab);
            //游戏对象池测试
            //var prefab = Resources.Load("TestGameObjectPrefab", typeof(GameObject)) as GameObject;
            

            //var gameObj = Instantiate(prefab) as GameObject;
            //gameObj.name = "TestGameObject_01";
            //gameObj.transform.localScale = Vector3.one;
            //gameObj.transform.localPosition = Vector3.zero;

            //ObjPoolManager.Release("TestGameObject", gameObj);
            //var backObj = ObjPoolManager.Get("TestGameObject");
            //if (backObj != null)
            //{
            //    backObj.transform.SetParent(null);
            //}

            //LogManager.Debug("TestGameObject--->>>" + backObj);
        }

        /// <summary>
        /// 当从池子里面获取时
        /// </summary>
        /// <param name="obj"></param>
        void OnPoolGetElement(TestObjectClass obj) {
            LogManager.Debug("OnPoolGetElement--->>>" + obj);
        }

        /// <summary>
        /// 当放回池子里面时
        /// </summary>
        /// <param name="obj"></param>
        void OnPoolPushElement(TestObjectClass obj) {
            LogManager.Debug("OnPoolPushElement--->>>" + obj);
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        void OnDestroy() {
            if (NetManager != null) {
                NetManager.Unload();
            }
            if (LuaManager != null) {
                LuaManager.Close();
            }
            LogManager.Debug("~GameManager was destroyed");
        }
        UObject LoadPrefab(string abName, string assetName)
        {
            abName = abName.ToLower();
            UObject go =ResManager.LoadAsset<UObject>(abName, assetName);
            return go;
        }
    }
}