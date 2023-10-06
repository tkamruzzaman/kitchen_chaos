using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start()
    {
        MultiplayerGameManager.Instance.OnTryingToJoinGame += MultiplayerGameManager_OnTryingToJoinGame;
        MultiplayerGameManager.Instance.OnFailedToJoinGame += MultiplayerGameManager_OnFailedToJoinGame;
        Hide();
    }

    private void MultiplayerGameManager_OnTryingToJoinGame(object sender, System.EventArgs e)
    {
        Show();
    }

    private void MultiplayerGameManager_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void Show() => gameObject.SetActive(true);

    private void Hide() => gameObject.SetActive(false);

    private void OnDestroy()
    {
        MultiplayerGameManager.Instance.OnTryingToJoinGame -= MultiplayerGameManager_OnTryingToJoinGame;
        MultiplayerGameManager.Instance.OnFailedToJoinGame -= MultiplayerGameManager_OnFailedToJoinGame;
    }
}
