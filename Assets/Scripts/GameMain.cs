using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Runtime.InteropServices;//��������
//webclient����
using System.Net;
using System.IO;
using UnityEngine.Networking;
using LitJson;

public class GameMain : MonoBehaviour
{
    public Button closeBtn;
    public Button cancelBtn;
    public Button confirmBtn;
    public Slider slider; //������
    public Text loadingProgress; //������
    public Text updateContent; //���µ����ݣ��ı���ʽ
    public Text downloadedSize; //�����ش�С
    public Text fileSize; //�ļ���С
    // Start is called before the first frame update
    [Obsolete]
    void Start()
    {
        //��ȡ�󶨵İ�ť��������¼�����
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
    
    //������½ӿڣ�����������õĸ���json�ļ������õ����ص�ַ�����ذ�װ
    [Obsolete]
    IEnumerator GetData()//Action action
    {
        yield return new WaitForSeconds(0.2f);
        //���ӾͲ�д��
        UnityWebRequest unityWebRequest = UnityWebRequest.Get("http://mas-x.oss-cn-hangzhou.aliyuncs.com/setup/Version.json");

        //UnityWebRequest unityWebRequest = UnityWebRequest.Get("http://222.222.185.194:8800/wy_eh_api/basic/apk/latest/version/");
        //UnityWebRequest unityWebRequest = UnityWebRequest.Get(jsonUrl);
        yield return unityWebRequest.SendWebRequest();
        if (!unityWebRequest.isNetworkError)
        {
            string data = unityWebRequest.downloadHandler.text;
            JsonData jd = JsonMapper.ToObject<JsonData>(data);
            //UnityEngine.Debug.Log(data);
            //���ص�ַ
            string appDownload = (string)jd["data"]["appDownload"];
            //UnityEngine.Debug.Log(appDownload);

            //��������
            string updateContentStr = (string)jd["data"]["remark"];
            updateContent.text = updateContentStr;


            if (appDownload != null)
            {
                //��������
                //StartCoroutine(DownloadFile("http://222.222.185.194:8800/dfs/123/20230420/18/17/1/bddwyy-1.0.2.apk"));
                StartCoroutine(DownloadFile(appDownload));
            }
            //string versionnumber = (string)jd["data"]["versionNumber"];
            //UnityEngine.Debug.Log(versionnumber);
            //UnityEngine.Debug.Log(Application.version);
            //�жϷ������汾�ź͵�ǰӦ�ð汾��
            //if (versionnumber != Application.version)
            //{
            //    //��������
            //    //StartCoroutine(DownloadFile("http://222.222.185.194:8800/dfs/123/20230420/18/17/1/bddwyy-1.0.2.apk"));
            //    StartCoroutine(DownloadFile(appDownload));
            //}
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="url">���صĵ�ַ</param>
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
            print("��ǰ�����ط�������" + request.error);
            yield break;
        }
        while (!request.isDone)
        {
            //print("��ǰ�����ؽ���Ϊ��" + request.downloadProgress);
            //UnityEngine.Debug.Log(request.downloadProgress);
            slider.value = request.downloadProgress;
            loadingProgress.text = ((float)(slider.value) * 100).ToString("0.00") + "%"; //��������ֵת����int���ͼ��ϰٷֱȸ����������
            //UnityEngine.Debug.Log("����11111��" + request.downloadProgress);
            UnityEngine.Debug.Log("����22222��" + loadingProgress.text);
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
            //�����ص��ļ�д��
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



