using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    private Transform targetTransform;

    public void SetTragetTransform(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }

    private void LateUpdate()
    {
        if (targetTransform == null) { return; }

        transform.SetPositionAndRotation(targetTransform.position, targetTransform.rotation);
    }
}
