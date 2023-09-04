using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private List<CuttingRecipeSO> cuttingRecipeSOList;

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
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            //there is a kitchenobject here and it can be cut
            KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
            GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);

        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO kitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOList)
        {
            if (cuttingRecipeSO.input == kitchenObjectSO)
            {
                return true;
            }
        }

        return false;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO kitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOList)
        {
            if (cuttingRecipeSO.input == kitchenObjectSO)
            {
                return cuttingRecipeSO.output;
            }
        }
        return null;
    }
}