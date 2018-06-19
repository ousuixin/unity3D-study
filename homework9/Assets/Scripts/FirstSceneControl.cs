using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IControler;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System;

namespace IControler
{
    public class FirstSceneControl : MonoBehaviour, IUserAction
    {
        public FirstSSActionManager actionManager;

        public string gameName = "牧师与魔鬼";
        public string gameRules = "你要运用智慧帮助3个" +
            "牧师（方块）和3个魔鬼（圆球）渡河。船最多可以载2名游" +
            "戏角色。船上有游戏角色时，你才可以点击这个船，让船移动到" +
            "对岸。当有一侧岸的魔鬼数多余牧师数时（包括船上的魔鬼和牧师" +
            "），魔鬼就会失去控制，吃掉牧师（如果这一侧没有牧师则不会失" +
            "败），游戏失败。当所有游戏角色都上到对岸时，游戏胜利。";

        SSDirector myDirector;

        public float speed = 10;//船速
        GameObject[] BoatAndCreature = new GameObject[2];//包括人和船的集合体，最多可以载三个对象：船（必须载）+人+人
        GameObject boat;//船
                        //左岸/右岸的魔鬼/牧师
        Queue<GameObject> priestOnLeftshore = new Queue<GameObject>();
        Queue<GameObject> priestOnRightshore = new Queue<GameObject>();
        Queue<GameObject> devilOnLeftshore = new Queue<GameObject>();
        Queue<GameObject> devilOnRightshore = new Queue<GameObject>();

        //距离相关定义
        public Vector3 LeftShorePos = new Vector3(-12, 0, 0);
        public Vector3 RightShorePos = new Vector3(12, 0, 0);
        public Vector3 LeftBoatPos = new Vector3(-4, 0, 0);
        public Vector3 RightBoatPos = new Vector3(4, 0, 0);
        public float distanceBetweenObj = 1;//相邻两个对象之间的距离
        public Vector3 LeftPriestPos = new Vector3(-6, 2.5f, 0);
        public Vector3 RightPriestPos = new Vector3(6, 2.5f, 0);
        public Vector3 LeftDevilPos = new Vector3(-9, 2.5f, 0);
        public Vector3 RightDevilPos = new Vector3(9, 2.5f, 0);

        // Use this for initialization
        void Awake()
        {
            myDirector = SSDirector.getInstance();
            myDirector.setCurrentSceneController(this);
            myDirector.getCurrentSceneController().loadSrc();

            setCharacterPositions(priestOnLeftshore, LeftPriestPos);
            setCharacterPositions(priestOnRightshore, RightPriestPos);
            setCharacterPositions(devilOnLeftshore, LeftDevilPos);
            setCharacterPositions(devilOnRightshore, RightDevilPos);
        }

        void start()
        {
            actionManager = GetComponent<FirstSSActionManager>() as FirstSSActionManager;
        }

        // Update is called once per frame
        void Update()
        {
            if (myDirector.state == State.BOAT_MOVING_FROM_LEFT_TO_RIGHT)
            {
                //boat.transform.position = Vector3.MoveTowards(boat.transform.position, RightBoatPos, speed * Time.deltaTime);
                if (boat.transform.position == RightBoatPos)
                {
                    myDirector.state = State.BOAT_STOP_ON_THE_RIGHT_SHORE;
                }
            }
            else if (myDirector.state == State.BOAT_MOVING_FROM_RIGHT_TO_LEFT)
            {
                //boat.transform.position = Vector3.MoveTowards(boat.transform.position, LeftBoatPos, speed * Time.deltaTime);
                if (boat.transform.position == LeftBoatPos)
                {
                    myDirector.state = State.BOAT_STOP_ON_THE_LEFT_SHORE;
                }
            }
            else if (myDirector.state != State.WIN && myDirector.state != State.LOSE) check();
        }

        //检查游戏是否结束
        void check()
        {
            //玩家获胜结束游戏：在右边出现全部三个法师和三个牧师：
            if (priestOnRightshore.Count == 3 && devilOnRightshore.Count == 3)
            {
                myDirector.state = State.WIN;
                return;
            }
            //玩家失败结束游戏：在任意时刻在左边或者右边的恶魔超过牧师：
            int a = 0, b = 0;
            int c = 0, d = 0, e = 0, f = 0;

            for (int i = 0; i < 2; ++i)
            {
                if (BoatAndCreature[i] != null)
                {
                    if (BoatAndCreature[i].tag == "Priest")
                    {
                        a++;
                    }
                    else if (BoatAndCreature[i].tag == "Devil")
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
            }
            else if (myDirector.state == State.BOAT_STOP_ON_THE_RIGHT_SHORE)
            {

                c = priestOnLeftshore.Count;
                d = devilOnLeftshore.Count;
                e = priestOnRightshore.Count + a;
                f = devilOnRightshore.Count + b;
            }
            //存在牧师而且牧师比恶魔少就输了
            if (c != 0 && c < d || e != 0 && e < f)
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
                GameObject temp = Instantiate(Resources.Load("prefabs/Priest")) as GameObject;
                temp.name = i + "";
                priestOnLeftshore.Enqueue(temp);
                GameObject temp1 = Instantiate(Resources.Load("prefabs/Devil")) as GameObject;
                temp1.name = i + 3 + "";
                devilOnLeftshore.Enqueue(temp1);
            }
        }

        void setCharacterPositions(Queue<GameObject> queue, Vector3 pos)
        {
            GameObject[] array = queue.ToArray();
            for (int i = 0; i < queue.Count; ++i)
            {
                int temp = 0;
                if (Convert.ToInt32(array[i].name) >= 3)
                {
                    temp = Convert.ToInt32(array[i].name) - 3;
                } else
                {
                    temp = Convert.ToInt32(array[i].name);
                }
                array[i].transform.position = new Vector3(pos.x - distanceBetweenObj * temp, pos.y, pos.z);
            }
        }

        //判断人是否位于船的左侧
        public bool ifPeopleOnTheLeftBoat(GameObject gameObj)
        {
            //Debug.Log(gameObj.name);
            return BoatAndCreature[0] == null ? false : BoatAndCreature[0].name == gameObj.name;
        }

        //判断人是否位于船的右侧
        public bool ifPeopleOnTheRightBoat(GameObject gameObj)
        {
            return BoatAndCreature[1] == null ? false : BoatAndCreature[1].name == gameObj.name;
        }

        public bool ifDevilOnTheLeftShore(GameObject gameObj)
        {
            GameObject[] array = devilOnLeftshore.ToArray();
            for (int i = 0; i < devilOnLeftshore.Count; ++i)
            {
                if (array[i].name == gameObj.name)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ifDevilOnTheRightShore(GameObject gameObj)
        {
            GameObject[] array = devilOnRightshore.ToArray();
            for (int i = 0; i < devilOnRightshore.Count; ++i)
            {
                if (array[i].name == gameObj.name)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ifPriestOnTheLeftShore(GameObject gameObj)
        {
            GameObject[] array = priestOnLeftshore.ToArray();
            for (int i = 0; i < priestOnLeftshore.Count; ++i)
            {
                if (array[i].name == gameObj.name)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ifPriestOnTheRightShore(GameObject gameObj)
        {
            GameObject[] array = priestOnRightshore.ToArray();
            for (int i = 0; i < priestOnRightshore.Count; ++i)
            {
                if (array[i].name == gameObj.name)
                {
                    return true;
                }
            }
            return false;
        }

        //对象上船
        public void leftShorePriestGoOnBoat(string index)
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
            {
                GameObject[] array = priestOnLeftshore.ToArray();
                Queue<GameObject> priestOnLeftshore1 = new Queue<GameObject>();
                for (int i = 0; i < priestOnLeftshore.Count; i++)
                {
                    if (array[i].name != index)
                    {
                        priestOnLeftshore1.Enqueue(array[i]);
                    } else
                    {
                        if (BoatAndCreature[0] == null)
                        {
                            BoatAndCreature[0] = array[i];
                            actionManager.getOnBoat(array[i], 0, 0);
                        }
                        else
                        {
                            BoatAndCreature[1] = array[i];
                            actionManager.getOnBoat(array[i], 0, 1);
                        }
                        array[i].transform.parent = boat.transform;
                    }
                }
                priestOnLeftshore = priestOnLeftshore1;
            }
        }

        public void rightShorePriestGoOnBoat(string index)
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
            {
                GameObject[] array = priestOnRightshore.ToArray();
                Queue<GameObject> priestOnRightshore1 = new Queue<GameObject>();
                for (int i = 0; i < priestOnRightshore.Count; i++)
                {
                    if (array[i].name != index)
                    {
                        priestOnRightshore1.Enqueue(array[i]);
                    }
                    else
                    {
                        if (BoatAndCreature[0] == null)
                        {
                            BoatAndCreature[0] = array[i];
                            actionManager.getOnBoat(array[i], 1, 0);
                        }
                        else
                        {
                            BoatAndCreature[1] = array[i];
                            actionManager.getOnBoat(array[i], 1, 1);
                        }
                        array[i].transform.parent = boat.transform;
                    }
                }
                priestOnRightshore = priestOnRightshore1;
            }
        }

        public void leftShoreDevilGoOnBoat(string index)
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
            {
                GameObject[] array = devilOnLeftshore.ToArray();
                Queue<GameObject> devilOnLeftshore1 = new Queue<GameObject>();
                for (int i = 0; i < devilOnLeftshore.Count; i++)
                {
                    if (array[i].name != index)
                    {
                        devilOnLeftshore1.Enqueue(array[i]);
                    }
                    else
                    {
                        if (BoatAndCreature[0] == null)
                        {
                            BoatAndCreature[0] = array[i];
                            actionManager.getOnBoat(array[i], 0, 0);
                        }
                        else
                        {
                            BoatAndCreature[1] = array[i];
                            actionManager.getOnBoat(array[i], 0, 1);
                        }
                        array[i].transform.parent = boat.transform;
                    }
                }
                devilOnLeftshore = devilOnLeftshore1;
            }
        }

        public void rightShoreDevilGoOnBoat(string index)
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
            {
                GameObject[] array = devilOnRightshore.ToArray();
                Queue<GameObject> devilOnRightshore1 = new Queue<GameObject>();
                for (int i = 0; i < devilOnRightshore.Count; i++)
                {
                    if (array[i].name != index)
                    {
                        devilOnRightshore1.Enqueue(array[i]);
                    }
                    else
                    {
                        if (BoatAndCreature[0] == null)
                        {
                            BoatAndCreature[0] = array[i];
                            actionManager.getOnBoat(array[i], 1, 0);
                        }
                        else
                        {
                            BoatAndCreature[1] = array[i];
                            actionManager.getOnBoat(array[i], 1, 1);
                        }
                        array[i].transform.parent = boat.transform;
                    }
                }
                devilOnRightshore = devilOnRightshore1;
            }
        }

        //对象下船
        public void GoOffBoard(int index)
        {
            if (BoatAndCreature[index] != null)
            {
                BoatAndCreature[index].transform.parent = null;

                if (myDirector.state == State.BOAT_STOP_ON_THE_RIGHT_SHORE)
                {
                    actionManager.getOffBoat(BoatAndCreature[index], 1);
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
                    actionManager.getOffBoat(BoatAndCreature[index], 0);
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
                actionManager.moveBoat(boat);
            }
        }

        //注册重开游戏事件（点击View的restart重开游戏）
        public void restart()
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            myDirector.state = State.BOAT_STOP_ON_THE_LEFT_SHORE;
            //Debug.Log(priestOnLeftshore.Count);
            for (int i = 0; i < priestOnRightshore.Count; i++)
            {
                priestOnLeftshore.Enqueue(priestOnRightshore.Dequeue());
            }
            //Debug.Log(priestOnLeftshore.Count);
            for (int i = 0; i < devilOnRightshore.Count; i++)
            {
                devilOnLeftshore.Enqueue(devilOnRightshore.Dequeue());
            }
            if (BoatAndCreature[0] != null)
            {
                BoatAndCreature[0].transform.parent = null;
                if (BoatAndCreature[0].tag == "Priest")
                {
                    priestOnLeftshore.Enqueue(BoatAndCreature[0]);
                } else
                {
                    devilOnLeftshore.Enqueue(BoatAndCreature[0]);
                }
                BoatAndCreature[0] = null;
            }
            if (BoatAndCreature[1] != null)
            {
                BoatAndCreature[1].transform.parent = null;
                if (BoatAndCreature[1].tag == "Priest")
                {
                    priestOnLeftshore.Enqueue(BoatAndCreature[1]);
                } else
                {
                    devilOnLeftshore.Enqueue(BoatAndCreature[1]);
                }
                BoatAndCreature[1] = null;
            }
            for (int i = 0; i < 10; i++)
            {

                boat.transform.position = LeftBoatPos;
                setCharacterPositions(priestOnLeftshore, LeftPriestPos);
                setCharacterPositions(devilOnLeftshore, LeftDevilPos);
            }
        }

        public void clickOne()
        {
            GameObject gameObj = null;

            if (Input.GetMouseButtonDown(0) && (SSDirector.getInstance().state == State.BOAT_STOP_ON_THE_LEFT_SHORE || SSDirector.getInstance().state == State.BOAT_STOP_ON_THE_RIGHT_SHORE))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    gameObj = hit.transform.gameObject;
                }
            }
            if (gameObj == null) return;
            //Debug.Log(gameObj.name+"hello");
            if (ifPeopleOnTheLeftBoat(gameObj))
            {
                GoOffBoard(0);
            }
            else if (ifPeopleOnTheRightBoat(gameObj))
            {
                GoOffBoard(1);
            }
            else if (gameObj.tag == "Devil")
            {
                if (ifDevilOnTheLeftShore(gameObj))
                {
                    leftShoreDevilGoOnBoat(gameObj.name);
                }
                else if (ifDevilOnTheRightShore(gameObj))
                {
                    rightShoreDevilGoOnBoat(gameObj.name);
                }
            }
            else if (gameObj.tag == "Priest")
            {
                if (ifPriestOnTheLeftShore(gameObj))
                {
                    leftShorePriestGoOnBoat(gameObj.name);
                }
                else if (ifPriestOnTheRightShore(gameObj))
                {
                    rightShorePriestGoOnBoat(gameObj.name);
                }
            }
            else if (gameObj.tag == "Boat")
            {
                moveBoat();
            }
        }

        //可以执行的操作
        enum NEXT_ACTION {P, PP, D, DD, PD};

        //是否正在执行提示的步骤，如果是，需要使得提示按钮无效
        bool isPrompting = false;

        NEXT_ACTION getNextAction() {
            NEXT_ACTION nextAction = NEXT_ACTION.P;
            int Pcount = priestOnLeftshore.Count;
            int Dcount = devilOnLeftshore.Count;
            if (Pcount == 3 && Dcount == 3 && SSDirector.getInstance().state == State.BOAT_STOP_ON_THE_LEFT_SHORE)
            {
                nextAction = NEXT_ACTION.DD;
            }
            else if (Pcount == 2 && Dcount == 2 && SSDirector.getInstance().state != State.BOAT_STOP_ON_THE_LEFT_SHORE) {
                nextAction = NEXT_ACTION.P;
            }
            else if (Pcount == 3 && Dcount == 2 && SSDirector.getInstance().state != State.BOAT_STOP_ON_THE_LEFT_SHORE)
            {
                nextAction = NEXT_ACTION.D;
            }
            else if (Pcount == 3 && Dcount == 1 && SSDirector.getInstance().state != State.BOAT_STOP_ON_THE_LEFT_SHORE)
            {
                nextAction = NEXT_ACTION.D;
            }
            else if (Pcount == 3 && Dcount == 2 && SSDirector.getInstance().state == State.BOAT_STOP_ON_THE_LEFT_SHORE)
            {
                nextAction = NEXT_ACTION.DD;
            }
            else if (Pcount == 3 && Dcount == 1 && SSDirector.getInstance().state == State.BOAT_STOP_ON_THE_LEFT_SHORE)
            {
                nextAction = NEXT_ACTION.PP;
            }
            else if (Pcount == 3 && Dcount == 0 && SSDirector.getInstance().state != State.BOAT_STOP_ON_THE_LEFT_SHORE)
            {
                nextAction = NEXT_ACTION.D;
            }
            else if (Pcount == 1 && Dcount == 1 && SSDirector.getInstance().state != State.BOAT_STOP_ON_THE_LEFT_SHORE)
            {
                nextAction = NEXT_ACTION.PD;
            }
            else if (Pcount == 2 && Dcount == 2 && SSDirector.getInstance().state == State.BOAT_STOP_ON_THE_LEFT_SHORE) {
                nextAction = NEXT_ACTION.PP;
            }
            else if (Pcount == 0 && Dcount == 2 && SSDirector.getInstance().state != State.BOAT_STOP_ON_THE_LEFT_SHORE) {
                nextAction = NEXT_ACTION.D;
            }
            else if (Pcount == 0 && Dcount == 3 && SSDirector.getInstance().state == State.BOAT_STOP_ON_THE_LEFT_SHORE)
            {
                nextAction = NEXT_ACTION.DD;
            }
            else if (Pcount == 2 && Dcount == 1 && SSDirector.getInstance().state == State.BOAT_STOP_ON_THE_LEFT_SHORE)
            {
                nextAction = NEXT_ACTION.PP;
            }
            else if (Pcount == 0 && Dcount == 1 && SSDirector.getInstance().state != State.BOAT_STOP_ON_THE_LEFT_SHORE)
            {
                nextAction = NEXT_ACTION.P;
            }
            else if (Pcount == 1 && Dcount == 1 && SSDirector.getInstance().state == State.BOAT_STOP_ON_THE_LEFT_SHORE)
            {
                nextAction = NEXT_ACTION.PD;
            }
            else if (Pcount == 0 && Dcount == 2 && SSDirector.getInstance().state == State.BOAT_STOP_ON_THE_LEFT_SHORE)
            {
                nextAction = NEXT_ACTION.DD;
            }
            return nextAction;
        }

        private IEnumerator doNextAction()
        {
            // 如果当前状态下船上有人，就先让人下船
            int temp = 0;
            for (int i = 0; i < 2; i++)
            {
                if (BoatAndCreature[i] != null)
                {
                    temp++;
                }
            }
            if (temp != 0)
            {
                GoOffBoard(0);
                GoOffBoard(1);
                yield return new WaitForSeconds(1.2f);
            }

            // 首先判断当前的情况，处于什么状态，得出下一步的动作是什么
            NEXT_ACTION nextAction = getNextAction();

            if (SSDirector.getInstance().state == State.BOAT_STOP_ON_THE_LEFT_SHORE)
            {
                if (nextAction == NEXT_ACTION.P)
                {
                    leftShorePriestGoOnBoat(priestOnLeftshore.ToArray()[0].name);
                }
                else if (nextAction == NEXT_ACTION.PP)
                {
                    leftShorePriestGoOnBoat(priestOnLeftshore.ToArray()[0].name);
                    leftShorePriestGoOnBoat(priestOnLeftshore.ToArray()[0].name);
                }
                else if (nextAction == NEXT_ACTION.D)
                {
                    leftShoreDevilGoOnBoat(devilOnLeftshore.ToArray()[0].name);
                }
                else if (nextAction == NEXT_ACTION.DD)
                {
                    leftShoreDevilGoOnBoat(devilOnLeftshore.ToArray()[0].name);
                    leftShoreDevilGoOnBoat(devilOnLeftshore.ToArray()[0].name);
                }
                else if (nextAction == NEXT_ACTION.PD)
                {
                    leftShorePriestGoOnBoat(priestOnLeftshore.ToArray()[0].name);
                    leftShoreDevilGoOnBoat(devilOnLeftshore.ToArray()[0].name);
                }
            } else
            {
                if (nextAction == NEXT_ACTION.P)
                {
                    rightShorePriestGoOnBoat(priestOnRightshore.ToArray()[0].name);
                }
                else if (nextAction == NEXT_ACTION.PP)
                {
                    rightShorePriestGoOnBoat(priestOnRightshore.ToArray()[0].name);
                    rightShorePriestGoOnBoat(priestOnRightshore.ToArray()[0].name);
                }
                else if (nextAction == NEXT_ACTION.D)
                {
                    rightShoreDevilGoOnBoat(devilOnRightshore.ToArray()[0].name);
                }
                else if (nextAction == NEXT_ACTION.DD)
                {
                    rightShoreDevilGoOnBoat(devilOnRightshore.ToArray()[0].name);
                    rightShoreDevilGoOnBoat(devilOnRightshore.ToArray()[0].name);
                }
                else if (nextAction == NEXT_ACTION.PD)
                {
                    rightShorePriestGoOnBoat(priestOnRightshore.ToArray()[0].name);
                    rightShoreDevilGoOnBoat(devilOnRightshore.ToArray()[0].name);
                }
            }
            yield return new WaitForSeconds(1.2f);
            // 开船
            moveBoat();
            yield return new WaitForSeconds(1.2f);
            // 下船
            GoOffBoard(0);
            GoOffBoard(1);
            yield return new WaitForSeconds(1.2f);
            isPrompting = false;
        }

        public void prompt()
        {
            if (isPrompting)
            {
                return;
            } else
            {
                isPrompting = true;
            }
            // 如果船没有靠岸，这个点击时没有用的
            if (!(SSDirector.getInstance().state == State.BOAT_STOP_ON_THE_LEFT_SHORE || SSDirector.getInstance().state == State.BOAT_STOP_ON_THE_RIGHT_SHORE))
            {
                isPrompting = false;
                return;
            }

            // 执行动作
            StartCoroutine(doNextAction());
        }
    }

}
