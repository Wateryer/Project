using UnityEngine;
using LuaFramework;
using System.Collections.Generic;
using UnityEngine.UI;

public class AppView : View {
    private string message;

    string extractFileName;
    int extractNowCount = 0;
    int extractAllCount = 0;

    string updateFileName;
    int updateNowCount = 0;
    int updateAllCount = 0;
    string updateSpeed;
    string countLength;
    float currentCountLength=0;
    float curCountLength = 0;

    //更新文本
    private Text text;
    private Slider loadProgress;
    private Text progressText;
    ///<summary>
    /// 监听的消息
    ///</summary>
    List<string> MessageList {
        get {
            return new List<string>()
            { 
                NotiConst.EXTRACT_FILE_NAME,
                NotiConst.EXTRACT_FINISH_ONE,
                NotiConst.EXTRACT_ALL_COUNT,

                NotiConst.UPDATE_SPEED,
                NotiConst.UPDATE_FILE_NAME,
                NotiConst.UPDATE_FINISH_ONE,
                NotiConst.UPDATE_ALL_COUNT,
                NotiConst.UPDATE_COUNTLENGTH,
                NotiConst.UPDATE_ALL_COUNTLENGTH,
                NotiConst.UPDATE_CUR_COUNTLENGTH,

                NotiConst.UPDATE_MESSAGE,
                NotiConst.UPDATE_EXTRACT,
                NotiConst.UPDATE_DOWNLOAD,
                NotiConst.UPDATE_PROGRESS,

                NotiConst.CHECK_UPDATE
            };
        }
    }

    void Awake() {
        RemoveMessage(this, MessageList);
        RegisterMessage(this, MessageList);

        text = GameObject.Find("Text").GetComponent<Text>();
        loadProgress = GameObject.Find("Slider").GetComponent<Slider>();
        progressText = GameObject.Find("ProgressText").GetComponent<Text>();
    }

    /// <summary>
    /// 处理View消息
    /// </summary>
    /// <param name="message"></param>
    public override void OnMessage(IMessage message) {
        string name = message.Name;
        object body = message.Body;
        this.message = message.Name;
        switch (name) {
            case NotiConst.EXTRACT_FILE_NAME:
                extractFileName = body.ToString();
                break;
            case NotiConst.EXTRACT_FINISH_ONE:
                extractNowCount++;
                break;
            case NotiConst.EXTRACT_ALL_COUNT:
                extractAllCount = (int)body;
                break;

            case NotiConst.UPDATE_SPEED:
                updateSpeed = body.ToString();
                break;
            case NotiConst.UPDATE_FILE_NAME:
                updateFileName = body.ToString();
                break;
            case NotiConst.UPDATE_FINISH_ONE:
                updateNowCount++;
                break;
            case NotiConst.UPDATE_ALL_COUNT:
                updateAllCount = (int)body;
                break;
            case NotiConst.UPDATE_COUNTLENGTH:
                currentCountLength = float.Parse(body.ToString());
                break;
            case NotiConst.UPDATE_ALL_COUNTLENGTH:
                countLength = body.ToString();
                break;
            case NotiConst.UPDATE_CUR_COUNTLENGTH:
                curCountLength += float.Parse(body.ToString());
                break;

            case NotiConst.UPDATE_MESSAGE:      //更新消息
                UpdateMessage(body.ToString());
                break;
            case NotiConst.CHECK_UPDATE:      //检查更新
                CheckUpdateMessage(body.ToString());
                break;
            //case NotiConst.UPDATE_EXTRACT:      //更新解压
            //    UpdateExtract(body.ToString());
            //break;
            //case NotiConst.UPDATE_DOWNLOAD:     //更新下载
            //    UpdateDownload(body.ToString());
            //break;
            //case NotiConst.UPDATE_PROGRESS:     //更新下载进度
            //    UpdateProgress(body.ToString());
            //break;
        }
    }

    public void UpdateMessage(string data)
    {
        this.message = data;
    }
    public void CheckUpdateMessage(string data)
    {
        this.message = data;
    }

    //public void UpdateExtract(string data) {
    //    this.message = data;
    //}

    //public void UpdateDownload(string data) {
    //    this.message = data;
    //}

    //public void UpdateProgress(string data) {
    //    this.message = data;
    //}

    void OnGUI() {
        //GUI.Label(new Rect(10, 120, 960, 50), message);
        //GUI.Label(new Rect(10, 0, 500, 50), "(1) 单击 \"Lua/Gen Lua Wrap Files\"。");
        //GUI.Label(new Rect(10, 20, 500, 50), "(2) 运行Unity游戏");
        //GUI.Label(new Rect(10, 40, 500, 50), "PS: 清除缓存，单击\"Lua/Clear LuaBinder File + Wrap Files\"。");
        //GUI.Label(new Rect(10, 60, 900, 50), "PS: 若运行到真机，请设置Const.DebugMode=false，本地调试请设置Const.DebugMode=true");
        //GUI.Label(new Rect(10, 80, 500, 50), "PS: 加Unity+ulua技术讨论群：>>341746602");

        if (string.IsNullOrEmpty(message)) return;
        text.text = message;
        if (message.StartsWith("EXTRACT_"))
        {
            //GUILayout.Label("正在解包的文件：" + extractFileName);
            //GUILayout.Label("当前解包数：" + extractNowCount);
            //GUILayout.Label("总的解包数：" + extractAllCount);
            text.text = string.Format("正在解包[{0}/{1}]：{2}", extractNowCount, extractAllCount, extractFileName);
            loadProgress.value = (float)extractNowCount / extractAllCount;
        }
        else if (message.StartsWith("UPDATE_"))
        {
            //GUILayout.Label("正在下载文件：" + updateFileName);
            //GUILayout.Label("当前下载数：" + updateNowCount);
            //GUILayout.Label("总的下载数：" + updateAllCount);
            //GUILayout.Label("下载速度：" + updateSpeed);
            //GUILayout.Label("下载总量：" +System.Math.Round( curCountLength,2) + "/" + countLength);
            text.text = string.Format("正在下载[ {0} ]：{1}", updateSpeed, updateFileName);
            loadProgress.value = curCountLength /float.Parse(countLength);
            progressText.text = System.Math.Round(curCountLength, 2) + "kb / " + countLength+"kb";
        }
    }
}
