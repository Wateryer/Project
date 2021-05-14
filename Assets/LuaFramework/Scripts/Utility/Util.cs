using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using LuaInterface;
using LuaFramework;
using DG.Tweening;
using UnityEngine.UI;
using UObject = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LuaFramework {
    public class Util {
        private static List<string> luaPaths = new List<string>();

        public static int Int(object o) {
            return Convert.ToInt32(o);
        }

        public static float Float(object o) {
            return (float)Math.Round(Convert.ToSingle(o), 2);
        }

        public static long Long(object o) {
            return Convert.ToInt64(o);
        }

        public static int Random(int min, int max) {
            return UnityEngine.Random.Range(min, max);
        }

        public static float Random(float min, float max) {
            return UnityEngine.Random.Range(min, max);
        }

        public static string Uid(string uid) {
            int position = uid.LastIndexOf('_');
            return uid.Remove(0, position + 1);
        }

        public static long GetTime() {
            TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
            return (long)ts.TotalMilliseconds;
        }

        /// <summary>
        /// 搜索子物体组件-GameObject版
        /// </summary>
        public static T Get<T>(GameObject go, string subnode) where T : Component {
            if (go != null) {
                Transform sub = go.transform.FindChild(subnode);
                if (sub != null) return sub.GetComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 搜索子物体组件-Transform版
        /// </summary>
        public static T Get<T>(Transform go, string subnode) where T : Component {
            if (go != null) {
                Transform sub = go.FindChild(subnode);
                if (sub != null) return sub.GetComponent<T>();
            }
            return null;
        }

        public static UnityEngine.UI.Text GetText(GameObject go, string subnode)
        {
            if (go != null)
            {
                Transform sub = go.transform.FindChild(subnode);
                if (sub != null) return sub.GetComponent<UnityEngine.UI.Text>();
            }
            return null;
        }

        /// <summary>
        /// 搜索子物体组件-Component版
        /// </summary>
        public static T Get<T>(Component go, string subnode) where T : Component {
            return go.transform.FindChild(subnode).GetComponent<T>();
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        public static T Add<T>(GameObject go) where T : Component {
            if (go != null) {
                T[] ts = go.GetComponents<T>();
                for (int i = 0; i < ts.Length; i++) {
                    if (ts[i] != null) GameObject.Destroy(ts[i]);
                }
                return go.gameObject.AddComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        public static T Add<T>(Transform go) where T : Component {
            return Add<T>(go.gameObject);
        }

        /// <summary>
        /// 查找子对象
        /// </summary>
        public static GameObject Child(GameObject go, string subnode) {
            return Child(go.transform, subnode);
        }

        /// <summary>
        /// 查找子对象
        /// </summary>
        public static GameObject Child(Transform go, string subnode) {
            Transform tran = go.FindChild(subnode);
            if (tran == null) return null;
            return tran.gameObject;
        }

        /// <summary>
        /// 取平级对象
        /// </summary>
        public static GameObject Peer(GameObject go, string subnode) {
            return Peer(go.transform, subnode);
        }

        /// <summary>
        /// 取平级对象
        /// </summary>
        public static GameObject Peer(Transform go, string subnode) {
            Transform tran = go.parent.FindChild(subnode);
            if (tran == null) return null;
            return tran.gameObject;
        }

        /// <summary>
        /// 计算字符串的MD5值
        /// </summary>
        public static string md5(string source) {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
            byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
            md5.Clear();

            string destString = "";
            for (int i = 0; i < md5Data.Length; i++) {
                destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
            }
            destString = destString.PadLeft(32, '0');
            return destString;
        }

        /// <summary>
        /// 计算文件的MD5值
        /// </summary>
        public static string md5file(string file) {
            try {
                FileStream fs = new FileStream(file, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(fs);
                fs.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++) {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            } catch (Exception ex) {
                throw new Exception("md5file() fail, error:" + ex.Message);
            }
        }

        /// <summary>
        /// 清除所有子节点
        /// </summary>
        public static void ClearChild(Transform go) {
            if (go == null) return;
            for (int i = go.childCount - 1; i >= 0; i--) {
                GameObject.Destroy(go.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 清理内存
        /// </summary>
        public static void ClearMemory() {
            GC.Collect(); Resources.UnloadUnusedAssets();
            LuaManager mgr = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
            if (mgr != null) mgr.LuaGC();
        }

        /// <summary>
        /// 取得数据存放目录
        /// </summary>
        public static string DataPath {
            get {
                string game = AppConst.AppName.ToLower();
                if (Application.isMobilePlatform) {
                    return Application.persistentDataPath + "/" + game + "/";
                }
                if (AppConst.DebugMode) {
                    return Application.dataPath + "/" + AppConst.AssetDir + "/";
                }
                if (Application.platform == RuntimePlatform.OSXEditor) {
                    int i = Application.dataPath.LastIndexOf('/');
                    return Application.dataPath.Substring(0, i + 1) + game + "/";
                }
                return "c:/" + game + "/";
            }
        }

        public static string GetRelativePath() {
            if (Application.isEditor)
                return "file://" + System.Environment.CurrentDirectory.Replace("\\", "/") + "/Assets/" + AppConst.AssetDir + "/";
            else if (Application.isMobilePlatform || Application.isConsolePlatform)
                return "file:///" + DataPath;
            else // For standalone player.
                return "file://" + Application.streamingAssetsPath + "/";
        }

        /// <summary>
        /// 取得行文本
        /// </summary>
        public static string GetFileText(string path) {
            return File.ReadAllText(path);
        }

        /// <summary>
        /// 网络可用
        /// </summary>
        public static bool NetAvailable {
            get {
                return Application.internetReachability != NetworkReachability.NotReachable;
            }
        }

        /// <summary>
        /// 是否是无线
        /// </summary>
        public static bool IsWifi {
            get {
                return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
            }
        }

        /// <summary>
        /// 应用程序内容路径
        /// </summary>
        public static string AppContentPath() {
            string path = string.Empty;
            switch (Application.platform) {
                case RuntimePlatform.Android:
                    path = "jar:file://" + Application.dataPath + "!/assets/";
                break;
                case RuntimePlatform.IPhonePlayer:
                    path = Application.dataPath + "/Raw/";
                break;
                default:
                    path = Application.dataPath + "/" + AppConst.AssetDir + "/";
                break;
            }
            return path;
        }

        public static void Log(string str) {
            LogManager.Debug(str);
        }

        public static void LogWarning(string str) {
            LogManager.Warning(str);
        }

        public static void LogError(string str) {
            LogManager.Error(str);
        }

        /// <summary>
        /// 防止初学者不按步骤来操作
        /// </summary>
        /// <returns></returns>
        public static int CheckRuntimeFile() {
            if (!Application.isEditor) return 0;
            string streamDir = Application.dataPath + "/StreamingAssets/";
            if (!Directory.Exists(streamDir)) {
                return -1;
            } else {
                string[] files = Directory.GetFiles(streamDir);
                if (files.Length == 0) return -1;

                if (!File.Exists(streamDir + "files.txt")) {
                    return -1;
                }
            }
            string sourceDir = AppConst.FrameworkRoot + "/ToLua/Source/Generate/";
            if (!Directory.Exists(sourceDir)) {
                return -2;
            } else {
                string[] files = Directory.GetFiles(sourceDir);
                if (files.Length == 0) return -2;
            }
            return 0;
        }

        /// <summary>
        /// 执行Lua方法
        /// </summary>
        public static object[] CallMethod(string module, string func, params object[] args) {
            LuaManager luaMgr = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
            if (luaMgr == null) return null;
            return luaMgr.CallFunction(module + "." + func, args);
        }
        public static void DoFile(string name)
        {
            LuaManager luaMgr = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
            if (luaMgr == null) return ;
            luaMgr.DoFile(name);
        }
        

        /// <summary>
        /// 检查运行环境
        /// </summary>
        public static bool CheckEnvironment() {
#if UNITY_EDITOR
            int resultId = Util.CheckRuntimeFile();
            if (resultId == -1) {
                LogManager.Error("没有找到框架所需要的资源，单击Game菜单下Build xxx Resource生成！！");
                EditorApplication.isPlaying = false;
                return false;
            } else if (resultId == -2) {
                LogManager.Error("没有找到Wrap脚本缓存，单击Lua菜单下Gen Lua Wrap Files生成脚本！！");
                EditorApplication.isPlaying = false;
                return false;
            }
            if (Application.loadedLevelName == "Test" && !AppConst.DebugMode) {
                LogManager.Error("测试场景，必须打开调试模式，AppConst.DebugMode = true！！");
                EditorApplication.isPlaying = false;
                return false;
            }
#endif
            return true;
        }

        //摄像机DoTween移动
        public static void DoMove(Camera cam, Vector3 vec, float time,LuaFunction f)
        {
            cam.transform.DOMove(vec, time).OnComplete(() => { if (f != null) f.Call(); });
        }
        //GameObject DoTween移动
        public static void DoMove(GameObject obj, Vector3 vec, float time)
        {
            obj.transform.DOMove(vec, time);
        }
        public static void DOJump(GameObject obj, Vector3 vec,float power,int num, float time,bool snapping,LuaFunction f)
        {
            obj.transform.DOJump(vec, power, num, time, snapping).SetEase(Ease.Linear).OnComplete(() => { if (f != null) f.Call(); });
        }
        public static void DORotate(GameObject obj,Vector3 vec,float time,int amplitude,float period)
        {
            obj.transform.DORotate(vec, time, RotateMode.FastBeyond360).SetEase(Ease.InSine);
            //obj.transform.DOLocalRotate(vec, time, RotateMode.FastBeyond360).SetEase(Ease.InCubic);
        }
        public static void DORotateQuaternion(GameObject obj, Vector3 vec, float time, int amplitude, float period)
        {
            obj.transform.DORotateQuaternion(new Quaternion(-1,-1,-1,-1), time);
        }
        public static void DoScale(GameObject obj, float f, float time)
        {
            obj.transform.DOScale(f, time);
        }

        public static void DoScaleY(GameObject obj, float f, float time)
        {
            obj.transform.DOScaleY(f, time);
        }
        public static void DoScale(GameObject obj, Vector3 vec, float time)
        {
            obj.transform.DOScale(vec, time);
        }

        public static void DoLocalMoveX(GameObject obj, float f, float time)
        {
            obj.transform.DOLocalMoveX(f, time);
        }

        public static void DoLocalMoveY(GameObject obj, float f, float time)
        {
            obj.transform.DOLocalMoveY(f, time);
        }

        public static void DoLocalMoveZ(GameObject obj, float f, float time)
        {
            obj.transform.DOLocalMoveZ(f, time);
        }
        public static void DOPunchScale(GameObject obj,Vector3 punch,float time,int vibrato,float elascity)
        {
            obj.transform.DOPunchScale(punch, time, vibrato, elascity);
        }
        public static void DOShakeScale(GameObject obj, Vector3 punch, float time, int vibrato, float randomness)
        {
            obj.transform.DOShakeScale(time, punch, vibrato, randomness,false);
        }
        public static void DOScaleYEase(GameObject obj, float scaleY, float time, int amplitude,float period,LuaFunction f)
        {
            //obj.transform.DOScaleY(scaleY, time).SetEase(Ease.Flash, amplitude, period);
            obj.transform.DOScaleY(scaleY, time).SetEase(Ease.OutBounce, amplitude, period).OnComplete(() => { if (f != null) f.Call(); });
        }

        public static void DOScaleEase(GameObject obj,Vector3 from ,Vector3 to, float time,Ease mode ,int amplitude, float period, LuaFunction f)
        {
            //Color curColor = text.color;
           // curColor.a = 1;
           // text.color = curColor;
            obj.transform.localScale = from;
            obj.transform.DOScale(to, time).SetEase(mode, amplitude, period).OnComplete(() => { if (f != null) f.Call(); });
        }

        public static void DOMoveEase(GameObject obj,Vector3 endValue,float time,bool snapping,Ease mode,int amplitude,float period,LuaFunction f)
        {
            obj.transform.DOMove(endValue, time, snapping).SetEase(mode, amplitude, period).OnComplete(() => { if (f != null) f.Call(); });
        }


        public static void DOMoveReturnEase(GameObject obj,Vector3 endValue, float time, bool snapping, Ease mode, int amplitude, float period, LuaFunction f)
        {
            //Vector3 startPosition = obj.transform.position;
            obj.transform.DOMove(endValue, time, snapping).SetEase(mode, amplitude, period).OnComplete(() => 
            {
                //obj.transform.position = startPosition;
                if (f != null) f.Call(); 
            });
        }
        public static void DOKill(GameObject obj)
        {
            obj.transform.DOKill();
        }
        public static void TweenerKill(Tween tweener)
        {
            if (tweener != null && tweener.IsPlaying())
                tweener.Kill();
        }
        public static float Distance(Vector3 from, Vector3 to)
        {
            return Vector3.Distance(from, to);
        }
        public static void DOFade(Text text, float fromValue,float endValue,float duration)
        {
            Color curColor = text.color;
            curColor.a = fromValue;
            text.color = curColor;
            Tween tweener = DOTween.ToAlpha(() => curColor, x => curColor = x, endValue, duration);
            tweener.onUpdate = () => { text.color = curColor; };
        }

        public static Tween DOAlpha(Text text, float fromValue, float endValue, float duration)
        {
            Color curColor = text.color;
            curColor.a = fromValue;
            text.color = curColor;
            Tween tweener= DOTween.ToAlpha(() => curColor, x => curColor = x, endValue, duration);
            tweener.onUpdate = () => { text.color = curColor; };
            return tweener;   
        }

        public static void DOTo(Transform target,Transform follow,float startValue, float endValue, float duration)
        {
            Vector3 followViewPos;
            Tweener tweener = DOTween.To(() => startValue, x => startValue = x, endValue, duration);
            tweener.onUpdate = () => 
            {
                followViewPos = Camera.main.WorldToScreenPoint(follow.position);
                target.position = followViewPos + new Vector3(0, startValue, 0); 
            };
        }
        //动态设置粒子rate数量
        public static void SetEmissionRate(GameObject pGo,float value)
        {
            ParticleSystem tParticleSystem = pGo.GetComponent<ParticleSystem>();
            ParticleSystem.EmissionModule emission = tParticleSystem.emission;
            emission.rate = new ParticleSystem.MinMaxCurve(value);
        }

        public static Vector3 DistanceToViewSide(Vector3 target)
        {
            Vector3 left = Camera.main.ViewportToWorldPoint(new Vector3(0.25f, 0, 0));
            Vector3 right = Camera.main.ViewportToWorldPoint(new Vector3(0.75f, 0, 0));
            //Vector3 middle = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, 0));
            Vector3 vPos = Camera.main.WorldToViewportPoint(target);
            Vector3 wPos = Camera.main.ViewportToWorldPoint(new Vector3(vPos.x,0,0));
            float x = 0;
            float y = 0;
            float z = (vPos.y - 0.20f) * 10;
            if (vPos.x < 0.25f )
            {
                x = (left.x - wPos.x) * 2;
                if (x < z)
                    return new Vector3(z-x, 0, z);
                return new Vector3(0, 0, x);
            }
            if (vPos.x > 0.75f)
            {
                y = (wPos.x - right.x) * 2 ;
                if (y<z)
                    return new Vector3(z, 0, z-y);
                return new Vector3(y, 0, 0);
            }
            if (vPos.y > 0.20f)
            {
                z = (vPos.y - 0.20f) * 10;
                return new Vector3(z, 0, z);
            }
            else
            {
                return new Vector3(0, 0, 0);
            }
            //return new Vector3(y+z, 0, x+z);
        }
    }
}