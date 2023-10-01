using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;
    public event EventHandler OnLocalPlayerReady;

    private NetworkVariable<State> state = new(State.WaitingToStart);
    private bool isLocalPlayerReady;
    private NetworkVariable<float> countdownToStartTimer = new(3f);
    private NetworkVariable<float> gamePlayingTimer = new(0f);
    [Range(10, 300)]
    [SerializeField] private float gamePlayingTimerMax = 90f;

    private GameInput gameInput;

    private bool isGamePaused;

    private Dictionary<ulong, bool> playersReadyDictionary = new();

    private void Awake()
    {
        Instance = this;

        playersReadyDictionary.Clear();
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
    }

    private void State_OnValueChanged(State previousValue, State currentValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if(state.Value == State.WaitingToStart)
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
        isGamePaused = !isGamePaused;

        if (isGamePaused)
        {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    public override void OnDestroy()
    {
        gameInput.OnPauseAction -= GameInput_OnPauseAction;
        gameInput.OnInteractAction -= GameInput_OnInteractAction;
    }

    public override void OnNetworkDespawn()
    {
        if(!IsServer) { return; }

        state.OnValueChanged -= State_OnValueChanged;
    }
}