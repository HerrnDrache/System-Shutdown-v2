using UnityEngine;
using Mirror;
using TMPro;

public class MultiheadController : NetworkBehaviour {
	[SyncVar(hook = nameof(HandleTimeUpdate))]
	float timeRemaining;
	bool doCountdown;
	bool BoomDone;
	public const float countdownTime = 20f;
	
	[SerializeField] TMP_Text displayText;
	[SerializeField] GameObject EngageButton;

	[Server]
	void Start() {
		timeRemaining = countdownTime;
		EngageButton.SetActive(true);
	}

	[Server]
	void Update() {

			
		if (timeRemaining <= 0f && !BoomDone) {
			BoomDone = true;
			Boom();
		}

		if (!doCountdown) { return; }

		timeRemaining -= Time.deltaTime;    
	}

	[ClientRpc]
	void Boom() {
		Debug.Log("Allahu Akabara");
	}

	[Server]
	public void EngageWarhead() {
		doCountdown = true;
	}

	[Client]
	void HandleTimeUpdate(float previousTime, float currentTime) {
		displayText.text = RemainingTimeToString(currentTime);
	}
	
	string RemainingTimeToString(float remainingTime) {
		if (remainingTime <= 0f) {
			return $"00:00:000";
		}

		// if (remainingTime > displayTimeAfter) {
		// 	return RemainingTimeToString(displayTimeAfter);
		// }

		var mins = Mathf.Floor(remainingTime / 60 % 60);
		var secs = Mathf.Floor(remainingTime % 60);
		var millisecs = Mathf.Floor(remainingTime * 1000 % 1000);

		return $"{mins:00}:{secs:00}:{millisecs:000}";
	}
}
