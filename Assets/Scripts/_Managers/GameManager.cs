using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    [SerializeField] private Transform playerPrefab;

    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnpaused;
    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameUnpaused;
    public event EventHandler OnLocalPlayerReady;

    private NetworkVariable<State> state = new(State.WaitingToStart);
    private bool isLocalPlayerReady;
    private NetworkVariable<float> countdownToStartTimer = new(3f);
    private NetworkVariable<float> gamePlayingTimer = new(0f);
    [Range(10, 300)]
    [SerializeField] private float gamePlayingTimerMax = 90f;

    private GameInput gameInput;

    private NetworkVariable<bool> isGamePaused = new(false);
    private bool isLocalGamePaused;

    private Dictionary<ulong, bool> playersReadyDictionary = new();
    private Dictionary<ulong, bool> playersPauseDictionary = new();

    private bool autoCheckGamePauseState;

    private void Awake()
    {
        Instance = this;

        playersReadyDictionary.Clear();
        playersPauseDictionary.Clear();
    }

    private void Start()
    {
        gameInput = FindObjectOfType<GameInput>();

        gameInput.OnPauseAction += GameInput_OnPauseAction;
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += NetworkManagerSceneManager_OnLoadEventCompleted;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        autoCheckGamePauseState = true;
    }

    private void NetworkManagerSceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in clientsCompleted)
        {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    private void State_OnValueChanged(State previousValue, State currentValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void IsGamePaused_OnValueChanged(bool previousValue, bool currentValue)
    {
        if (isGamePaused.Value)
        {
            Time.timeScale = 0f;
            OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnMultiplayerGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (state.Value == State.WaitingToStart)
        {
            isLocalPlayerReady = true;
            OnLocalPlayerReady?.Invoke(this, EventArgs.Empty);

            SetPlayerReadyServerRpc();
        }
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

        if(allClientsReady)
        {
            state.Value = State.CountdownToStart;
        }
    }
    


    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    private void Update()
    {
        if (!IsServer) { return; }

        switch (state.Value)
        {
            case State.WaitingToStart:
                break;

            case State.CountdownToStart:

                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value < 0f)
                {
                    state.Value = State.GamePlaying;
                    gamePlayingTimer.Value = gamePlayingTimerMax;
                }
                break;

            case State.GamePlaying:

                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value < 0f)
                {
                    state.Value = State.GameOver;
                }
                break;

            case State.GameOver:
                break;
        }
    }

    private void LateUpdate()
    {
        if (autoCheckGamePauseState)
        {
            autoCheckGamePauseState = false;
            CheckGamePauseState();
        }
    }

    public bool IsWaitingToStart() => state.Value == State.WaitingToStart;

    public bool IsCountdownToStartActive() => state.Value == State.CountdownToStart;

    public bool IsGamePlaying() => state.Value == State.GamePlaying;

    public bool IsGameOver() => state.Value == State.GameOver;

    public bool IsLocalPlayerReady() => isLocalPlayerReady;

    public float GetCountdownToStartTimer() => countdownToStartTimer.Value;

    public float GetGamePlayingTimerNormalized() => 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);

    public float GetGamePlayingTime() => gamePlayingTimer.Value;

    public void TogglePauseGame()
    {
        isLocalGamePaused = !isLocalGamePaused;

        if (isLocalGamePaused)
        {
            PauseGameServerRpc();
            OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            UnpauseGameServerRpc();
            OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playersPauseDictionary[serverRpcParams.Receive.SenderClientId] = true;
        CheckGamePauseState();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playersPauseDictionary[serverRpcParams.Receive.SenderClientId] = false;
        CheckGamePauseState();
    }

    private void CheckGamePauseState()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playersPauseDictionary.ContainsKey(clientId)
                && playersPauseDictionary[clientId])
            {
                //this player with clientId is Paused
                isGamePaused.Value = true;
                return;
            }
        }

        //all players are Unpaused
        isGamePaused.Value = false;
    }

    public override void OnDestroy()
    {
        gameInput.OnPauseAction -= GameInput_OnPauseAction;
        gameInput.OnInteractAction -= GameInput_OnInteractAction;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }

        state.OnValueChanged -= State_OnValueChanged;
        isGamePaused.OnValueChanged -= IsGamePaused_OnValueChanged;

    }
}