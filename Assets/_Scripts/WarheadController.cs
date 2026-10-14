using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Mirror;
using System;

public class WarheadController : MonoBehaviour {
	public static WarheadController Singleton { get; private set; }
	[SerializeField] float displayTimeAfter;
	[SerializeField] AudioSource source;
	[SerializeField] AudioClip detonationSequenceClip;
	[SerializeField] AudioClip last20Secs;
	[SerializeField] AudioClip cancelClip;
	[SerializeField] AudioClip clickclick;
	[SerializeField] GameObject connectionMenu;

	public String IPAdress { get; set; }

	float timeUntilDetonation;
	bool isRunning;
	bool counting20Secs;
	bool shutdownInProgress;
	bool doShutdown = true;

	public bool IsRunning {
		get => isRunning;
	}

	public bool DoShutdown {
		get => doShutdown;
	}

	public float TimeUntilDetonation {
		get => timeUntilDetonation;
	}

	void Awake() {
		Singleton = this;
	}

	void Start() {
		ResetTimeUntilDetonation();
	}

	public void EngageWarhead() {
		ClickSound();

		if (DiscordManager.IsExisting()) {
			DiscordManager.Singleton.SetEngageActivity();
		}

		InterfaceManipulation.Singleton.HideMainMenu();
		isRunning = true;
		counting20Secs = false;

		InterfaceManipulation.Singleton.SetStatus("Running...", "#ff0000");
		source.clip = detonationSequenceClip;
		source.Play();
		InvokeRepeating(nameof(UpdateDiscordActivity), 12.5f, 5f);
	}

	public void CancelWarhead(bool playCancelSound) {
		if (!isRunning) return;
		ClickSound();

		Invoke(nameof(ResetTimeUntilDetonation), 1f);
		CancelInvoke(nameof(UpdateDiscordActivity));
		InterfaceManipulation.Singleton.ShowMainMenu();
		if (DiscordManager.IsExisting()) {
			DiscordManager.Singleton.SetWaitingActivity();
		}
		
		isRunning = false;
		source.Stop();

		InterfaceManipulation.Singleton.SetStatus("Restarting...", "#ffff00");
		StartCoroutine(SetStatusReadyAfterDelay());

		if (playCancelSound) {
			source.clip = cancelClip;
			source.Play();
		}
	}

	IEnumerator SetStatusReadyAfterDelay() {
		yield return new WaitForSeconds(20f);
		InterfaceManipulation.Singleton.SetStatus("Ready", "#00ff00");
		if (DiscordManager.IsExisting()) {
			DiscordManager.Singleton.SetStartupActivity();
		}
	}


	void Update() {
		CountDown();
		UpdateCountdownDisplay();
	}

	void UpdateDiscordActivity() {
		if (DiscordManager.IsExisting()) {
			DiscordManager.Singleton.SetDetonationActivity();
		}
	}

	void CountDown() {
		if (!isRunning) return;

		timeUntilDetonation -= Time.deltaTime;

		if (!counting20Secs && timeUntilDetonation <= 20f + 1f) {
			CountLast20Secs();
		}

		if (!shutdownInProgress && timeUntilDetonation <= 0f) {
			SystemShutdown();
		}
	}

	void UpdateCountdownDisplay() {
		var remainingTime = RemainingTimeToString(timeUntilDetonation);


		InterfaceManipulation.Singleton.CountdownText1.text = remainingTime;
		InterfaceManipulation.Singleton.CountdownText2.text = remainingTime;
	}

	void CountLast20Secs() {
		counting20Secs = true;
		InterfaceManipulation.Singleton.BlinkWarning();
		source.PlayOneShot(last20Secs);
	}

	public void SwitchMode() {
		ClickSound();

		if (Input.GetKey(KeyCode.LeftControl)) {
			NetworkManager.singleton.StartHost();

			Debug.Log("KYS");
			return;
		} else if (Input.GetKey(KeyCode.RightControl)) {
			connectionMenu.SetActive(true);

			Debug.Log("SYK");
			return;
		}
		//Somewhat Keychaos
		doShutdown = !doShutdown;

		if (doShutdown) {
			InterfaceManipulation.Singleton.SetInfo("Shutdown", "#ff0000");
		} else {
			InterfaceManipulation.Singleton.SetInfo("Restart", "#f37907");
		}

		DiscordManager.Singleton.SetStartupActivity();
	}

	void SystemShutdown() {
		shutdownInProgress = true;
		if (DiscordManager.IsExisting()) {
			DiscordManager.Singleton.SetOfflineActivity();
		}

#if !UNITY_EDITOR
		var shutdownProcess = new ProcessStartInfo("shutdown", doShutdown ? "/s /t 0" : "/r /t 0") {
			CreateNoWindow = true,
			UseShellExecute = false
		};

		Process.Start(shutdownProcess);
#else
		Debug.Log($"System is now {(doShutdown ? "shutting down" : "restarting")}!");
#endif
	}

	string RemainingTimeToString(float remainingTime) {
		if (remainingTime <= 0f) {
			return $"00:00:000";
		}

		if (remainingTime > displayTimeAfter) {
			return RemainingTimeToString(displayTimeAfter);
		}

		var mins = Mathf.Floor(remainingTime / 60 % 60);
		var secs = Mathf.Floor(remainingTime % 60);
		var millisecs = Mathf.Floor(remainingTime * 1000 % 1000);

		return $"{mins:00}:{secs:00}:{millisecs:000}";
	}

	void ResetTimeUntilDetonation() {
		timeUntilDetonation = detonationSequenceClip.length;
	}

	void ClickSound() {
		source.PlayOneShot(clickclick);
	}

	public void ClientConnect() { //Server connecten
		NetworkManager.singleton.networkAddress = IPAdress;
		NetworkManager.singleton.StartClient();
	}
}
