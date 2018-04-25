using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System;

public class FirstSceneController : MonoBehaviour, IUserAction
{
    SSDirector myDirector;
    IActionManager acm;
    GameStatus gameStatus;

    public GameObject myExplosion;
    private GameObject explosion;

    // Use this for initialization
    void Awake()
    {
        myDirector = SSDirector.getInstance();
        myDirector.setCurrentSceneController(this);
        explosion = Instantiate(myExplosion);
    }

    void Start()
    {
        gameStatus = myDirector.getGameStatus();
    }

    // Update is called once per frame
    void Update()
    {
        DiskFactory.getInstance().recycleLanded();
    }

    //开始游戏
    void IUserAction.startGame(int mode)
    {
        gameStatus.startGame();
        if (mode == 0)
        {
            Physics.gravity = new Vector3(0, -20F, 0);
            acm = (PhysisActionManager.getInstance()) as IActionManager;
        } else
        {
            acm = (KinematicActionManager.getInstance()) as IActionManager;
        }
    }

    //重开游戏
    public void restart()
    {
        //仍然无法解决变色问题，所以干脆去掉restart选项
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//        Camera MainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>() as Camera;
//        MainCamera.clearFlags = CameraClearFlags.Depth;
        gameStatus.setScore(0);
        gameStatus.setRound(0);
    }

    //处理用户点击事件
    public void clickOne()
    {
        GameObject gameObj = null;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                gameObj = hit.transform.gameObject;
            }
        }
        
        if (gameObj == null) return;

        if (gameObj.tag != "Disk") return;

        explosion.transform.position = gameObj.transform.position;
        explosion.GetComponent<ParticleSystem>().GetComponent<Renderer>().material = gameObj.GetComponent<MeshRenderer>().material;
        explosion.GetComponent<ParticleSystem>().Play();

        gameStatus.setScore(gameStatus.getScore() + 10);
        DiskFactory.getInstance().free(gameObj);

    }

    //处理用户按下空格按键
    public void spacePress()
    {
        //如果用户按下space并且此时没有飞盘正在飞行中，则通知飞盘工厂飞出飞盘
        if (Input.GetKeyDown(KeyCode.Space) && !DiskFactory.getInstance().isLaunching())
        {
            StartCoroutine(acm.launchDisks(gameStatus.getRound()));
            gameStatus.addTrial();
        }
    }
}