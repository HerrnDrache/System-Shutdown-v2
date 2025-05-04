using UnityEngine;

public class DiscordManager : MonoBehaviour {
    public static DiscordManager Singleton { get; private set; }

    const long GAME_ID = 1361108781445943476;

    Discord.Discord discord;
    long time;

    [Header("Startup Activity")]
    [SerializeField] string details;

    void Awake() {
        if (Singleton == null) {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        discord = new Discord.Discord(GAME_ID, (ulong) Discord.CreateFlags.NoRequireDiscord);
        Debug.Log($"[{nameof(DiscordManager)}] Initialised Discord");

        UpdateTime();
        SetStartupActivity();
    }

    void Update() {
        try {
            discord.RunCallbacks();
        } catch {
            Destroy(gameObject);
        }
    }

    public void SetStartupActivity() {
        UpdateActivity(details, true);
    }

    public void SetEngageActivity() {
        UpdateActivity("Detonation Engaged...", true);
    }

    public void SetDetonationActivity() {
        var displayedTime = Mathf.Floor(WarheadController.Singleton.GetTimeUntilDetonation());

        if (displayedTime <= 0) {
            return;
        }

        UpdateActivity($"Detonation in T-{displayedTime}", false);
    }

    public void SetShutdownActivity() {
        UpdateActivity("Detonation completed, User offline since:", true);
    }

    public void SetWaitingActivity() {
        UpdateActivity("System Restarting...", false);
    }

    public void UpdateActivity(string details, bool updateTime) {
        if (updateTime) {
            UpdateTime();
        }

        try {
            var activityManager = discord.GetActivityManager();
            var newActivity = new Discord.Activity {
                Details = details,
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
            Destroy(gameObject);
        }
    }

    void UpdateTime() {
        time = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    void OnApplicationQuit() {
        try {
            discord.Dispose();
            Debug.Log($"[{nameof(DiscordManager)}] Shutting down...");
        } catch {
            Destroy(gameObject);
        }
    }
}