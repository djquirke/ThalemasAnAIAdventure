using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.W))
		{
			camera.transform.Translate(Vector3.up);
		}
		if (Input.GetKey(KeyCode.A))
		{
			camera.transform.Translate(Vector3.left);
		}
		if (Input.GetKey(KeyCode.S))
		{
			camera.transform.Translate(Vector3.down);
		}
		if (Input.GetKey(KeyCode.D))
		{
			camera.transform.Translate(Vector3.right);
		}
		camera.orthographicSize -= (Input.mouseScrollDelta.y / 5f);
	}
}
