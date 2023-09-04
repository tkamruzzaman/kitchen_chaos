using System;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    public event EventHandler OnInteracted;

    public event EventHandler<OnProgessChangedEventArgs> OnProgessChanged;
    public class OnProgessChangedEventArgs: EventArgs
    {
        public float progessNormalized;
    }

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
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;

                    int cuttingProgessMax = GetCuttingProgessMax(GetKitchenObject().GetKitchenObjectSO());

                    OnProgessChanged?.Invoke(this, new OnProgessChangedEventArgs
                    {
                        progessNormalized = (float)cuttingProgress / cuttingProgessMax
                    });
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
            }
            else
            {
                //player not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
                //To solve the bug: when player picks up the object mid-cutting and progress bar remains
                OnProgessChanged?.Invoke(this, new OnProgessChangedEventArgs { 
                    progessNormalized = 0
                });
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            //there is a kitchenobject here and it can be cut
            cuttingProgress++;
            
            OnInteracted?.Invoke(this, EventArgs.Empty);

            int cuttingProgessMax = GetCuttingProgessMax(GetKitchenObject().GetKitchenObjectSO());
            
            OnProgessChanged?.Invoke(this, new OnProgessChangedEventArgs
            {
                progessNormalized = (float)cuttingProgress / cuttingProgessMax
            });
            
            if (cuttingProgress >= cuttingProgessMax)
            {
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
                GetKitchenObject().DestroySelf();
                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
            }
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