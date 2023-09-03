using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : MonoBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    [SerializeField] private Transform spawnPoint;

    public void Interact()
    {
        print("Interact");
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, spawnPoint);
        kitchenObjectTransform.localPosition = Vector3.zero;

        print(kitchenObjectTransform.GetComponent<KitchenObject>().GetKitchenObjectSO().objectName);
    }
}