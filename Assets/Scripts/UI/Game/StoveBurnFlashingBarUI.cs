using UnityEngine;

public class StoveBurnFlashingBarUI : MonoBehaviour
{
    private StoveCounter stoveCounter;

    private Animator animator;

    private const string IS_FLASHING = "IsFlashing";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        stoveCounter = GetComponentInParent<StoveCounter>();

        stoveCounter.OnProgessChanged += StoveCounter_OnProgessChanged;

        animator.SetBool(IS_FLASHING, false);
    }

    private void OnDestroy()
    {
        stoveCounter.OnProgessChanged -= StoveCounter_OnProgessChanged; //Fix_Me: some times host disconnects and gives null ref ex
    }

    private void StoveCounter_OnProgessChanged(object sender, IHasProgress.OnProgessChangedEventArgs e)
    {
        float burnShowProgressAmount = 0.5f;
        bool show = stoveCounter.IsFried() && e.progessNormalized >= burnShowProgressAmount;

        animator.SetBool(IS_FLASHING, show);
    }
}
