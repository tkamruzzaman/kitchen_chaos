using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnJoinFailed;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }

    private Lobby joinedLobby;

    private float heartbeatTimer;
    private float heartbeatTimerMax = 15f;
    private float refreshLobbyListTimer;
    private float refreshLobbyListTimerMax = 3f;

    private const string KEY_RELAY_JOIN_CODE = "_KEY_RELAY_JOIN_CODE";

    private UnityTransport unityTransport;

    [SerializeField] private bool isToTestMultipleLocalBuild;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        InitializeUnityAuthentication();
    }

    private void Start()
    {
        unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    }

    private void Update()
    {
        HandleHeartbeat();
        PriodicallyRefreshLobbyList();
    }

    private async void HandleHeartbeat()
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.LobbyScene.ToString()) { return; }

        if (IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;

            if (heartbeatTimer <= 0)
            {
                heartbeatTimer = heartbeatTimerMax;

                try
                {
                    await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
                }
                catch (LobbyServiceException ex)
                {
                    Debug.Log(ex);
                }
            }
        }
    }

    private void PriodicallyRefreshLobbyList()
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.LobbyScene.ToString()) { return; }
        if (!AuthenticationService.Instance.IsSignedIn) { return; }

        //if player joins a lobby no need to refresh the lobby list
        if (joinedLobby != null) { return; }

        refreshLobbyListTimer -= Time.deltaTime;

        if (refreshLobbyListTimer < 0)
        {
            refreshLobbyListTimer = refreshLobbyListTimerMax;
            ListLobbies();
        }
    }

    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new();
            
            if (Debug.isDebugBuild || isToTestMultipleLocalBuild)
            {
                initializationOptions.SetProfile(UnityEngine.Random.Range(10000, 100000).ToString());          //testing code for testing multiple builds at once
            }
            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MultiplayerGameManager.Instance.MAX_PLAYER_AMOUNT - 1); //max player - host
            return allocation;
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch (RelayServiceException ex)
        {
            Debug.Log(ex);
            return default;
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);

        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MultiplayerGameManager.Instance.MAX_PLAYER_AMOUNT, new CreateLobbyOptions
            {
                IsPrivate = isPrivate
            });

            Allocation allocation = await AllocateRelay();

            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {
                        KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)
                    }
                }
            });

            unityTransport.SetRelayServerData(new RelayServerData(allocation, "dtls"));  //Unity recommends "dtls", some type of encryption

            MultiplayerGameManager.Instance.StartHost();
            Loader.LoadSceneNetwork(Loader.Scene.CharacterSelectionScene);
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void QuickJoin()
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);

        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            unityTransport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));  //Unity recommends "dtls", some type of encryption
            //unityTransport.SetRelayServerData(new RelayServerData(joinAllocation.RelayServer.IpV4, (ushort)joinAllocation.RelayServer.Port, joinAllocation.AllocationIdBytes, joinAllocation.Key, joinAllocation.ConnectionData, joinAllocation.HostConnectionData, true));

            MultiplayerGameManager.Instance.StartClient();
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void JoinWithCode(string lobbyCode)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);

        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            unityTransport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));  //Unity recommends "dtls", some type of encryption

            MultiplayerGameManager.Instance.StartClient();
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void JoinWithId(string lobbyId)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);

        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            unityTransport.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));  //Unity recommends "dtls", some type of encryption

            MultiplayerGameManager.Instance.StartClient();
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void DeleteLobby()
    {
        if (joinedLobby == null) { return; }

        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

            joinedLobby = null;
        }

        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    public async void LeaveLobby()
    {
        if (joinedLobby == null) { return; }

        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

            joinedLobby = null;
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    public async void KickPlayer(string playerId)
    {
        if (IsLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException ex)
            {
                Debug.Log(ex);
            }
        }
    }

    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new()
            {
                Filters = new()
            {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            }
            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs
            {
                lobbyList = queryResponse.Results
            });
        }
        catch (LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    public Lobby GetLobby() => joinedLobby;

    private bool IsLobbyHost() => joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;



}
