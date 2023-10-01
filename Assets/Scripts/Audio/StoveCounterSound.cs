using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    private AudioSource audioSource;
    private StoveCounter stoveCounter;

    private float warningSoundTimer;
    private float warningSoundTimerMax = 0.2f;

    private bool playWarningSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        stoveCounter = GetComponentInParent<StoveCounter>();
    }

    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
        stoveCounter.OnProgessChanged += StoveCounter_OnProgessChanged;
    }

    private void OnDestroy()
    {
        stoveCounter.OnStateChanged -= StoveCounter_OnStateChanged;
        stoveCounter.OnProgessChanged -= StoveCounter_OnProgessChanged;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        bool playSound = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fried;

        if (playSound)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Pause();
        }
    }

    private void StoveCounter_OnProgessChanged(object sender, IHasProgress.OnProgessChangedEventArgs e)
    {
        float burnShowProgressAmount = 0.5f;
        
        playWarningSound = stoveCounter.IsFried() && e.progessNormalized >= burnShowProgressAmount;
    }

    private void Update()
    {
        if (playWarningSound)
        {
            warningSoundTimer -= Time.deltaTime;

            if (warningSoundTimer <= 0f)
            {
                warningSoundTimer = warningSoundTimerMax;

                SoundManager.Instance.PlayWarningSound(stoveCounter.transform.position);
            }
        }
    }
}