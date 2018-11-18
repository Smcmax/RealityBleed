using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/Scan")]
public class ScanAction : Action {

	public override void Execute(StateController p_controller) {
		Scan(p_controller);
	}

	private void Scan(StateController p_controller) {
		float speed = p_controller.m_look.m_scanSpeed * Time.deltaTime;
		Quaternion rotation = p_controller.m_entity.transform.rotation;

		p_controller.m_entity.transform.Rotate(0, 0, speed * 10);
	}
}