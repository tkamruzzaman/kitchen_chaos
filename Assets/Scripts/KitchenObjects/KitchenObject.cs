using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectParent kitchenObjectParent;

    private FollowTransform followTransform;

    private void Awake()
    {
        followTransform = GetComponent<FollowTransform>();
    }

    public KitchenObjectSO GetKitchenObjectSO() => kitchenObjectSO;

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
    }

    [ClientRpc]
    private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        if (kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject))
        {
            IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
            
            this.kitchenObjectParent?.ClearKitchenObject();

            this.kitchenObjectParent = kitchenObjectParent;

            if (kitchenObjectParent.HasKitchenObject())
            {
                Debug.LogError("IKitchenObjectParent already has a KitchenObject!");
            }
            kitchenObjectParent.SetKitchenObject(this);

            followTransform.SetTragetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());
        }
        else
        {
            Debug.LogError("KitchenObjectParentNetworkObjectReference not found on server, likely because it already has been destroyed/despawned");
        }
    }

    public IKitchenObjectParent GetKitchenObjectParent() => kitchenObjectParent;

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void ClearKitchenObjectOnParent()
    {
        kitchenObjectParent.ClearKitchenObject();
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        else
        {
            plateKitchenObject = null;
            return false;
        }
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        MultiplayerGameManager.Instance.SpawnKitchenObject(kitchenObjectSO, kitchenObjectParent);
    }

    public static void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        MultiplayerGameManager.Instance.DestroyKitchenObject(kitchenObject);
    }
}