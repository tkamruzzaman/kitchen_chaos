using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanup : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if (MultiplayerGameManager.Instance != null)
        {
            Destroy(MultiplayerGameManager.Instance.gameObject);
        }

        if(LobbyManager.Instance != null)
        {
            Destroy(LobbyManager.Instance.gameObject);
        }
    }
}