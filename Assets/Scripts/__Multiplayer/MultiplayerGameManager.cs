using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerGameManager : NetworkBehaviour
{
    public static MultiplayerGameManager Instance { get; private set; }

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;

    private const int MAX_PLAYER_AMOUNT = 4;

    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;
    [SerializeField] private List<Color> playerColorList;
    private NetworkList<PlayerData> playerDataNetworkList;


    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerDataNetworkList = new();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;

        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = playerDataNetworkList[i];
            if (playerData.clientId == clientId)
            {
                //This player disconnected
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_ConnectionApprovalCallback(
        NetworkManager.ConnectionApprovalRequest connectionApprovalRequest,
        NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectionScene.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started!";
            return;
        }
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full!";
            return;
        }
        connectionApprovalResponse.Approved = true;
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            clientId = clientId,
            colorId = GetFirstUnusedColorId(),
        });
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        SpawnKitchenObjectServerRpc(GetKitchenObjectIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);
        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

        if (kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject))
        {
            IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
            kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
        }
        else
        {
            Debug.LogError("KitchenObjectParentNetworkObjectReference not found on server, likely because it already has been destroyed/despawned");
        }
    }

    public int GetKitchenObjectIndex(KitchenObjectSO kitchenObjectSO)
        => kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);

    public KitchenObjectSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex)
        => kitchenObjectListSO.kitchenObjectSOList[kitchenObjectSOIndex];


    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        if (kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject))
        {
            KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

            ClearKitchenObjectParentClientRpc(kitchenObjectNetworkObjectReference);

            kitchenObject.DestroySelf();
        }
        else
        {
            Debug.LogError("KitchenObjectNetworkObjectReference not found on server, likely because it already has been destroyed/despawned");
        }
    }

    [ClientRpc]
    private void ClearKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        if (kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject))
        {
            KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

            kitchenObject.ClearKitchenObjectOnParent();
        }
        else
        {
            Debug.LogError("KitchenObjectNetworkObjectReference not found on server, likely because it already has been destroyed/despawned");
        }
    }

    public bool IsPlayerIndexConnected(int playerIndex) => playerIndex < playerDataNetworkList.Count;

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex) => playerDataNetworkList[playerIndex];

    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
            {
                return playerData;
            }
        }
        return default;
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
            {
                return i;
            }
        }
        return -1;
    }

    public PlayerData GetPlayerData() => GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);

    public Color GetPlayerColor(int colorId) => playerColorList[colorId];

    public void ChangePlayerColor(int colorId)
    {
        ChangePlayerColorServerRpc(colorId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
    {
        if (!IsColorAvailable(colorId))
        {
            // Color not available
            return;
        }

        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.colorId = colorId;
        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private bool IsColorAvailable(int colorId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.colorId == colorId)
            {
                // Color is already in use
                return false;
            }
        }
        return true;
    }

    private int GetFirstUnusedColorId()
    {
        for (int i = 0; i < playerColorList.Count; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }
        return default;
    }

    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }
}
