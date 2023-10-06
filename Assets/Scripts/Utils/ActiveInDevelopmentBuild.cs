using UnityEngine;

public class ActiveInDevelopmentBuild : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(Debug.isDebugBuild);
    }
}