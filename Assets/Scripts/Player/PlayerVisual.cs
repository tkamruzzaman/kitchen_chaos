using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private MeshRenderer headMeshRenderer;
    [SerializeField] private MeshRenderer bodyMeshRenderer;

    private Material playerVisualMaterial;

    private void Awake()
    {
        playerVisualMaterial = new Material(headMeshRenderer.material);
        
        headMeshRenderer.material = playerVisualMaterial;
        bodyMeshRenderer.material = playerVisualMaterial;
    }

    public void SetPlayerColor(Color playerColor)
    {
        playerVisualMaterial.color = playerColor;
    }
}
