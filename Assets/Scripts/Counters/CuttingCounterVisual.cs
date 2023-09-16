using System.Collections;
using System.Collections.Generic;
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
        cuttingCounter.OnInteracted += CuttingCounter_OnInteracted;
    }

    private void CuttingCounter_OnInteracted(object sender, System.EventArgs e)
    {
        animator.SetTrigger(CUT);
    }
}