using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System;

public class FirstSceneController : MonoBehaviour, IUserAction
{
    SSDirector myDirector;
    GameStatus gameStatus;

    public GameObject myExplosion;
    public Material myRed, myOrange, myGreen, myBlue;

    private GameObject explosion;

    // Use this for initialization
    void Awake()
    {
        myDirector = SSDirector.getInstance();
        myDirector.setCurrentSceneController(this);
        explosion = Instantiate(myExplosion);
        Physics.gravity = new Vector3(0, -20F, 0);
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

    //获取材质
    private Material getMaterial(int i)
    {
        i = i % 4;
        if (i == 1)
        {
            return myRed;
        }
        else if (i == 2)
        {
            return myOrange;
        }
        else if (i == 3)
        {
            return myGreen;
        }
        else
        {
            return myBlue;
        }
    }

    //处理用户按下空格按键
    public void spacePress()
    {
        //如果用户按下space并且此时没有飞盘正在飞行中，则通知飞盘工厂飞出飞盘
        if (Input.GetKeyDown(KeyCode.Space) && !DiskFactory.getInstance().isLaunching())
        {
            //Debug.Log("test");
            StartCoroutine(launchDisks(gameStatus.getRound()));
        }
    }

    //第n轮的10次trial每一次都是发射n个飞盘，相邻两个飞盘发射时间间隔为2/n
    IEnumerator launchDisks(int round)
    {
        for (int i = 0; i < round; i++)
        {
            GameObject disk = DiskFactory.getInstance().getUseableDisk();
            disk.transform.position = new Vector3(0, -5f, 0);
            disk.GetComponent<MeshRenderer>().material = getMaterial(gameStatus.getRound());

            Vector3 force = getRandomForce();
            disk.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            float temp = 2.0f / gameStatus.getRound();
            yield return new WaitForSeconds(temp);
        }
        gameStatus.addTrial();
    }

    Vector3 getRandomForce()
    {
        int x = UnityEngine.Random.Range(-30, 30);
        int y = UnityEngine.Random.Range(15, 40);
        int z = UnityEngine.Random.Range(15, 40);
        return new Vector3(x, y, z);
    }
    
}