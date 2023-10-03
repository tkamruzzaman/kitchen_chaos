using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionPlayer : MonoBehaviour
{
    [Range(0f, 3f)]
    [SerializeField] private int playerIndex;
    [SerializeField] private TMP_Text playerReadyText;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Button kickButton;

    private void Awake()
    {
        kickButton.onClick.AddListener(() => 
        {
            PlayerData playerData = MultiplayerGameManager.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            MultiplayerGameManager.Instance.KickPlayer(playerData.clientId);
        });
    }

    private void Start()
    {
        MultiplayerGameManager.Instance.OnPlayerDataNetworkListChanged += NetworkManager_OnPlayerDataNetworkListChanged;
        CharacterSelectionManager.Instance.OnPlayerReadyChanged += CharacterSelectionManager_OnPlayerReadyChanged;
        
        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        UpdatePlayer();
    }

    private void NetworkManager_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void CharacterSelectionManager_OnPlayerReadyChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (MultiplayerGameManager.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();
            PlayerData playerData = MultiplayerGameManager.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            playerReadyText.gameObject.SetActive(
                CharacterSelectionManager.Instance.IsPlayerReady(playerData.clientId));
            playerVisual.SetPlayerColor(MultiplayerGameManager.Instance.GetPlayerColor(playerData.colorId));
        }
        else
        {
            Hide();
        }
    }

    private void Show() => gameObject.SetActive(true);

    private void Hide() => gameObject.SetActive(false);

    private void OnDestroy()
    {
        MultiplayerGameManager.Instance.OnPlayerDataNetworkListChanged -= NetworkManager_OnPlayerDataNetworkListChanged;
        CharacterSelectionManager.Instance.OnPlayerReadyChanged -= CharacterSelectionManager_OnPlayerReadyChanged;
    }
}
