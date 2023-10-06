using UnityEngine;

public class StoveBurnWarningUI : MonoBehaviour
{
    private StoveCounter stoveCounter;

    private void Start()
    {
        stoveCounter = GetComponentInParent<StoveCounter>();

        stoveCounter.OnProgessChanged += StoveCounter_OnProgessChanged;

        Hide();
    }

    private void OnDestroy()
    {
        stoveCounter.OnProgessChanged -= StoveCounter_OnProgessChanged;
    }

    private void StoveCounter_OnProgessChanged(object sender, IHasProgress.OnProgessChangedEventArgs e)
    {
        float burnShowProgressAmount = 0.5f;
        bool show = stoveCounter.IsFried() && e.progessNormalized >= burnShowProgressAmount;

        if (show) { Show(); }
        else { Hide(); }
    }

    private void Show() => gameObject.SetActive(true);

    private void Hide() => gameObject.SetActive(false);
}
