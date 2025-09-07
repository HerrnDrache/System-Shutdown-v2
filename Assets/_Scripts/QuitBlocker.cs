using UnityEngine;

public static class QuitBlocker {
	static bool CanQuit() {
		return !WarheadController.Singleton.IsRunning;
	}
	
	[RuntimeInitializeOnLoadMethod]
	static void AttachQuitBlocker() {
		Application.wantsToQuit += CanQuit;
	}
}
