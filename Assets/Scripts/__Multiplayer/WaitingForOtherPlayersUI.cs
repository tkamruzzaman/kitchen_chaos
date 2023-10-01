using System;
using UnityEngine;

public class WaitingForOtherPlayersUI : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.OnLocalPlayerReady += GameManager_OnLocalPlayerReady;
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();
    }

    private void GameManager_OnLocalPlayerReady(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsLocalPlayerReady())
        {
            Show();
        }
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsCountdownToStartActive())
        {
            Hide();
        }
    }

    private void Show() => gameObject.SetActive(true);

    private void Hide() => gameObject.SetActive(false);

    private void OnDestroy()
    {
        GameManager.Instance.OnLocalPlayerReady -= GameManager_OnLocalPlayerReady;
        GameManager.Instance.OnStateChanged -= GameManager_OnStateChanged;
    }
}
