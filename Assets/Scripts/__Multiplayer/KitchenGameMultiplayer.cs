using System;
using Unity.Netcode;
using UnityEngine;


public class KitchenGameMultiplayer : NetworkBehaviour
{
    public static KitchenGameMultiplayer Instance { get; private set; }

    [SerializeField] private KitchenObjectListSO kitchenObjectListSO; 

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        SpawnKitchenObjectServerRpc(GetKitchenObjectIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);
        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

        if (kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject))
        {
            IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
            kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
        }
        else
        {
            Debug.LogError("KitchenObjectParentNetworkObjectReference not found on server, likely because it already has been destroyed/despawned");
        }
    }

    private int GetKitchenObjectIndex(KitchenObjectSO kitchenObjectSO) 
        => kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);

    private KitchenObjectSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex) 
        => kitchenObjectListSO.kitchenObjectSOList[kitchenObjectSOIndex];

    
    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        if(kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject))
        {
            KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
            
            ClearKitchenObjectParentClientRpc(kitchenObjectNetworkObjectReference);

            kitchenObject.DestroySelf();
        }
        else
        {
            Debug.LogError("KitchenObjectNetworkObjectReference not found on server, likely because it already has been destroyed/despawned");
        }
    }

    [ClientRpc]
    private void ClearKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        if (kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject))
        {
            KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

            kitchenObject.ClearKitchenObjectOnParent();
        }
        else
        {
            Debug.LogError("KitchenObjectNetworkObjectReference not found on server, likely because it already has been destroyed/despawned");
        }
    }
}
