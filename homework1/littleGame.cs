using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class littleGame : MonoBehaviour {
    char markForPlayer1 = '@';
    char markForPlayer2 = '#';
    int winner;//0表示没有人获胜，1表示玩家1获胜，2表示玩家2获胜；
    int turn;//表示当前是那个玩家的回合,1表示玩家1的回合，2表示玩家2的回合；
    char[,] state = new char[3, 3];//使用字符表示每个方格的状态；

    int labelPositionX = 420;
    int labelPositionY = 360;
    int labelWidth = 80;
    int labelHeight = 40;

    int resetPositionX = 440;
    int resetPositionY = 520;
    int resetWidth = 40;
    int resetHeight = 40;

    int buttonPositionX = 400;
    int buttonPositionY = 400;
    int buttonWidth = 40;
    int buttonHeight = 40;

    // Use this for initialization
    void Start () {
        //初始化：
        winner = 0;
        turn = 1;
        for(int i =0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                state[i,j] = ' ' ;
            }
        }
	}
    //检查是否有玩家获胜的函数：
    void getWinner()
    {
        for (int i = 0; i < 3; i++)
        {
            if (state[i,0] == state[i, 1]&& state[i, 0] == state[i, 2]&&state[i,0]!=' ')
            {
                if (state[i,0] == markForPlayer1)
                {
                    winner = 1;
                } else
                {
                    winner = 2;
                }
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (state[0, i] == state[1, i] && state[0, i] == state[2, i] && state[0, i] != ' ')
            {
                if (state[0,i] == markForPlayer1)
                {
                    winner = 1;
                }
                else
                {
                    winner = 2;
                }
            }
        }
        if(state[0,0] == state[2,2]&& state[0, 0] == state[1,1] && state[1, 1] != ' ')
        {
            if(state[1,1]==markForPlayer1)
            {
                winner = 1;
            } else
            {
                winner = 2;
            }
        }
        if (state[0, 2] == state[1, 1] && state[1, 1] == state[2, 0] && state[1, 1] != ' ')
        {
            if (state[1, 1] == markForPlayer1)
            {
                winner = 1;
            }
            else
            {
                winner = 2;
            }
        }
    }
    //每一帧都刷新屏幕：显示最新的状态
    private void OnGUI()
    {
        //先检查有没有玩家获胜：
        getWinner();
        if(winner != 0)
        {
            if (winner == 1)
            {
                GUI.Label(new Rect(labelPositionX,labelPositionY-labelHeight,labelWidth,labelHeight), " Player1 win!");
            } else
            {
                GUI.Label(new Rect(labelPositionX, labelPositionY - labelHeight, labelWidth, labelHeight), " Player2 win!");
            }
        }

        //显示现在是谁的回合
        if (turn == 1)
        {
            GUI.Label(new Rect(labelPositionX, labelPositionY, labelWidth, labelHeight), "Player1's turn");
        }
        else
        {
            GUI.Label(new Rect(labelPositionX, labelPositionY, labelWidth, labelHeight), "Player2's turn");
        }

        //显示整个游戏主界面
        for (int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                if (state[i,j] == ' ')
                {
                    if (GUI.Button(new Rect(buttonPositionX + i * buttonWidth, buttonPositionY + j * buttonHeight, buttonWidth, buttonHeight), "" + state[i, j])&&winner==0)
                    {
                        state[i, j] = (turn==1?markForPlayer1:markForPlayer2);
                        turn = 3 - turn;//交换回合
                    }
                } else
                {
                    GUI.Button(new Rect(buttonPositionX + i * buttonWidth, buttonPositionY + j * buttonHeight, buttonWidth, buttonHeight), "" + state[i, j]);
                }
               
            }
        }

        //玩家可以重玩
        if (GUI.Button(new Rect(resetPositionX, resetPositionY, resetWidth, resetHeight), "reset"))
        {
            Start();
        }
    }
}
