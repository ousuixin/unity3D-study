using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {
    private Rigidbody rg;
    private Animator anim;
    private CapsuleCollider col;
    private AnimatorStateInfo currentBaseState;

    static int walkState = Animator.StringToHash("Base Layer.walk");
    static int runState = Animator.StringToHash("Base Layer.run");
    static int idleState = Animator.StringToHash("Base Layer.idle");
    static int attackState = Animator.StringToHash("Base Layer.attack_1");

    private float time;
    private float timePassed;

    private bool isRun = false;
    private Vector3 pos;

    private float length = 13;
    private float walkSpeed = 3;
    private float runSpeed = 5;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider>();
        rg = GetComponent<Rigidbody>();
        rg.useGravity = true;
        anim.SetBool("IsWalk", true);
        setTime();
    }

    private void FixedUpdate()
    {
        if (Director.getInstance().getFirstController().isGameOver())
        {
            return;
        }

        if (isRun == true)
        {
            anim.SetBool("IsRun",true);
            Vector3 dir = pos - transform.position;
            dir = dir.normalized;
            Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
            rotation.x = 0;
            rotation.z = 0;
            transform.rotation = rotation;

            Vector3 velocity0 = new Vector3(0, 0, runSpeed);
            velocity0 = transform.TransformDirection(velocity0);
            velocity0.y = 0;

            transform.localPosition += velocity0 * Time.fixedDeltaTime;
            return;
        } else
        {
            anim.SetBool("IsRun", false);
        }

        if (timePassed >= time)
        {
            transform.Rotate(0, 90, 0);
            timePassed = 0;
            return;
        }

        Vector3 velocity = new Vector3(0, 0, walkSpeed);
        velocity = transform.TransformDirection(velocity);

        transform.localPosition += velocity * Time.fixedDeltaTime;
        timePassed += Time.fixedDeltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Wall")
        {
            isRun = false;
            transform.Rotate(0, 90, 0);
            setTime();
        } else if (collision.gameObject.tag == "Player")
        {
            Director.getInstance().getFirstController().gameOver();
            anim.SetBool("IsWalk",false);
            anim.SetTrigger("Attack_1");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isRun = true;
            pos = other.gameObject.transform.position;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            pos = other.gameObject.transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isRun = false;
            ScoreRecorder.getInstance().addScore();
        }
    }

    private void setTime()
    {
        System.Random random = new System.Random();
        time = random.Next(2,3);
        timePassed = 0;
    }




}
