using UnityEngine;

public class Stabilizer : MonoBehaviour {

	void LateUpdate() {
		transform.rotation = Quaternion.Euler(0, 0, Quaternion.Inverse(transform.parent.rotation).z + transform.parent.rotation.z);
		if((transform.parent.localScale.x < 0 && transform.localScale.x > 0) || (transform.parent.localScale.x > 0 && transform.localScale.x < 0)) {
			Vector3 localScale = transform.localScale;
			localScale.x *= -1;
			transform.localScale = localScale;
		}
	}
}
