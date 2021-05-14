using UnityEngine;
using System.Collections;

public class NotiConst
{
    /// <summary>
    /// Controller层消息通知
    /// </summary>
    public const string START_UP = "StartUp";                       //启动框架
    public const string DISPATCH_MESSAGE = "DispatchMessage";       //派发信息

    /// <summary>
    /// View层消息通知
    /// </summary>
    public const string UPDATE_MESSAGE = "UpdateMessage";           //更新消息
    public const string UPDATE_EXTRACT = "UpdateExtract";           //更新解包

    public const string EXTRACT_FILE_NAME = "EXTRACT_FILE_NAME";    //解包文件名
    public const string EXTRACT_FINISH_ONE = "EXTRACT_FINISH_ONE";  //解包完一个文件
    public const string EXTRACT_ALL_COUNT = "EXTRACT_ALL_COUNT";    //解包总文件数

    public const string UPDATE_SPEED = "UPDATE_SPEED";              //下载速度
    public const string UPDATE_FILE_NAME = "UPDATE_FILE_NAME";      //下载文件名
    public const string UPDATE_FINISH_ONE = "UPDATE_FINISH_ONE";    //下载完一个文件
    public const string UPDATE_ALL_COUNT = "UPDATE_ALL_COUNT";      //下载总文件数
    public const string UPDATE_CUR_COUNTLENGTH = "UPDATE_CUR_COUNTLENGTH";   //当前下载文件长度
    public const string UPDATE_COUNTLENGTH = "UPDATE_COUNTLENGTH";   //当前下载文件总长度
    public const string UPDATE_ALL_COUNTLENGTH = "UPDATE_ALL_COUNTLENGTH";   //下载总文件长度
    public const string UPDATE_DOWNLOAD = "UpdateDownload";         //更新下载
    public const string UPDATE_PROGRESS = "UpdateProgress";         //更新进度

    public const string THREAD_EXTRACT = "THREAD_EXTRACT";          //线程解包
    public const string THREAD_DOWNLOAD = "THREAD_DOWNLOAD";        //线程下载

    public const string CHECK_UPDATE = "CHECK_UPDAT";         //检查更新
}
