using UnityEngine;
using System.Collections;

public class KatanaController : MonoBehaviour {

	private Vector3 posOffset;
	private Vector3 orntOffset;
	private bool isHeld;

	public Transform beltHolder;
	public Transform handHolder;

	// Use this for initialization
	void Start () {
	
		isHeld = false;
		//posOffset = beltHolder.transform.position - transform.position;
		//orntOffset = beltHolder.transform.rotation - transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {

		if (!isHeld) {

			transform.position = beltHolder.position;
			transform.rotation = beltHolder.rotation;
		} else {

			transform.position = handHolder.position;
			transform.rotation = handHolder.rotation;
		}
	}

	public void holdKatana() {

		isHeld = true;
	}
}
