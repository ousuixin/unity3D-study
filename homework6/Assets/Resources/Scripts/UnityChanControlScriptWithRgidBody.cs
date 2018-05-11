//
// Mecanimのアニメーションデータが、原点で移動しない場合の Rigidbody付きコントローラ
// サンプル
// 2014/03/13 N.Kobyasahi
//
using UnityEngine;
using System.Collections;

// 必要なコンポーネントの列記
[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Rigidbody))]

public class UnityChanControlScriptWithRgidBody : MonoBehaviour
{
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
	public float rotateSpeed = 2.0f;
	// 跳跃时施加的力
	public float jumpPower = 5.0f; 

	private CapsuleCollider col;
	private Rigidbody rb;

	private Vector3 velocity;
	
	private float orgColHight;
	private Vector3 orgVectColCenter;
	
	private Animator anim;
	private AnimatorStateInfo currentBaseState;

	private GameObject cameraObject;
		
	static int idleState = Animator.StringToHash("Base Layer.Idle");
	static int locoState = Animator.StringToHash("Base Layer.Locomotion");
	static int jumpState = Animator.StringToHash("Base Layer.Jump");
	static int restState = Animator.StringToHash("Base Layer.Rest");
    
	void Start ()
	{
		anim = GetComponent<Animator>();
		col = GetComponent<CapsuleCollider>();
		rb = GetComponent<Rigidbody>();
		cameraObject = GameObject.FindWithTag("MainCamera");
		orgColHight = col.height;
		orgVectColCenter = col.center;
}
	
	
	void FixedUpdate ()
	{
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		anim.SetFloat("Speed", v);
		anim.SetFloat("Direction", h);

        if (transform.position.y <= 0)
        {
            isJump = false;
            anim.speed = animSpeed;
        } else
        {
            anim.speed = jumpAnimSpeed;
        }

        currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
		rb.useGravity = true;
		
		
		velocity = new Vector3(0, 0, v);

		velocity = transform.TransformDirection(velocity);

		if (v > 0.1) {
			velocity *= forwardSpeed;
		} else if (v < -0.1) {
			velocity *= backwardSpeed;
		}
		
		if (Input.GetButtonDown("Jump") && isJump == false) {
            isJump = true;

			if (currentBaseState.fullPathHash == locoState){
				if(!anim.IsInTransition(0))
				{
						rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
						anim.SetBool("Jump", true);
				}
			}
		}
        
		transform.localPosition += velocity * Time.fixedDeltaTime;

		transform.Rotate(0, h * rotateSpeed, 0);	
	
		if (currentBaseState.fullPathHash == locoState){
			if(useCurves){
				resetCollider();
			}
		}
		else if(currentBaseState.fullPathHash == jumpState)
		{
			
			if(!anim.IsInTransition(0))
			{
				if(useCurves){
					float jumpHeight = anim.GetFloat("JumpHeight");
					float gravityControl = anim.GetFloat("GravityControl"); 
					if(gravityControl > 0)
						rb.useGravity = false;

					Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
					RaycastHit hitInfo = new RaycastHit();
					
					if (Physics.Raycast(ray, out hitInfo))
					{
						if (hitInfo.distance > useCurvesHeight)
						{
							col.height = orgColHight - jumpHeight;		
							float adjCenterY = orgVectColCenter.y + jumpHeight;
							col.center = new Vector3(0, adjCenterY, 0);
						}
						else{				
							resetCollider();
						}
					}
				}			
				anim.SetBool("Jump", false);
			}
		}

		else if (currentBaseState.fullPathHash == idleState)
		{

			if(useCurves){
				resetCollider();
			}

			if (Input.GetButtonDown("Jump")) {
				anim.SetBool("Rest", true);
			}
		}

		else if (currentBaseState.fullPathHash == restState)
		{
			if(!anim.IsInTransition(0))
			{
				anim.SetBool("Rest", false);
			}
		}
	}

	void OnGUI()
	{
		GUI.Box(new Rect(Screen.width -260, 10 ,250 ,110), "操作方法");
		GUI.Label(new Rect(Screen.width -245,30,250,30),"上/下 键 : 向前跑/向后退");
		GUI.Label(new Rect(Screen.width -245,50,250,30), "左/右 键 : 向左转/向右转");
		GUI.Label(new Rect(Screen.width -245,70,250,30),"奔跑时按下空格键 : 跳跃");
		GUI.Label(new Rect(Screen.width -245,90,250,30),"停下的时候按下空格键 : 卖萌");
	}

    
	void resetCollider()
	{
		col.height = orgColHight;
		col.center = orgVectColCenter;
	}
}
