using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Runtime.InteropServices;//弹窗依赖
//webclient依赖
using System.Net;
using System.IO;
using UnityEngine.Networking;
using LitJson;

public class GameMain : MonoBehaviour
{
    public Button closeBtn;
    public Button cancelBtn;
    public Button confirmBtn;
    public Slider slider; //进度条
    public Text loadingProgress; //加载数
    public Text updateContent; //更新的内容，文本形式
    public Text downloadedSize; //已下载大小
    public Text fileSize; //文件大小
    // Start is called before the first frame update
    [Obsolete]
    void Start()
    {
        //获取绑定的按钮，并添加事件监听
        closeBtn.onClick.AddListener(CloseBtnClicked);
        cancelBtn.onClick.AddListener(CancelBtnClicked);
        confirmBtn.onClick.AddListener(ConfirmBtnClicked);
        
        StartCoroutine(GetData());
    }
    private void CloseBtnClicked()
    {
        UnityEngine.Debug.Log("CloseBtnClicked");
    }
    private void CancelBtnClicked()
    {
        UnityEngine.Debug.Log("CancelBtnClicked");
    }
    private void ConfirmBtnClicked()
    {
        UnityEngine.Debug.Log("ConfirmBtnClicked");
    }
    
    //请求更新接口（或服务器配置的更新json文件），拿到下载地址，下载安装
    [Obsolete]
    IEnumerator GetData()//Action action
    {
        yield return new WaitForSeconds(0.2f);
        //链接就不写了
        UnityWebRequest unityWebRequest = UnityWebRequest.Get("http://mas-x.oss-cn-hangzhou.aliyuncs.com/setup/Version.json");

        //UnityWebRequest unityWebRequest = UnityWebRequest.Get("http://222.222.185.194:8800/wy_eh_api/basic/apk/latest/version/");
        //UnityWebRequest unityWebRequest = UnityWebRequest.Get(jsonUrl);
        yield return unityWebRequest.SendWebRequest();
        if (!unityWebRequest.isNetworkError)
        {
            string data = unityWebRequest.downloadHandler.text;
            JsonData jd = JsonMapper.ToObject<JsonData>(data);
            //UnityEngine.Debug.Log(data);
            //下载地址
            string appDownload = (string)jd["data"]["appDownload"];
            //UnityEngine.Debug.Log(appDownload);

            //更新内容
            string updateContentStr = (string)jd["data"]["remark"];
            updateContent.text = updateContentStr;


            if (appDownload != null)
            {
                //调用下载
                //StartCoroutine(DownloadFile("http://222.222.185.194:8800/dfs/123/20230420/18/17/1/bddwyy-1.0.2.apk"));
                StartCoroutine(DownloadFile(appDownload));
            }
            //string versionnumber = (string)jd["data"]["versionNumber"];
            //UnityEngine.Debug.Log(versionnumber);
            //UnityEngine.Debug.Log(Application.version);
            //判断服务器版本号和当前应用版本号
            //if (versionnumber != Application.version)
            //{
            //    //调用下载
            //    //StartCoroutine(DownloadFile("http://222.222.185.194:8800/dfs/123/20230420/18/17/1/bddwyy-1.0.2.apk"));
            //    StartCoroutine(DownloadFile(appDownload));
            //}
        }
    }

    /// <summary>
    /// 下载
    /// </summary>
    /// <param name="url">下载的地址</param>
    /// <returns></returns>
    [Obsolete]
    IEnumerator DownloadFile(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        UnityEngine.Debug.Log(url);
        UnityEngine.Debug.Log(request);
        
        request.SendWebRequest();
        if (request.isHttpError || request.isNetworkError)
        {
            print("当前的下载发生错误" + request.error);
            yield break;
        }
        while (!request.isDone)
        {
            //print("当前的下载进度为：" + request.downloadProgress);
            //UnityEngine.Debug.Log(request.downloadProgress);
            slider.value = request.downloadProgress;
            loadingProgress.text = ((float)(slider.value) * 100).ToString("0.00") + "%"; //将进度条值转化成int类型加上百分比赋予加载数。
            //UnityEngine.Debug.Log("进度11111：" + request.downloadProgress);
            UnityEngine.Debug.Log("进度22222：" + loadingProgress.text);
            UnityEngine.Debug.Log(request.downloadedBytes);
            float temDownloadedSize = (request.downloadedBytes) / (1024 * 1024);
            float temFileSize = (request.downloadedBytes / request.downloadProgress) / (1024*1024);
            UnityEngine.Debug.Log(request.downloadedBytes);
            UnityEngine.Debug.Log(temFileSize);
            downloadedSize.text = ((float)(temDownloadedSize)).ToString("0.00") + "M";
            fileSize.text = ((float)(temFileSize)).ToString("0.00") + "M";
            yield return 0;
        }
        if (request.isDone)
        {
            //UnityEngine.Debug.Log("2");
            slider.value = 1;
            loadingProgress.text = "100%";
            //"http://222.222.185.194:8800/dfs/123/20230420/18/17/1/bddwyy-1.0.2.apk"
            //将下载的文件写入
            using (FileStream fs = new FileStream(Application.streamingAssetsPath + @"/4UGIGC.exe", FileMode.Create))
            {
                byte[] data = request.downloadHandler.data;
                fs.Write(data, 0, data.Length);
            }
            //UnityEngine.Debug.Log(Application.streamingAssetsPath + @"/4UGIGC.exe");

            //string str = System.Environment.CurrentDirectory;
            //UnityEngine.Debug.Log("############str:" + str);
            //string file1 = str + "\\dotNetFx45_Full_setup.exe";

            string file1 = Application.streamingAssetsPath + "/4UGIGC.exe";
            Process myprocess = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo(file1);
            myprocess.StartInfo = startInfo;
            myprocess.StartInfo.CreateNoWindow = false;
            myprocess.Start();

            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}

public class Messagebox
{
    [DllImport("User32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern int MessageBox(IntPtr handle, String message, String title, int type);
}



