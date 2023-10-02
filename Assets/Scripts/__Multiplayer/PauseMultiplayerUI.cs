using UnityEngine;

public class PauseMultiplayerUI : MonoBehaviour
{

    private void Start()
    {
        GameManager.Instance.OnMultiplayerGamePaused += GameManager_OnMultiplayerGamePaused;
        GameManager.Instance.OnMultiplayerGameUnpaused += GameManager_OnMultiplayerGameUnpaused;
        
        Hide();
    }

    private void GameManager_OnMultiplayerGamePaused(object sender, System.EventArgs e)
    {
        Show();
    }

    private void GameManager_OnMultiplayerGameUnpaused(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void Show() => gameObject.SetActive(true);

    private void Hide() => gameObject.SetActive(false);

    private void OnDestroy()
    {
        GameManager.Instance.OnMultiplayerGamePaused -= GameManager_OnMultiplayerGamePaused;
        GameManager.Instance.OnMultiplayerGameUnpaused -= GameManager_OnMultiplayerGameUnpaused;
    }
}
