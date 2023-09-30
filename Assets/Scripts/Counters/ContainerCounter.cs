using System;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnInteracted;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            //player is not carrying anytging
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
            InteractLogicServerRpc();
        }
        else
        {
            //player is carrying kitchenobject
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                //player is holding a Plate
                if (plateKitchenObject.TryAddIngredient(kitchenObjectSO))
                {
                    //placing valid kitchenObjectSO (Bread) item on the plate
                    InteractLogicServerRpc();
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        OnInteracted?.Invoke(this, EventArgs.Empty);
    }

    public override void InteractAlternate(Player player)
    {
    }
}