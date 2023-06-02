using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagBoxCtroller : MonoBehaviour {

    public static MessagBoxCtroller _MBC;

    public Button okBtn;
    public Button cancleBtn;
    public Button closeBtn;
    public Text message;
    public GameObject MessageBox;

    private Action<int> actions;

    //左侧位置
    private Vector3 leftPosition = new Vector2(-167,-135);
    private Vector3 middlePosition = new Vector3(15,-135);

    // Use this for initialization
    void Awake () {
        _MBC = this;
        okBtn.onClick.AddListener(delegate () { this.OnClick(okBtn.name); });
        cancleBtn.onClick.AddListener(delegate () { this.OnClick(cancleBtn.name); });
        closeBtn.onClick.AddListener(delegate () { this.OnClick(closeBtn.name); });
    }

    public void show(string messageStr,Action<int> aciton,bool isShowAll =true)
    {
        MessageBox.SetActive(true);
        message.text = messageStr;
        actions = aciton;
        if (!isShowAll)
        {
            showOneButton();
        }
        else
        {
            showAllButton();
        }
    }

    private void showOneButton()
    {
        setButton(true,false);
        setOkButtonPositionAndName("知道了", middlePosition);
    }

    private void showAllButton()
    {
        setButton(true, true);
        setOkButtonPositionAndName("确定", leftPosition);
    }

    private void setButton(bool okButton,bool cancleButton)
    {
        okBtn.gameObject.SetActive(okButton);
        cancleBtn.gameObject.SetActive(cancleButton);
    }

    private void setOkButtonPositionAndName(string str,Vector3 position)
    {
        okBtn.GetComponentInChildren<Text>().text = str;
        okBtn.transform.localPosition = position;
    }

    private void OnClick(string name)
    {
        switch (name)
        {
            case "OKBtn":
                if (actions != null)
                {
                    actions(0);
                } 
                break;
            case "CancleBtn":
                if (actions != null)
                {
                    actions(1);
                }
                break;
            case "CloseBtn":
                if (actions != null)
                {
                    actions(2);
                }
                break;
         
        }
        MessageBox.SetActive(false);
    }
	
	
}
