using UnityEngine;

public interface IKitchenObjectParent
{
    public KitchenObject GetKitchenObject();

    public void SetKitchenObject(KitchenObject kitchenObject);

    public bool HasKitchenObject();

    public void ClearKitchenObject();

    public Transform GetKitchenObjectFollowTransform();
}