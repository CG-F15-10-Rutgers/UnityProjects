using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class BotMoveScript : MonoBehaviour
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

    private NavMeshAgent navMeshAgent;
    private bool isPlayerActive;
    private bool isDestinationSet;
    private float lastClickTime;

    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<CapsuleCollider>();
        if (anim.layerCount == 2)
            anim.SetLayerWeight(1, 1);

        navMeshAgent = GetComponent<NavMeshAgent>();
        isPlayerActive = false;
        isDestinationSet = false;
        lastClickTime = 0;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            Application.LoadLevel(0);

        if (Input.GetButtonDown("Fire1"))
        {
            isPlayerActive = isPlayerActivated();
        }
        if (isPlayerActive)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                if ((Time.time - lastClickTime) < 0.3)
                {
                    navMeshAgent.speed = 5f;
                    anim.SetFloat("Speed", navMeshAgent.speed, 1f, Time.deltaTime * 10f);
                    anim.SetBool("Running", true);
                }
                else
                {
                    navMeshAgent.speed = 2.5f;
                    anim.SetFloat("Speed", navMeshAgent.speed, 1f, Time.deltaTime * 10f);
                    anim.SetBool("Running", false);
                }



                Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                isDestinationSet = true;
                if (Physics.Raycast(cameraRay, out hit, 100))
                {
                    navMeshAgent.destination = hit.point;
                }
                lastClickTime = Time.time;
            }
        }

        if (navMeshAgent.remainingDistance < 0.5)
        {
            anim.SetFloat("Speed", 0, 1f, Time.deltaTime * 10f);
            anim.SetBool("Running", false);
        }
        

        if (navMeshAgent.isOnOffMeshLink)
        {
            anim.SetBool("Jump", true);
            anim.SetBool("Walk", true);

        }
        else
        {
            anim.SetBool("Jump", false);
            if (navMeshAgent.speed > 3)
            {
                anim.SetBool("Running", true);
            }
            else
            {
                anim.SetBool("Running", false);
            }
        }
    }

    bool isPlayerActivated()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(cameraRay, out hit, 100))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                return true;
            }
            else if (isPlayerActive)
            {
                if (hit.collider.CompareTag("MovingObstacle"))
                    return true;
                else if (!isDestinationSet && hit.collider.CompareTag("Player"))
                    return true;
            }
        }
        isDestinationSet = false;
        return false;
    }
};