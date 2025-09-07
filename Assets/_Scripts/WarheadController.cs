using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class WarheadController : MonoBehaviour {
	public static WarheadController Singleton { get; private set; }

	[SerializeField] TMP_Text countdownDisplay;
	[SerializeField] TMP_Text statusDisplay;
	[SerializeField] TMP_Text modeDisplay;
	[SerializeField] float displayTimeAfter;
	[SerializeField] AudioSource source;
	[SerializeField] AudioClip detonationSequenceClip;
	[SerializeField] AudioClip last20Secs;
	[SerializeField] AudioClip cancelClip;
	[SerializeField] AudioClip clickclick;

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
		SetStatus("Ready", "#00ff00");
		SetInfo("Shutdown", "#ff0000");
	}

	public void EngageWarhead() {
		Soundings();

		if (DiscordManager.IsExisting()) {
			DiscordManager.Singleton.SetEngageActivity();
		}

		InterfaceManipulation.Singleton.HideMainMenu();
		isRunning = true;
		counting20Secs = false;

		SetStatus("Running...", "#ff0000");
		source.clip = detonationSequenceClip;
		source.Play();
		InvokeRepeating(nameof(UpdateDiscordActivity), 12.5f, 5f);
	}

	public void CancelWarhead(bool playCancelSound) {
		if (!isRunning) return;
		Soundings();

		Invoke(nameof(ResetTimeUntilDetonation), 1f);
		CancelInvoke(nameof(UpdateDiscordActivity));
		InterfaceManipulation.Singleton.ShowMainMenu();
		if (DiscordManager.IsExisting()) {
			DiscordManager.Singleton.SetWaitingActivity();
		}


		InterfaceManipulation.Singleton.ShowMainMenu();
		isRunning = false;
		source.Stop();

		SetStatus("Restarting...", "#ffff00");
		StartCoroutine(SetStatusReadyAfterDelay());

		if (playCancelSound) {
			source.clip = cancelClip;
			source.Play();
		}
	}

	IEnumerator SetStatusReadyAfterDelay() {
		yield return new WaitForSeconds(20f);
		SetStatus("Ready", "#00ff00");
		if (DiscordManager.IsExisting()) {
			DiscordManager.Singleton.SetStartupActivity();
		}
	}

	void SetStatus(string statusText, string colorHex) {
		statusDisplay.text = $"SYSTEM STATUS: <color={colorHex}>{statusText}</color>";
	}

	void SetInfo(string infoText, string colorHex) {
		modeDisplay.text = $"Warhead Mode: <color={colorHex}>{infoText}</color>";
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
		countdownDisplay.text = RemainingTimeToString(timeUntilDetonation);
	}

	void CountLast20Secs() {
		counting20Secs = true;
		InterfaceManipulation.Singleton.BlinkWarning();
		source.PlayOneShot(last20Secs);
	}

	public void SwitchMode() {
		doShutdown = !doShutdown;
		Soundings();

		if (doShutdown) {
			SetInfo("Shutdown", "#ff0000");
		} else {
			SetInfo("Restart", "#f37907");
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

	void Soundings() {
		source.PlayOneShot(clickclick);
	}
}
