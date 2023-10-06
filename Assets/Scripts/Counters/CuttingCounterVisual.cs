using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour
{
    private CuttingCounter cuttingCounter;
    private Animator animator;

    private const string CUT = "Cut";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        cuttingCounter = GetComponentInParent<CuttingCounter>();
    }

    private void Start()
    {
        cuttingCounter.OnCutInteraction += CuttingCounter_OnInteracted;
    }

    private void OnDestroy()
    {
        cuttingCounter.OnCutInteraction -= CuttingCounter_OnInteracted;
    }

    private void CuttingCounter_OnInteracted(object sender, System.EventArgs e)
    {
        animator.SetTrigger(CUT);
    }
}
