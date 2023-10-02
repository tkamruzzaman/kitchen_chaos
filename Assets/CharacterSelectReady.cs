using System.Collections.Generic;
using Unity.Netcode;

public class CharacterSelectReady : NetworkBehaviour
{
    private Dictionary<ulong, bool> playersReadyDictionary = new();

    private void Awake()
    {
        playersReadyDictionary.Clear();
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
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
            Loader.LoadSceneNetwork(Loader.Scene.GameScene);
        }
    }
}