using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour {
    private ScoreRecorder scoreRecorder;
    private UserAction action;
    
	// Use this for initialization
	void Start () {
        action = Director.getInstance().getFirstController();
        scoreRecorder = ScoreRecorder.getInstance();
    }
	
	// Update is called once per frame
	void Update () {
        if (action.isGameOver())
        {
            action.movePlayer(0, 0);
            return;
        }
        if (Input.GetButtonDown("Jump"))
        {
            action.jump();
        }
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        action.movePlayer(h, v);
	}

    private void OnGUI()
    {
        // 游戏信息
        GUI.Box(new Rect(Screen.width - 260, 10, 250, 110), "操作方法");
        GUI.Label(new Rect(Screen.width - 245, 30, 250, 30), "上/下 键 : 向前跑/向后退");
        GUI.Label(new Rect(Screen.width - 245, 50, 250, 30), "左/右 键 : 向左转/向右转");
        GUI.Label(new Rect(Screen.width - 245, 70, 250, 30), "奔跑时按下空格键 : 跳跃");
        GUI.Label(new Rect(Screen.width - 245, 90, 250, 30), "停下的时候按下空格键 : 卖萌");

        string gameName = "躲避怪物";
        string gameRules = "操作任务躲避怪物的追捕，每躲过一只怪物分数加一";
        // 显示游戏名字和游戏规则信息
        if (GUI.RepeatButton(new Rect(10, 10, 120, 20), gameName))
        {
            GUI.TextArea(new Rect(10, 40, Screen.width - 20, Screen.height / 2), gameRules);
        }
        // 重开游戏按钮
        else if (GUI.Button(new Rect(140, 10, 70, 20), "重新开始"))
        {
            action.reStart();
        }

        if (action.isGameOver())
        {
            GUI.Box(new Rect(Screen.width - 340, 37, 70, 23), "游戏结束");
        }

        GUI.Box(new Rect(Screen.width - 340, 10, 70, 23), "分数：" + scoreRecorder.getScore());
    }
}
