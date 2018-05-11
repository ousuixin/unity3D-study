using UnityEngine;
using System.Collections;

// 必要なコンポーネントの列記
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]

public class Player
{
    GameObject myGirl;
    private bool isJump = false;

    public float animSpeed = 1.3f;
    public float jumpAnimSpeed = 1f;
    public float lookSmoother = 3.0f;
    public bool useCurves = true;
    public float useCurvesHeight = 1f;

    // 前進速度
    public float forwardSpeed = 7.0f;
    // 後退速度
    public float backwardSpeed = 5.0f;
    // 旋转速度
    public float rotateSpeed = 1.3f;
    // 跳跃时施加的力
    public float jumpPower = 5.0f;

    private CapsuleCollider col;
    private Rigidbody rb;

    private Vector3 velocity;

    private float orgColHight;
    private Vector3 orgVectColCenter;

    private Animator anim;
    private AnimatorStateInfo currentBaseState;

    static int idleState = Animator.StringToHash("Base Layer.Idle");
    static int locoState = Animator.StringToHash("Base Layer.Locomotion");
    static int jumpState = Animator.StringToHash("Base Layer.Jump");
    static int restState = Animator.StringToHash("Base Layer.Rest");

    public Player()
    {
        myGirl = GameObject.FindWithTag("Player");
        anim = myGirl.GetComponent<Animator>();
        col = myGirl.GetComponent<CapsuleCollider>();
        rb = myGirl.GetComponent<Rigidbody>();
        orgColHight = col.height;
        orgVectColCenter = col.center;
    }

    public void run(float h, float v)
    {
        anim.SetFloat("Speed", v);
        anim.SetFloat("Direction", h);

        rb.useGravity = true;

        velocity = new Vector3(0, 0, v);
        velocity = myGirl.transform.TransformDirection(velocity);

        if (v > 0.1)
        {
            velocity *= forwardSpeed;
        }
        else if (v < -0.1)
        {
            velocity *= backwardSpeed;
        }

        myGirl.transform.localPosition += velocity * Time.fixedDeltaTime;
        myGirl.transform.Rotate(0, h * rotateSpeed, 0);
    }

    public void jump()
    {
        if (isJump == false)
        {
            if (currentBaseState.fullPathHash == locoState)
            {
                if (!anim.IsInTransition(0))
                {
                    rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
                    anim.SetBool("Jump", true);
                }
            }
        }
    }

    public void cry()
    {
        if (myGirl.transform.position.y > 1)
        {
            myGirl.transform.position = new Vector3(myGirl.transform.position.x, 0, myGirl.transform.position.z);
        }
        anim.SetBool("GameOver",true);
    }

    public void myUpdate()
    {
        if (myGirl.transform.position.y <= 0)
        {
            isJump = false;
            anim.speed = animSpeed;
        }
        else
        {
            anim.speed = jumpAnimSpeed;
        }

        currentBaseState = anim.GetCurrentAnimatorStateInfo(0);

        if (currentBaseState.fullPathHash == locoState)
        {
            if (useCurves)
            {
                resetCollider();
            }
        }
        else if (currentBaseState.fullPathHash == jumpState)
        {

            if (!anim.IsInTransition(0))
            {
                if (useCurves)
                {
                    float jumpHeight = anim.GetFloat("JumpHeight");
                    float gravityControl = anim.GetFloat("GravityControl");
                    if (gravityControl > 0)
                        rb.useGravity = false;

                    Ray ray = new Ray(myGirl.transform.position + Vector3.up, -Vector3.up);
                    RaycastHit hitInfo = new RaycastHit();

                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        if (hitInfo.distance > useCurvesHeight)
                        {
                            col.height = orgColHight - jumpHeight;
                            float adjCenterY = orgVectColCenter.y + jumpHeight;
                            col.center = new Vector3(0, adjCenterY, 0);
                        }
                        else
                        {
                            resetCollider();
                        }
                    }
                }
                anim.SetBool("Jump", false);
            }
        }

        else if (currentBaseState.fullPathHash == idleState)
        {

            if (useCurves)
            {
                resetCollider();
            }

            if (Input.GetButtonDown("Jump"))
            {
                anim.SetBool("Rest", true);
            }
        }

        else if (currentBaseState.fullPathHash == restState)
        {
            if (!anim.IsInTransition(0))
            {
                anim.SetBool("Rest", false);
            }
        }
    }

    void resetCollider()
    {
        col.height = orgColHight;
        col.center = orgVectColCenter;
    }
}
