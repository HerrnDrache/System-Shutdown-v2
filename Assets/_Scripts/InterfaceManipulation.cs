using System.Runtime.CompilerServices;
using DG.Tweening;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManipulation : MonoBehaviour {
	public static InterfaceManipulation Singleton { get; private set; }
	[Header("Main")]
	[SerializeField] GameObject titeltext;
	[SerializeField] GameObject engageButton;
	[SerializeField] GameObject modeButton;
	[SerializeField] TMP_Text statusDisplay;

	[Header("First Screen")]
	[SerializeField] GameObject countdownText1;
	[SerializeField] GameObject countdownDisplay1;
	[SerializeField] TMP_Text modeDisplay1;
	[SerializeField] TMP_Text versionDisplay1;
	[SerializeField] GameObject warningText1_1;
	[SerializeField] GameObject warningText1_2;

	[Header("Second Screen")]
	[SerializeField] GameObject countdownText2;
	[SerializeField] GameObject countdownDisplay2;
	[SerializeField] TMP_Text modeDisplay2;
	[SerializeField] TMP_Text versionDisplay2;
	[SerializeField] GameObject warningText2_1;
	[SerializeField] GameObject warningText2_2;


	public const string WOS_Version = "3.0.1";

	public TMP_Text CountdownText1 {
		get => countdownDisplay1.GetComponent<TMP_Text>();
	}

	public TMP_Text CountdownText2 {
		get => countdownDisplay2.GetComponent<TMP_Text>();
	}

	bool MultipleScreens {
		get => Display.displays.Length > 1;
	}

	void Awake() {
		Singleton = this;
	}

	void Start() {
		SetStatus("Ready", "#00ff00");
		SetInfo("Shutdown", "#ff0000");
		versionDisplay1.text = versionDisplay2.text ="Running WarheadOS v" + WOS_Version;

		if (MultipleScreens) {
			Display.displays[1].Activate();
		}
    }

    public void SetStatus(string statusText, string colorHex) {
		statusDisplay.text = $"SYSTEM STATUS: <color={colorHex}>{statusText}</color>";
	}

	public void SetInfo(string infoText, string colorHex) {
		modeDisplay1.text = modeDisplay2.text = $"Warhead Mode: <color={colorHex}>{infoText}</color>";
	}

	public void HideMainMenu() {
		KillCurrentTweens();

		engageButton.GetComponent<Button>()
			.interactable = false;
		modeButton.GetComponent<Button>()
			.interactable = false;

		statusDisplay.GetComponent<TMP_Text>()
			.DOFade(0f, 3f);
		engageButton.GetComponent<Image>()
			.DOFade(0f, 3f);
		engageButton.GetComponentInChildren<TMP_Text>()
			.DOFade(0f, 3f);
		modeButton.GetComponent<Image>()
			.DOFade(0f, 3f);
		modeButton.GetComponentInChildren<TMP_Text>()
			.DOFade(0f, 3f);
		titeltext.GetComponent<TMP_Text>()
			.DOFade(0f, 3f)

		.OnComplete(() => {
			DOVirtual.DelayedCall(4f, () => {
				countdownText1.GetComponentInChildren<TMP_Text>()
					.DOFade(1f, 4.25f);
				countdownText2.GetComponentInChildren<TMP_Text>()
					.DOFade(1f, 4.25f)

				.OnComplete(() => {
					countdownDisplay1.GetComponent<TMP_Text>()
						.DOFade(1f, 1.25f);
					countdownDisplay2.GetComponent<TMP_Text>()
						.DOFade(1f, 1.25f);
				});
			});
		});
	}

	public void ShowMainMenu() {
		KillCurrentTweens();

		countdownDisplay1.GetComponent<TMP_Text>()
			.DOFade(0f, 1f);
		countdownDisplay2.GetComponent<TMP_Text>()
			.DOFade(0f, 1f);

		countdownText1.GetComponent<TMP_Text>()
			.DOFade(0f, 1f);
		countdownText2.GetComponent<TMP_Text>()
			.DOFade(0f, 1f)

		.OnComplete(() => {
			titeltext.GetComponent<TMP_Text>()
			.DOFade(1f, 1f);
			statusDisplay.GetComponentInChildren<TMP_Text>()
			.DOFade(1f, 1f)

			.OnComplete(() => {
				DOVirtual.DelayedCall(17f, () => {
					engageButton.GetComponent<Image>()
						.DOFade(1f, 1f);
					engageButton.GetComponentInChildren<TMP_Text>()
						.DOFade(1f, 1f);
					modeButton.GetComponent<Image>()
						.DOFade(1f, 1f);
					modeButton.GetComponentInChildren<TMP_Text>()
						.DOFade(1f, 1f)

					.OnComplete(() => {
						engageButton.GetComponent<Button>()
							.interactable = true;
						modeButton.GetComponent<Button>()
							.interactable = true;
					});
				});
			});
		});
	}

	public void BlinkWarning() {
		if (WarheadController.Singleton.TimeUntilDetonation <= 0.9f) return;

		Invoke(nameof(BlinkWarning), 1f);

		warningText1_1.GetComponent<TMP_Text>()
			.DOFade(1f, 0.25f);
		warningText1_2.GetComponent<TMP_Text>()
			.DOFade(1f, 0.25f);
		warningText2_1.GetComponent<TMP_Text>()
			.DOFade(1f, 0.25f);
		warningText2_2.GetComponent<TMP_Text>()
			.DOFade(1f, 0.25f)

		.OnComplete(() => {
			DOVirtual.DelayedCall(0.5f, () => {
				warningText1_1.GetComponent<TMP_Text>()
					.DOFade(0f, 0.25f);
				warningText1_2.GetComponent<TMP_Text>()
					.DOFade(0f, 0.25f);
				warningText2_1.GetComponent<TMP_Text>()
					.DOFade(0f, 0.25f);
				warningText2_2.GetComponent<TMP_Text>()
					.DOFade(0f, 0.25f);
			});
		});
	}

	void KillCurrentTweens() {
		DOTween.KillAll();
	}
}