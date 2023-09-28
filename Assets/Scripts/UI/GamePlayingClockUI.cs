using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour
{
    [SerializeField] private Image timerImage;
    [SerializeField] private TMP_Text timerText;

    private void Update()
    {
        if(GameManager.Instance.IsWaitingToStart() 
            || GameManager.Instance.IsCountdownToStartActive() 
            || GameManager.Instance.IsGameOver())
        {
            timerImage.fillAmount = 0;
            timerText.text = string.Empty;
        }

        if (GameManager.Instance.IsGamePlaying())
        {
            timerImage.fillAmount = GameManager.Instance.GetGamePlayingTimerNormalized();
            timerText.text = Mathf.CeilToInt(GameManager.Instance.GetGamePlayingTime()).ToString();
        }       
    }
}
