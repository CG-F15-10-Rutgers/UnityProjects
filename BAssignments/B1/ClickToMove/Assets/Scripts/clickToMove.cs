using UnityEngine;
using System.Collections;

public class clickToMove : MonoBehaviour {

    private NavMeshAgent navMeshAgent;
    private bool isPlayerActive;
    private bool isDestinationSet;
	// Use this for initialization
	void Start () {
        navMeshAgent = GetComponent<NavMeshAgent>();
        isPlayerActive = false;
        isDestinationSet = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.R))
            Application.LoadLevel(0);

        if (Input.GetButtonDown("Fire1")){
            isPlayerActive = isPlayerActivated();
        }
        if (isPlayerActive)
        {   
            if (Input.GetButtonDown("Fire2"))
            {
                Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                isDestinationSet = true;
                if (Physics.Raycast(cameraRay, out hit, 100))
                {
                    navMeshAgent.destination = hit.point;
                }
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
            } else if ( isPlayerActive )
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
}
