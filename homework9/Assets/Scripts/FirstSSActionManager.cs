using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using IControler;

public interface ISSActionCallback
{
    void ActionDone(SSAction source);
}

//SSAction是动作基类，保存要移动的游戏对象的参数、记住该基类的管理者
public class SSAction : ScriptableObject
{

    public bool enable = true;
    public bool destroy = false;

    public GameObject gameObject { get; set; }
    public Transform transform { get; set; }
    public ISSActionCallback callback { get; set; }

    public virtual void Start()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Update()
    {
        throw new System.NotImplementedException();
    }
}

//继承SSaction，是简单动作的实现（平移动作）
public class MoveToAction : SSAction
{
    public Vector3 target;
    public float speed;

    private MoveToAction() { }
    public static MoveToAction GetAction(Vector3 target, float speed)
    {
        MoveToAction action = ScriptableObject.CreateInstance<MoveToAction>();
        action.target = target;
        action.speed = speed;
        return action;
    }

    //执行动作，如果动作完成，则期望管理程序自动回收运行对象，并发出事件通知管理者(管理者通过manager类分配)
    public override void Update()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, target, speed * Time.deltaTime);
        if (this.transform.position == target)
        {
            this.destroy = true;
            this.callback.ActionDone(this);
        }
    }

    public override void Start() { }

}

//继承SSaction，组合动作的实现（组合多个平移动作）
public class SequenceAction : SSAction, ISSActionCallback
{
    public List<SSAction> sequence;
    public int repeat = -1; //repeat==-1表示无限循环，repeat==0表示只执行一遍，repeat>0 表示重复repeat遍
    public int currentAction = 0;//当前动作列表里，执行到的动作序号

    //创建一个动作顺序执行序列，-1 表示 无限循环，start 开始动作
    public static SequenceAction GetAction(int repeat, int currentActionIndex, List<SSAction> sequence)
    {
        SequenceAction action = ScriptableObject.CreateInstance<SequenceAction>();
        action.sequence = sequence;
        action.repeat = repeat;
        action.currentAction = currentActionIndex;
        return action;
    }

    //执行当前动作
    public override void Update()
    {
        if (sequence.Count == 0) return;
        if (currentAction < sequence.Count)
        {
            sequence[currentAction].Update();
        }
    }

    //收到当前动作执行完成的信息，推下一个动作，如果完成一次循环，repeat次数减一。如完成，通知该动作的管理者(管理者通过manager类分配)
    public void ActionDone(SSAction source)
    {
        source.destroy = false;
        this.currentAction++;
        if (this.currentAction >= sequence.Count)
        {
            this.currentAction = 0;
            if (repeat > 0) repeat--;
            if (repeat == 0)
            {
                this.destroy = true;
                this.callback.ActionDone(this);
            }
        }
    }

    //执行一连串动作前，为这一串动作的每个动作注入当前动作游戏对象（也就是系列动作的游戏对象，这一连串动作是同一个游戏对象的），并将自己作为动作事件的接收者
    public override void Start()
    {
        foreach (SSAction action in sequence)
        {
            action.gameObject = this.gameObject;
            action.transform = this.transform;
            action.callback = this;
            action.Start();
        }
    }

    //如果自己被注销，应该释放自己管理的动作
    void OnDestroy()
    {
        foreach (SSAction action in sequence)
        {
            DestroyObject(action);
        }
    }
}

//SSActionManager是动作管理类（管理简单动作和组合动作）
public class SSActionManager : MonoBehaviour
{
    private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
    private List<SSAction> waitingAdd = new List<SSAction>();
    private List<int> watingDelete = new List<int>();

    //创建MonoBehaiviour管理一个动作集合，动作做完自动回收动作
    protected void Update()
    {
        foreach (SSAction ac in waitingAdd)
        {
            actions[ac.GetInstanceID()] = ac;
        }
        waitingAdd.Clear();

        foreach (KeyValuePair<int, SSAction> kv in actions)
        {
            SSAction ac = kv.Value;
            if (ac.destroy)
            {
                watingDelete.Add(ac.GetInstanceID());
            }
            else if (ac.enable)
            {
                ac.Update();
            }
        }

        foreach (int key in watingDelete)
        {
            SSAction ac = actions[key];
            actions.Remove(key);
            DestroyObject(ac);
        }
        watingDelete.Clear();
    }

    //激活动作（为动作绑定对象以及管理者，即消息接收者）
    public void RunAction(GameObject gameObject, SSAction action, ISSActionCallback manager)
    {
        action.gameObject = gameObject;
        action.transform = gameObject.transform;
        action.callback = manager;
        waitingAdd.Add(action);
        action.Start();
    }

    protected void Start(){ }
}


public class FirstSSActionManager : SSActionManager, ISSActionCallback
{
    public FirstSceneControl sceneController;
    public MoveToAction action1, action2;
    public SequenceAction saction;
    float speed;

    protected new void Start()
    {
        sceneController = (FirstSceneControl)SSDirector.getInstance().getCurrentSceneController();
        sceneController.actionManager = this;
        this.speed = sceneController.speed;
        Debug.Log("speed=" + speed);
    }

    protected new void Update()
    {
        base.Update();
    }

    public void ActionDone(SSAction source)
    {
        Debug.Log("当前动作完成");
    }

    //船的平移
    public void moveBoat(GameObject boat)
    {
        Vector3 temp = boat.transform.position == sceneController.LeftBoatPos ? sceneController.RightBoatPos: sceneController.LeftBoatPos;
        action1 = MoveToAction.GetAction(temp, speed);
        this.RunAction(boat, action1, this);
    }

    //人上船
    public void getOnBoat(GameObject people, int shore, int seat)
    {
        if (shore == 0 && seat == 0)
        {
            action1 = MoveToAction.GetAction(new Vector3(-5f, people.transform.position.y, 0), speed);
            action2 = MoveToAction.GetAction(new Vector3(-5f, 0.5f, 0), speed);
        }
        else if (shore == 0 && seat == 1)
        {
            action1 = MoveToAction.GetAction(new Vector3(-3f, people.transform.position.y, 0), speed);
            action2 = MoveToAction.GetAction(new Vector3(-3f, 0.5f, 0), speed);
        }
        else if (shore == 1 && seat == 0)
        {
            action1 = MoveToAction.GetAction(new Vector3(3f, people.transform.position.y, 0), speed);
            action2 = MoveToAction.GetAction(new Vector3(3f, 0.5f, 0), speed);
        }
        else if (shore == 1 && seat == 1)
        {

            action1 = MoveToAction.GetAction(new Vector3(5f, people.transform.position.y, 0), speed);
            action2 = MoveToAction.GetAction(new Vector3(5f, 0.5f, 0), speed);
        }

        SequenceAction saction = SequenceAction.GetAction(0, 0, new List<SSAction> { action1, action2 });
        this.RunAction(people, saction, this);
    }

    //人下船
    public void getOffBoat(GameObject people, int shoreNum)
    {
        action1 = MoveToAction.GetAction(new Vector3(people.transform.position.x, 2.5f, 0), speed);//人向上移动

        if (shoreNum == 0) action2 = MoveToAction.GetAction(new Vector3(-6f - sceneController.distanceBetweenObj * Convert.ToInt32(people.name), 2.5f, 0), speed);//人向左移动
        else action2 = MoveToAction.GetAction(new Vector3(6f + sceneController.distanceBetweenObj * Convert.ToInt32(people.name), 2.5f, 0), speed);//人向右移动

        SequenceAction saction = SequenceAction.GetAction(0, 0, new List<SSAction> { action1, action2 });
        this.RunAction(people, saction, this);
    }

}
