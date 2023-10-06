using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;
    }

    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;

    private List<KitchenObjectSO> kitchenObjectSOList = new List<KitchenObjectSO>();

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO))
        {
            //not a valid ingredient
            return false;
        }

        if (kitchenObjectSOList.Contains(kitchenObjectSO))
        {
            //already has this type
            return false;
        }
        else
        {
            //unique object
            int kitchenObjectSOIndex = MultiplayerGameManager.Instance.GetKitchenObjectIndex(kitchenObjectSO);
            IngredientAddedToPlateServerRpc(kitchenObjectSOIndex);        
            return true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void IngredientAddedToPlateServerRpc(int kitchenObjectSOIndex)
    {
        IngredientAddedToPlateClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void IngredientAddedToPlateClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = MultiplayerGameManager.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        kitchenObjectSOList.Add(kitchenObjectSO);
        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
        {
            kitchenObjectSO = kitchenObjectSO
        });
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList() => kitchenObjectSOList;
}