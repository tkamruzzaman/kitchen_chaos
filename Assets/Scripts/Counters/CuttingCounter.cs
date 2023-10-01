using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public event EventHandler OnCutInteraction;
    public event EventHandler OnAnyCutInteraction;

    public event EventHandler<IHasProgress.OnProgessChangedEventArgs> OnProgessChanged;

    [SerializeField] private List<CuttingRecipeSO> cuttingRecipeSOList;

    private int cuttingProgress;

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
                    
                    InteractLogicPlacedObjectOnCounterServerRpc();
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
                    }
                }
            }
            else
            {
                //player not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
                //To solve the bug: when player picks up the object mid-cutting and progress bar remains
                OnProgessChanged?.Invoke(this, new IHasProgress.OnProgessChangedEventArgs
                {
                    progessNormalized = 0
                });
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlacedObjectOnCounterServerRpc()
    {
        InteractLogicPlacedObjectOnCounterClientRpc();
    }

    [ClientRpc]
    private void InteractLogicPlacedObjectOnCounterClientRpc()
    {
        cuttingProgress = 0;

        OnProgessChanged?.Invoke(this, new IHasProgress.OnProgessChangedEventArgs
        {
            progessNormalized = 0f
        });
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            //there is a kitchenobject here and it can be cut
            CutInteractionServerRpc();
            CuttingProgressDoneServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CutInteractionServerRpc()
    {
        CutInteractionClientRpc();
    }

    [ClientRpc]
    private void CutInteractionClientRpc()
    {
        cuttingProgress++;

        OnCutInteraction?.Invoke(this, EventArgs.Empty);
        OnAnyCutInteraction?.Invoke(this, EventArgs.Empty);

        int cuttingProgessMax = GetCuttingProgessMax(GetKitchenObject().GetKitchenObjectSO());

        OnProgessChanged?.Invoke(this, new IHasProgress.OnProgessChangedEventArgs
        {
            progessNormalized = (float)cuttingProgress / cuttingProgessMax
        });

    }

    [ServerRpc(RequireOwnership = false)]
    private void CuttingProgressDoneServerRpc()
    {
        int cuttingProgessMax = GetCuttingProgessMax(GetKitchenObject().GetKitchenObjectSO());

        if (cuttingProgress >= cuttingProgessMax)
        {
            KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
            KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO kitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(kitchenObjectSO);
        return cuttingRecipeSO != null;  
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO kitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(kitchenObjectSO);
        if (cuttingRecipeSO != null)
        {
            return cuttingRecipeSO.output;
        }
        return null;
    }

    private int GetCuttingProgessMax(KitchenObjectSO kitchenObjectSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(kitchenObjectSO);
        if (cuttingRecipeSO != null)
        {
            return cuttingRecipeSO.cuttingProgessMax;
        }
        return 0;
    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(KitchenObjectSO kitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOList)
        {
            if (cuttingRecipeSO.input == kitchenObjectSO)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }
}