using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreRecorder {
    public static ScoreRecorder _instance;

    private int Score = 0;
    public void addScore()
    {
        Score++;
    }
    public int getScore()
    {
        return Score;
    }
    public void setScore(int num)
    {
        Score = num;
    }
    public static ScoreRecorder getInstance()
    {
        if (_instance == null)
        {
            _instance = new ScoreRecorder();
        }
        return _instance;
    }
}
