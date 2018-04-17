using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour {
    IUserAction action;
    // Use this for initialization
    void Start () {
        action = SSDirector.getInstance().getCurrentSceneController() as IUserAction;
    }
	
	// Update is called once per frame
	void Update () {
        if (SSDirector.getInstance().getGameStatus().canStart())
        {
            action.clickOne();
            action.spacePress();
        }
    }
}
