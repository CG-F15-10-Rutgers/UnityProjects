using UnityEngine;
using System.Collections;

[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Rigidbody))]
public class BotControlScript : MonoBehaviour
{
	[System.NonSerialized]					
	public float lookWeight;					
	
	[System.NonSerialized]
	public Transform enemy;						
	
	public float animSpeed = 1.5f;				
	public float lookSmoother = 3f;			
	public bool useCurves;						
	
	
	private Animator anim;							
	private AnimatorStateInfo currentBaseState;		
	private AnimatorStateInfo layer2CurrentState;	
	private CapsuleCollider col;				
	
	
	static int idleState = Animator.StringToHash("Base Layer.Idle");	
	static int walkState = Animator.StringToHash("Base Layer.Walk");		
	static int runState = Animator.StringToHash("Base Layer.Run");			
	static int jumpState = Animator.StringToHash("Base Layer.Jump");				
	static int jumpDownState = Animator.StringToHash("Base Layer.JumpDown");		
	static int fallState = Animator.StringToHash("Base Layer.Fall");
	static int rollState = Animator.StringToHash("Base Layer.Roll");
	static int waveState = Animator.StringToHash("Layer2.Wave");
	
	
	void Start ()
	{
		anim = GetComponent<Animator>();					  
		col = GetComponent<CapsuleCollider>();
		if(anim.layerCount ==2)
			anim.SetLayerWeight(1, 1);
	}
	
	
	void FixedUpdate ()
	{
        if (Input.GetKeyDown(KeyCode.R))
            Application.LoadLevel(0);

        float h = Input.GetAxis("Horizontal");			
		float v = Input.GetAxis("Vertical");			
		anim.SetFloat("Speed", v, 1f, Time.deltaTime * 10f);		
		anim.SetFloat("Direction", h, 1f, Time.deltaTime * 10f); 				
		anim.speed = animSpeed;								
		anim.SetLookAtWeight(lookWeight);					
		currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
		
		if(anim.layerCount ==2)		
			layer2CurrentState = anim.GetCurrentAnimatorStateInfo(1);	
		
		

		if(Input.GetButton("Fire2"))
		{
			anim.SetLookAtPosition(enemy.position);
			lookWeight = Mathf.Lerp(lookWeight,1f,Time.deltaTime*lookSmoother);
		}
		else
		{
			lookWeight = Mathf.Lerp(lookWeight,0f,Time.deltaTime*lookSmoother);
		}
		
		if (Input.GetKey(KeyCode.LeftShift)) {
			anim.SetBool("Running", true);
		}
		else {
			anim.SetBool("Running", false);
		}

		if (Input.GetKey(KeyCode.Space))
		{
				anim.SetBool("Jump", true);
		}
		else if(currentBaseState.nameHash == jumpState)
		{
			if(!anim.IsInTransition(0))
			{
				if(useCurves)
					col.height = anim.GetFloat("ColliderHeight");
				
				anim.SetBool("Jump", false);
			}
			
			Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
			RaycastHit hitInfo = new RaycastHit();
			
			if (Physics.Raycast(ray, out hitInfo))
			{
				if (hitInfo.distance > 1.75f)
				{

					anim.MatchTarget(hitInfo.point, Quaternion.identity, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(0, 1, 0), 0), 0.35f, 0.5f);
				}
			}
		}
		

	}
};
