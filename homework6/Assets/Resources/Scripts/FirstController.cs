using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstController : MonoBehaviour, SceneController, UserAction
{
    private bool isgameOver = false;
    Player myGirl;
    public GameObject monster;

    private void Awake()
    {
        Director.getInstance().setFirstController(this);
        MonsterFactory.getInstance().setPrefabs(monster);
    }

    // Use this for initialization
    void Start () {
        myGirl = new Player();
        Director.getInstance().getFirstController().LoadResources();
    }

    private void Update()
    {
        myGirl.myUpdate();
    }

    public void LoadResources () {
        GameObject temp1 = MonsterFactory.getInstance().getMonster();
        temp1.transform.position = new Vector3(3,0,0);
        GameObject temp2 = MonsterFactory.getInstance().getMonster();
        temp2.transform.position = new Vector3(-3, 0, 0);
        GameObject temp3 = MonsterFactory.getInstance().getMonster();
        temp3.transform.position = new Vector3(3, 0, 10);
        GameObject temp4 = MonsterFactory.getInstance().getMonster();
        temp4.transform.position = new Vector3(-3, 0, 10);
    }

    public void movePlayer(float h, float v) {
        myGirl.run(h, v);
    }
    public void jump() {
        myGirl.jump();
    }
    public bool isGameOver() {
        return isgameOver;
    }
    public void reStart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        ScoreRecorder.getInstance().setScore(0);
    }

    public void gameOver()
    {
        isgameOver = true;
        myGirl.cry();
    }
}

