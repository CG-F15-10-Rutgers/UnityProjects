using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObstacleControl : MonoBehaviour {
	
	public float speed;
	private Rigidbody rb;
    private bool isObstacleActive;

	void Start () {

		rb = GetComponent<Rigidbody> ();
        isObstacleActive = false;
    }

	void FixedUpdate() {
        if (Input.GetButtonDown("Fire1"))
        {
            isObstacleActive = isObstacleActived();
        }

        if (isObstacleActive)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(-moveVertical, 0.0f, moveHorizontal);

            rb.AddForce(movement * speed);
        }	
	}

    bool isObstacleActived()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(cameraRay, out hit, 100))
        {
            if (hit.collider.gameObject == this.gameObject)
                return true;
            else if (hit.collider.CompareTag("Player") && isObstacleActive)
                return true;
            else
            {
                rb.velocity = new Vector3(0f, 0f, 0f);
                rb.angularVelocity = new Vector3(0f, 0f, 0f);
            }
        }
        return false;
    }
}
