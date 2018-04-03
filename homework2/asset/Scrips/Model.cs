using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IControler;

public class Model : MonoBehaviour {
    SSDirector myDirector;

    float speed = 10;//船速
    GameObject[] BoatAndCreature = new GameObject[2];//包括人和船的集合体，最多可以载三个对象：船（必须载）+人+人
    GameObject boat;//船
    //左岸/右岸的魔鬼/牧师
    Queue<GameObject> priestOnLeftshore = new Queue<GameObject>();
    Queue<GameObject> priestOnRightshore = new Queue<GameObject>();
    Queue<GameObject> devilOnLeftshore = new Queue<GameObject>();
    Queue<GameObject> devilOnRightshore = new Queue<GameObject>();

    //距离相关定义
    Vector3 LeftShorePos = new Vector3(-12, 0, 0);
    Vector3 RightShorePos = new Vector3(12, 0, 0);
    Vector3 LeftBoatPos = new Vector3(-4, 0, 0);
    Vector3 RightBoatPos = new Vector3(4, 0 ,0);

    float distanceBetweenObj = 1;//相邻两个对象之间的距离
    Vector3 LeftPriestPos = new Vector3(-8, 2.5f, 0);
    Vector3 RightPriestPos = new Vector3(6, 2.5f, 0);
    Vector3 LeftDevilPos = new Vector3(-12, 2.5f, 0);
    Vector3 RightDevilPos = new Vector3(10, 2.5f, 0);

    // Use this for initialization
    void Start () {
        myDirector = SSDirector.getInstance();
        myDirector.setGenGameObject(this);
        loadSrc();
	}
	
	// Update is called once per frame
	void Update () {
        setCharacterPositions(priestOnLeftshore, LeftPriestPos);
        setCharacterPositions(priestOnRightshore, RightPriestPos);
        setCharacterPositions(devilOnLeftshore, LeftDevilPos);
        setCharacterPositions(devilOnRightshore, RightDevilPos);

        if (myDirector.state == State.BOAT_MOVING_FROM_LEFT_TO_RIGHT)
        {
            boat.transform.position = Vector3.MoveTowards(boat.transform.position, RightBoatPos, speed * Time.deltaTime);
            if (boat.transform.position == RightBoatPos)
            {
                myDirector.state = State.BOAT_STOP_ON_THE_RIGHT_SHORE;
            }
        }
        else if (myDirector.state == State.BOAT_MOVING_FROM_RIGHT_TO_LEFT)
        {
            boat.transform.position = Vector3.MoveTowards(boat.transform.position, LeftBoatPos, speed * Time.deltaTime);
            if (boat.transform.position == LeftBoatPos)
            {
                myDirector.state = State.BOAT_STOP_ON_THE_LEFT_SHORE;
            }
        }
        else check();
    }

    //检查游戏是否结束
    void check()
    {
        //玩家获胜结束游戏：在右边出现全部三个法师和三个牧师：
        if(priestOnRightshore.Count == 3&&devilOnRightshore.Count == 3)
        {
            myDirector.state = State.WIN;
            return;
        }
        //玩家失败结束游戏：在任意时刻在左边或者右边的恶魔超过牧师：
        int a = 0, b = 0;
        int c = 0, d = 0, e = 0, f = 0;

        for (int i = 0; i < 2; ++i)
        {
            if (BoatAndCreature[i]!=null)
            {
                if (BoatAndCreature[i].tag == "Priest")
                {
                    a++;
                } else if (BoatAndCreature[i].tag == "Devil")
                {
                    b++;
                }
            }
        }
        if (myDirector.state == State.BOAT_STOP_ON_THE_LEFT_SHORE)
        {
            c = priestOnLeftshore.Count + a;
            d = devilOnLeftshore.Count + b;
            e = priestOnRightshore.Count;
            f = devilOnRightshore.Count;
        }else if (myDirector.state == State.BOAT_STOP_ON_THE_RIGHT_SHORE)
        {

            c = priestOnLeftshore.Count;
            d = devilOnLeftshore.Count;
            e = priestOnRightshore.Count + a;
            f = devilOnRightshore.Count + b;
        }
        //存在牧师而且牧师比恶魔少就输了
        if (c!=0&&c<d||e!=0&&e<f)
        {
            myDirector.state = State.LOSE;
        }
    }

    void loadSrc()
    {
        // shore  
        Instantiate(Resources.Load("prefabs/Shore"), LeftShorePos, Quaternion.identity);
        Instantiate(Resources.Load("prefabs/Shore"), RightShorePos, Quaternion.identity);
        // boat  
        boat = Instantiate(Resources.Load("prefabs/Boat"), LeftBoatPos, Quaternion.identity) as GameObject;
        // priests & devils  
        for (int i = 0; i < 3; ++i)
        {
            priestOnLeftshore.Enqueue(Instantiate(Resources.Load("prefabs/Priest")) as GameObject);
            devilOnLeftshore.Enqueue(Instantiate(Resources.Load("prefabs/Devil")) as GameObject);
        }
    }

    void setCharacterPositions(Queue<GameObject> queue, Vector3 pos)
    {
        GameObject[] array = queue.ToArray();
        for (int i = 0; i < queue.Count; ++i)
        {
            array[i].transform.position = new Vector3(pos.x + distanceBetweenObj * i, pos.y, pos.z);
        }
    }

    //对象上船---仅针对船，不针对对象
    void GoOnBoard(GameObject obj)
    {
        //先看看船的容量
        int temp = 0;
        for (int i = 0; i < 2; i++)
        {
            if (BoatAndCreature[i] == null)
            {
                temp++;
            }
        }
        //还有容量就让人上船
        if (temp != 0)
        {
            obj.transform.parent = boat.transform;
            if (BoatAndCreature[0] == null)
            {
                BoatAndCreature[0] = obj;
                obj.transform.localPosition = new Vector3(-0.3f, 1.2f, 0);
            }
            else
            {
                BoatAndCreature[1] = obj;
                obj.transform.localPosition = new Vector3(0.3f, 1.2f, 0);
            }
        }
    }
    //对象上船----针对对象，不针对船：
    public void leftShorePriestGoOnBoat()
    {
        int temp = 0;
        for (int i = 0; i < 2; i++)
        {
            if (BoatAndCreature[i] == null)
            {
                temp++;
            }
        }
        //船停在左边&&船上有空位&&左边岸上还有牧师
        if (priestOnLeftshore.Count != 0 && temp != 0 && myDirector.state == State.BOAT_STOP_ON_THE_LEFT_SHORE)
            GoOnBoard(priestOnLeftshore.Dequeue());
    }

    public void rightShorePriestGoOnBoat()
    {
        int temp = 0;
        for (int i = 0; i < 2; i++)
        {
            if (BoatAndCreature[i] == null)
            {
                temp++;
            }
        }
        //船停在右边&&船上有空位&&右边岸上还有牧师
        if (priestOnRightshore.Count != 0 && temp != 0 && myDirector.state == State.BOAT_STOP_ON_THE_RIGHT_SHORE)
            GoOnBoard(priestOnRightshore.Dequeue());
    }

    public void leftShoreDevilGoOnBoat()
    {
        int temp = 0;
        for (int i = 0; i < 2; i++)
        {
            if (BoatAndCreature[i] == null)
            {
                temp++;
            }
        }
        //船停在左边&&船上有空位&&左边岸上还有恶魔
        if (devilOnLeftshore.Count != 0 && temp != 0 && myDirector.state == State.BOAT_STOP_ON_THE_LEFT_SHORE)
            GoOnBoard(devilOnLeftshore.Dequeue());
    }

    public void rightShoreDevilGoOnBoat()
    {
        int temp = 0;
        for (int i = 0; i < 2; i++)
        {
            if (BoatAndCreature[i] == null)
            {
                temp++;
            }
        }
        //船停在右边&&船上有空位&&右边岸上还有恶魔
        if (devilOnRightshore.Count != 0 && temp != 0 && myDirector.state == State.BOAT_STOP_ON_THE_RIGHT_SHORE)
            GoOnBoard(devilOnRightshore.Dequeue());
    }

    //对象下船
    public void GoOffBoard(int index)
    {
        if (BoatAndCreature[index] != null)
        {
            BoatAndCreature[index].transform.parent = null;
            if (myDirector.state == State.BOAT_STOP_ON_THE_RIGHT_SHORE)
            {
                if (BoatAndCreature[index].tag == "Priest")
                {
                    priestOnRightshore.Enqueue(BoatAndCreature[index]);
                }
                else if (BoatAndCreature[index].tag == "Devil")
                {
                    devilOnRightshore.Enqueue(BoatAndCreature[index]);
                }
            }
            else if (myDirector.state == State.BOAT_STOP_ON_THE_LEFT_SHORE)
            {
                if (BoatAndCreature[index].tag == "Priest")
                {
                    priestOnLeftshore.Enqueue(BoatAndCreature[index]);
                }
                else if (BoatAndCreature[index].tag == "Devil")
                {
                    devilOnLeftshore.Enqueue(BoatAndCreature[index]);
                }
            }
        }
        BoatAndCreature[index] = null;
    }
    //开船
    public void moveBoat()
    {
        //先看看船有多少人
        int temp = 0;
        for (int i = 0; i < 2; i++)
        {
            if (BoatAndCreature[i] != null)
            {
                temp++;
            }
        }
        //只要船不空就可以开船，开船以后马上转变状态
        if (temp != 0)
        {
            if (myDirector.state == State.BOAT_STOP_ON_THE_LEFT_SHORE)
            {
                myDirector.state = State.BOAT_MOVING_FROM_LEFT_TO_RIGHT;
            }
            else if (myDirector.state == State.BOAT_STOP_ON_THE_RIGHT_SHORE)
            {
                myDirector.state = State.BOAT_MOVING_FROM_RIGHT_TO_LEFT;
            }
        }
    }


}
