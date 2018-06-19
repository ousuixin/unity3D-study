using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IControler;

public class View : MonoBehaviour
{

    SSDirector mySSDirector;
    IUserAction action;

    float width, height;

    float castw(float scale)
    {
        return (Screen.width - width) / scale;
    }

    float casth(float scale)
    {
        return (Screen.height - height) / scale;
    }

    void Start()
    {
        mySSDirector = SSDirector.getInstance();
        action = SSDirector.getInstance().getCurrentSceneController() as IUserAction;
    }

    void OnGUI()
    {
        width = Screen.width / 12;//所有按钮大小设置为屏幕1/12
        height = Screen.height / 12;
        // print(mySSDirector.state);
        //通知玩家胜利/失败，并点击重新开始
        if (mySSDirector.state == State.WIN)
        {
            if (GUI.Button(new Rect(castw(2f), casth(6f), width, height), "你赢了!\n(click here to restart)"))
            {
                action.restart();
                action.restart();
            }
        }
        else if (mySSDirector.state == State.LOSE)
        {
            if (GUI.Button(new Rect(castw(2f), casth(6f), width, height), "你输了!\n(click here to restart)"))
            {
                action.restart();
                action.restart();
            }
        }
        else
        {
            if (GUI.RepeatButton(new Rect(10, 10, 120, 20), mySSDirector.getCurrentSceneController().gameName))
            {
                GUI.TextArea(new Rect(10, 40, Screen.width - 20, Screen.height / 2), mySSDirector.getCurrentSceneController().gameRules);
            }//显示游戏名字和游戏规则信息
            else if (GUI.Button(new Rect(140, 10, 60, 20), "重玩"))
            {
                action.restart();
                action.restart();
            }//重开游戏按钮
            else if (GUI.Button(new Rect(210, 10, 60, 20), "提示"))
            {
                action.prompt();
            }
        }
    }

    void Update()
    {
        if (mySSDirector.state == State.BOAT_STOP_ON_THE_LEFT_SHORE || mySSDirector.state == State.BOAT_STOP_ON_THE_RIGHT_SHORE)
        {
            action.clickOne();
        }   
    }
}