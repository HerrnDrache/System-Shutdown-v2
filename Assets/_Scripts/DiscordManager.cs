using System;
using Unity.VisualScripting;
using UnityEngine;

public class DiscordManager : MonoBehaviour {
	public static DiscordManager Singleton { get; private set; }

	const long GAME_ID = 1361108781445943476;

	Discord.Discord discord;
	long time;

	[SerializeField] string startDetails;

	void Awake() {
		if (Singleton == null) {
			Singleton = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}

	void Start() {
		try {
			discord = new Discord.Discord(GAME_ID, (ulong)Discord.CreateFlags.NoRequireDiscord);

			UpdateTime();
			SetStartupActivity();
		} catch {
			Debug.LogError("Discord Initializing failed");
		}
	}

	void Update() {
		if (discord == null) return;

		try {
			discord.RunCallbacks();
		} catch {
			discord = null;
			Debug.LogError("Discord disconnected");
		}
	}

	public void SetStartupActivity() {
		UpdateActivity("System Ready", false, true);
	}

	public void SetEngageActivity() {
		UpdateActivity("Detonation Engaged...", true);
	}

	public void SetDetonationActivity() {
		var displayedTime = Mathf.Floor(WarheadController.Singleton.TimeUntilDetonation);

		if (displayedTime <= 0) return;

		UpdateActivity($"Detonation in T-{displayedTime}", false);
	}

	public void SetOfflineActivity() {
		if (WarheadController.Singleton.DoShutdown) {
			UpdateActivity("Detonation completed, User offline since:", true);
		} else {
			UpdateActivity("Detonation completed, User returning soon...", true);
		}
		
	}

	public void SetWaitingActivity() {
		UpdateActivity("System Restarting...", false);
	}

	public void UpdateActivity(string details, bool updateTime, bool doState = false) {
		string state = null;

		if (doState) {
			state = WarheadController.Singleton.DoShutdown
				? "Mode: Shutdown"
				: "Mode: Restart";
		}

		Debug.Log("Update: " + WarheadController.Singleton.DoShutdown);

		if (updateTime) {
			UpdateTime();
		}

		try {
			var activityManager = discord.GetActivityManager();
			var newActivity = new Discord.Activity {
				Details = details,
				State = state,
				Timestamps = {
					Start = time
				}
			};

			activityManager.UpdateActivity(newActivity, (res) => {
				if (res != Discord.Result.Ok) {
					Debug.LogWarning($"[{nameof(DiscordManager)}] Couldn't update activity! Error: {res}");
				}
			});
		} catch {
			try {
				TryReinitialize();
				Debug.Log("Discord reconnected!");
			} catch {
				Debug.LogWarning("Reinitializing failed");
			}
		}
	}

	void UpdateTime() {
		time = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
	}

	void OnApplicationQuit() {
		try {
			discord.Dispose();
		} catch {
			Destroy(gameObject);
		}
	}

	public static bool IsExisting() {
		return Singleton != null;
	}

	void TryReinitialize() {
		discord = new Discord.Discord(GAME_ID, (ulong)Discord.CreateFlags.NoRequireDiscord);

		var activityManager = discord.GetActivityManager();
		var newActivity = new Discord.Activity {
			Details = startDetails,
			Timestamps = {
				Start = time
			}
		};
		
		activityManager.UpdateActivity(newActivity, (res) => {
			if (res != Discord.Result.Ok) {
				Debug.LogWarning($"[{nameof(DiscordManager)}] Couldn't update activity! Error: {res}");
			}
		});
	}
}
