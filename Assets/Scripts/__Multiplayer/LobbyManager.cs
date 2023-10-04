using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

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

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        InitializeUnityAuthentication();
    }

    private void Update()
    {
        HandleHeartbeat();
        PriodicallyRefreshLobbyList();
    }

    private async void HandleHeartbeat()
    {
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
        if(!AuthenticationService.Instance.IsSignedIn) { return; }
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
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
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
            await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

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
            await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

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
        catch(LobbyServiceException ex)
        {
            Debug.Log(ex);
        }
    }

    public Lobby GetLobby() => joinedLobby;

    private bool IsLobbyHost() => joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;



}
