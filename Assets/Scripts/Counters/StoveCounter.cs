using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgessChangedEventArgs> OnProgessChanged;

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    [SerializeField] private List<FryingRecipeSO> fryingRecipeSOList;
    [SerializeField] private List<BurningRecipeSO> burningRecipeSOList;

    private NetworkVariable< State> state = new(State.Idle);

    private NetworkVariable< float> fryingTimer = new(0f);
    private NetworkVariable< float> burningTimer = new(0f);

    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
        fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
    }

    public override void OnNetworkDespawn()
    {
        state.OnValueChanged -= State_OnValueChanged;
        fryingTimer.OnValueChanged -= FryingTimer_OnValueChanged;
        burningTimer.OnValueChanged -= BurningTimer_OnValueChanged;
    }

    public void State_OnValueChanged(State previousState, State newState)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state.Value
        });

        if (state.Value == State.Idle || state.Value == State.Burned)
        {
            OnProgessChanged?.Invoke(this, new IHasProgress.OnProgessChangedEventArgs
            {
                progessNormalized = 0
            });
        }
    }

    private void FryingTimer_OnValueChanged(float previourValue, float currentValue) 
    {
        float fryingTimerMax = fryingRecipeSO != null? fryingRecipeSO.fryingTimerMax : 1f;

        OnProgessChanged?.Invoke(this, new IHasProgress.OnProgessChangedEventArgs
        {
            progessNormalized = fryingTimer.Value / fryingTimerMax
        });
    }

    private void BurningTimer_OnValueChanged(float previourValue, float currentValue)
    {
        float burningTimerMax = burningRecipeSO != null? burningRecipeSO.burningTimerMax : 1f;

        OnProgessChanged?.Invoke(this, new IHasProgress.OnProgessChangedEventArgs
        {
            progessNormalized = burningTimer.Value / burningTimerMax
        });
    }

    private void Update()
    {
        if (!IsServer) { return; }

        if (HasKitchenObject())
        {
            switch (state.Value)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer.Value += Time.deltaTime;

                    if (fryingTimer.Value > fryingRecipeSO.fryingTimerMax)
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);

                        state.Value = State.Fried;
                        burningTimer.Value = 0;

                        int kitchenObjectSOIndex = KitchenGameMultiplayer.Instance.GetKitchenObjectIndex(GetKitchenObject().GetKitchenObjectSO());
                        SetBurningRecipeSOClientRpc(kitchenObjectSOIndex);
                    }
                    break;
                case State.Fried:
                    burningTimer.Value += Time.deltaTime;

                    if (burningTimer.Value > burningRecipeSO.burningTimerMax)
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);

                        state.Value = State.Burned;
                    }
                    break;
                case State.Burned:
                    break;
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            //no kitchen object on the counter
            if (player.HasKitchenObject())
            {
                //player is carrying kitchen object
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    //player carrying item that has a recipe
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this);

                    int kitchenObjectSOIndex = KitchenGameMultiplayer.Instance.GetKitchenObjectIndex(kitchenObject.GetKitchenObjectSO());

                    InteractLogicPlacedObjectOnStoveServerRpc(kitchenObjectSOIndex);
                }
            }
            else
            {
                //player not carrying anything
            }
        }
        else
        {
            //kitchen object on the counter
            if (player.HasKitchenObject())
            {
                //player is carrying kitchen object
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // player is holding a Plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());

                        SetStateIdleServerRpc();
                    }
                }
            }
            else
            {
                //player not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);

                SetStateIdleServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        state.Value = State.Idle;
    }


    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlacedObjectOnStoveServerRpc(int kitchenObjectSOIndex)
    {
        state.Value = State.Frying;
        fryingTimer.Value = 0;
        burningTimer.Value = 0;

        SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
    }

    [ClientRpc]
    private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        burningRecipeSO = GetBurningRecipeSOWithInput(kitchenObjectSO);
    }

    public override void InteractAlternate(Player player)
    {
    }

    private bool HasRecipeWithInput(KitchenObjectSO kitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
        return fryingRecipeSO != null;

    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO kitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.output;
        }
        return null;
    }

    private float GetFryingTimerMax(KitchenObjectSO kitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.fryingTimerMax;
        }
        return 0;
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO kitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOList)
        {
            if (fryingRecipeSO.input == kitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO kitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOList)
        {
            if (burningRecipeSO.input == kitchenObjectSO)
            {
                return burningRecipeSO;
            }
        }
        return null;
    }

    public bool IsFried()
    {
        return state.Value == State.Fried;
    }
}
