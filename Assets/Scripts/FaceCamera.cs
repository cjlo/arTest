using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.identity;
//		transform.LookAt(Camera.main.transform.position, -Vector3.up);
	}
}
