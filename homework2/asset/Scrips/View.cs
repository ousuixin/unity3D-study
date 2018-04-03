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
        action = SSDirector.getInstance () as IUserAction;
    }

    void OnGUI()
    {
        width = Screen.width / 12;
        height = Screen.height / 12;
        print(mySSDirector.state);
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
            }
            else if (mySSDirector.state == State.BOAT_STOP_ON_THE_LEFT_SHORE || mySSDirector.state == State.BOAT_STOP_ON_THE_RIGHT_SHORE)
            {
                if (GUI.Button(new Rect(castw(2f), casth(6f), width, height), "Go"))
                {
                    action.moveBoat();
                }
                if (GUI.Button(new Rect(castw(10.5f), casth(4f), width, height), "On"))
                {
                    action.devilLeftshoreGoOnboard();
                }
                if (GUI.Button(new Rect(castw(4.3f), casth(4f), width, height), "On"))
                {
                    action.priestLeftshoreGoOnboard();
                }
                if (GUI.Button(new Rect(castw(1.1f), casth(4f), width, height), "On"))
                {
                    action.devilRightshoreGoOnboard();
                }
                if (GUI.Button(new Rect(castw(1.3f), casth(4f), width, height), "On"))
                {
                    action.priestRightshoreGoOnboard();
                }
                if (GUI.Button(new Rect(castw(2.5f), casth(1.3f), width, height), "Off"))
                {
                    action.LeftshoreDisembark();
                }
                if (GUI.Button(new Rect(castw(1.7f), casth(1.3f), width, height), "Off"))
                {
                    action.RightshoreDisembark();
                }
            }
        }
    }
}