using System;
using Unity.Netcode;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    public event EventHandler<OnAnyObjectTrashedEventArgs> OnAnyObjectTrashed;
    public class OnAnyObjectTrashedEventArgs: EventArgs
    {
        public KitchenObject kitchenObject;
    }

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            KitchenObject kitchenObject = player.GetKitchenObject();
            
            KitchenObject.DestroyKitchenObject(kitchenObject);

            InteractLogicServerRpc(kitchenObject.GetNetworkObject());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        InteractLogicClientRpc(kitchenObjectNetworkObjectReference);
    }

    [ClientRpc]
    private void InteractLogicClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        if (kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject))
        {
            KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

            OnAnyObjectTrashed?.Invoke(this, new OnAnyObjectTrashedEventArgs
            {
                kitchenObject = kitchenObject
            });
        }
        else
        {
            Debug.LogError("KitchenObjectNetworkObjectReference not found on server, likely because it already has been destroyed/despawned");
        }
    }

    public override void InteractAlternate(Player player)
    {
    }
}
