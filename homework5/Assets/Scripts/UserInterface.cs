using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour {
    IUserAction action;
    //这里0表示动力学模式，1表示运动学模式，-1表示游戏还没有开始
    private int mode = -1;
    // Use this for initialization
    void Start () {
        action = SSDirector.getInstance().getCurrentSceneController() as IUserAction;
    }

    // Update is called once per frame
    void Update() {
        if (SSDirector.getInstance().getGameStatus().canStart() && mode != -1)
        {
            action.clickOne();
            action.spacePress();
        }
    }

    private void OnGUI()
    {
        if (mode == -1)
        {
            if (GUI.Button(new Rect(400, 200, 800, 200), "use PhysicActionManager")) {
                action.startGame(0);
                mode = 0;
            }
            if (GUI.Button(new Rect(400, 400, 800, 200), "use ActionManager"))
            {
                action.startGame(1);
                mode = 1;
            }
        }
    }
}
