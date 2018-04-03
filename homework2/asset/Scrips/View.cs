using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IControler;

public class View : MonoBehaviour
{

    SSDirector mySSDirector;
    IUserAction action;

    float width, height;
    //获取屏幕除去按钮大小以后的分位值，使界面工整
    float castw(float scale)
    {
        return (Screen.width - width) / scale;
    }

    //获取屏幕除去按钮大小以后的分位值，使界面工整
    float casth(float scale)
    {
        return (Screen.height - height) / scale;
    }

    void Start()
    {
        mySSDirector = SSDirector.getInstance();
        action = SSDirector.getInstance () as IUserAction;
    }

    void OnGUI()
    {
        width = Screen.width / 12;//所有按钮大小设置为屏幕1/12
        height = Screen.height / 12;
        print(mySSDirector.state);
        //通知玩家胜利/失败，并点击重新开始
        if (mySSDirector.state == State.WIN)
        {
            if (GUI.Button(new Rect(castw(2f), casth(6f), width, height), "Win!\n(click here to restart)"))
            {
                action.restart();
            }
        }
        else if (mySSDirector.state == State.LOSE)
        {
            if (GUI.Button(new Rect(castw(2f), casth(6f), width, height), "Lose!\n(click here to restart)"))
            {
                action.restart();
            }
        }
        else
        {
            if (GUI.RepeatButton(new Rect(10, 10, 120, 20), mySSDirector.getBaseCode().gameName))
            {
                GUI.TextArea(new Rect(10, 40, Screen.width - 20, Screen.height / 2), mySSDirector.getBaseCode().gameRules);
            }//显示游戏名字和游戏规则信息
            else if (GUI.Button(new Rect(140, 10, 60,20), "Restart"))
            {
                mySSDirector.restart();
            }//重开游戏按钮
            else if (mySSDirector.state == State.BOAT_STOP_ON_THE_LEFT_SHORE || mySSDirector.state == State.BOAT_STOP_ON_THE_RIGHT_SHORE)
            {
                if (GUI.Button(new Rect(castw(2f), casth(6f), width, height), "Go"))
                {
                    action.moveBoat();
                }//点击go按钮开船
                if (GUI.Button(new Rect(castw(10.5f), casth(4f), width, height), "On"))
                {
                    action.devilLeftshoreGoOnboard();
                }//点击左岸左侧的on使左侧恶魔上船
                if (GUI.Button(new Rect(castw(4.3f), casth(4f), width, height), "On"))
                {
                    action.priestLeftshoreGoOnboard();
                }//点击左岸右侧的on使左侧牧师上船
                if (GUI.Button(new Rect(castw(1.1f), casth(4f), width, height), "On"))
                {
                    action.devilRightshoreGoOnboard();
                }//点击右岸右侧的on使右侧恶魔上船
                if (GUI.Button(new Rect(castw(1.3f), casth(4f), width, height), "On"))
                {
                    action.priestRightshoreGoOnboard();
                }//点击右岸左侧的on使右侧牧师上船
                if (GUI.Button(new Rect(castw(2.5f), casth(1.3f), width, height), "Off"))
                {
                    action.LeftshoreDisembark();
                }//点击左侧按钮使船左侧人下船
                if (GUI.Button(new Rect(castw(1.7f), casth(1.3f), width, height), "Off"))
                {
                    action.RightshoreDisembark();
                }//点击右侧按钮使船右侧人下船
            }
        }
    }
}
