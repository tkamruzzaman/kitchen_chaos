using System;
using Unity.Netcode;
using UnityEngine;

[SelectionBase]
public abstract class BaseCounter : NetworkBehaviour, IKitchenObjectParent
{
    public EventHandler OnAnyObjectPlaced;

    [SerializeField] private Transform kitchenObjectHoldPoint;
    private KitchenObject kitchenObject;

    public abstract void Interact(Player player);

    public abstract void InteractAlternate(Player player);

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null)
        {
            OnAnyObjectPlaced?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject() => kitchenObject;

    public void ClearKitchenObject() => kitchenObject = null;

    public bool HasKitchenObject() => kitchenObject != null;

    public Transform GetKitchenObjectFollowTransform() => kitchenObjectHoldPoint;

    public NetworkObject GetNetworkObject() => NetworkObject;
}
