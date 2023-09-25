using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour
{
    private ContainerCounter containerCounter;
    private Animator animator;
    private const string OPEN_CLOSE = "OpenClose";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        containerCounter = GetComponentInParent<ContainerCounter>();
    }

    private void Start()
    {
        containerCounter.OnInteracted += ContainerCounter_OnInteracted;
    }

    private void OnDestroy()
    {
        containerCounter.OnInteracted -= ContainerCounter_OnInteracted;
    }

    private void ContainerCounter_OnInteracted(object sender, System.EventArgs e)
    {
        animator.SetTrigger(OPEN_CLOSE);
    }
}
