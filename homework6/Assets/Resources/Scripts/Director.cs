using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : System.Object
{
    private static Director _instance;
    private FirstController firstSceneController;

    public static Director getInstance()
    {
        if (_instance == null)
        {
            _instance = new Director();
        }
        return _instance;
    }

    public FirstController getFirstController()
    {
        return firstSceneController;
    }

    internal void setFirstController(FirstController gom)
    {
        if (null == firstSceneController)
        {
            firstSceneController = gom;
        }
    }
}
