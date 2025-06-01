using UnityEngine;

public static class QuitBlocker {
	static bool CanQuit() {
		return !WarheadController.Singleton.GetIsRunning();
	}
	
	[RuntimeInitializeOnLoadMethod]
	static void AttachQuitBlocker() {
		Application.wantsToQuit += CanQuit;
	}
}
