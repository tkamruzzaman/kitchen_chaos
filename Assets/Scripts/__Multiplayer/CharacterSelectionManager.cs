using System;
using System.Collections.Generic;
using Unity.Netcode;

public class CharacterSelectionManager : NetworkBehaviour
{
    public static CharacterSelectionManager Instance { get; private set; }  

    public event EventHandler OnPlayerReadyChanged;
    private Dictionary<ulong, bool> playersReadyDictionary = new();

    private void Awake()
    {
        Instance = this;

        playersReadyDictionary.Clear();
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);

        playersReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playersReadyDictionary.ContainsKey(clientId)
                || !playersReadyDictionary[clientId])
            {
                //the player with clientId is not ready
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            LobbyManager.Instance.DeleteLobby();
            Loader.LoadSceneNetwork(Loader.Scene.GameScene);
        }
    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        playersReadyDictionary[clientId] = true;

        OnPlayerReadyChanged?.Invoke(this, new EventArgs());
    }

    public bool IsPlayerReady(ulong clientId) 
        => playersReadyDictionary.ContainsKey(clientId) 
        && playersReadyDictionary[clientId];
}