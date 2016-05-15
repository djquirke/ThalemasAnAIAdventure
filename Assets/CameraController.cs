using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	private float max_size = 36, min_size = 6;
	private float min_z = 1, max_z = 47;
	private float min_x = 0, max_x = 50;

	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.W)) camera.transform.Translate(Vector3.up);
		if (Input.GetKey(KeyCode.A)) camera.transform.Translate(Vector3.left);
		if (Input.GetKey(KeyCode.S)) camera.transform.Translate(Vector3.down);
		if (Input.GetKey(KeyCode.D)) camera.transform.Translate(Vector3.right);

		camera.orthographicSize -= Input.mouseScrollDelta.y;
		camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, min_size, max_size);
		float new_z = Mathf.Clamp (camera.transform.position.z, min_z, max_z);
		float new_x = Mathf.Clamp (camera.transform.position.x, min_x, max_x);
		camera.transform.position = new Vector3(new_x, camera.transform.position.y, new_z);
	}
}
