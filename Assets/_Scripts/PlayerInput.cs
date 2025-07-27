using UnityEngine;

public class PlayerInput : MonoBehaviour {
	const KeyCode cancelKey = KeyCode.Escape;

	float cancelKeyPressTime;

	void FixedUpdate() {
		if (!Input.GetKey(cancelKey)) {
			if (cancelKeyPressTime <= 0f) return;

			cancelKeyPressTime = 0f;
			return;
		}

		if (!WarheadController.Singleton.GetIsRunning()) return;

		cancelKeyPressTime += Time.fixedDeltaTime;
	}

	void Update() {
		if (WarheadController.Singleton.GetTimeUntilDetonation() <= 20f) return;

		if (cancelKeyPressTime < 3f) return;

		cancelKeyPressTime = 0f;
		WarheadController.Singleton.CancelWarhead(true);
	}
}
