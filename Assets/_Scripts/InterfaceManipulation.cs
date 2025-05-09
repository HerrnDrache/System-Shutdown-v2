using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManipulation : MonoBehaviour {
    public static InterfaceManipulation Singleton { get; private set; }

    [SerializeField] GameObject titeltext;
    [SerializeField] GameObject countdownText;
    [SerializeField] GameObject countdownDisplay;
    [SerializeField] GameObject engageButton;
    [SerializeField] GameObject warningText1;
    [SerializeField] GameObject warningText2;
    [SerializeField] TMP_Text statusDisplay;

    void Awake() {
        Singleton = this;
    }

    public void HideEngageButton() {
        KillCurrentTweens();

        engageButton.GetComponent<Button>()
            .interactable = false;
        
        statusDisplay.GetComponent<TMP_Text>()
            .DOFade(0f, 5f);
        engageButton.GetComponent<Image>()
            .DOFade(0f, 3f);
        engageButton.GetComponentInChildren<TMP_Text>()
            .DOFade(0f, 3f);
        titeltext.GetComponent<TMP_Text>()
            .DOFade(0f, 3f)

        .OnComplete(() => {
            DOVirtual.DelayedCall(4f, () => {
                countdownText.GetComponentInChildren<TMP_Text>()
                    .DOFade(1f, 4.25f)

                .OnComplete(() => {
                    countdownDisplay.GetComponent<TMP_Text>()
                        .DOFade(1f, 1.25f);
                });
            });
        });    
    }

    public void ShowEngageButton() {
        KillCurrentTweens();

        countdownDisplay.GetComponent<TMP_Text>()
            .DOFade(0f, 1f);

        countdownText.GetComponent<TMP_Text>()
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
                        .DOFade(1f, 1f)

                    .OnComplete(() => {
                        engageButton.GetComponent<Button>()
                            .interactable = true;
                    });
                });
            });
        });        
    }

    public void BlinkWarning() {
        if (WarheadController.Singleton.GetTimeUntilDetonation() <= 0.9f) { return; }

        Invoke(nameof(BlinkWarning), 1f);

        warningText1.GetComponent<TMP_Text>()
            .DOFade(1f, 0.25f);
        warningText2.GetComponent<TMP_Text>()
            .DOFade(1f, 0.25f)

        .OnComplete(() => {
            DOVirtual.DelayedCall(0.5f, () => {
                warningText1.GetComponent<TMP_Text>()
                    .DOFade(0f, 0.25f);
                warningText2.GetComponent<TMP_Text>()
                    .DOFade(0f, 0.25f);
            });
        });
    }

    void KillCurrentTweens() {
        DOTween.KillAll();
    }
}