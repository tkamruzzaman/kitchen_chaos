using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioClipReferencesSO audioClipReferencesSO;

    private float volume = 1f;

    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "_Key_Sound_Effects_Volume";

    private BaseCounter[] baseCounters;
    private CuttingCounter[] cuttingCounters;
    private TrashCounter[] trashCounters;

    private void Awake()
    {
        Instance = this;

        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
    }

    private void Start()
    {
        Player.Instance.OnPickedSomthing += Player_OnPickedSomthing;
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;

        baseCounters = FindObjectsOfType<BaseCounter>();
        foreach (BaseCounter baseCounter in baseCounters)
        {
            baseCounter.OnAnyObjectPlaced += BaseCounter_OnAnyObjectPlaced;
        }
        cuttingCounters = FindObjectsOfType<CuttingCounter>();
        foreach (CuttingCounter cuttingCounter in cuttingCounters)
        {
            cuttingCounter.OnAnyInteraction += CuttingCounter_OnAnyInteraction;
        }
        trashCounters = FindObjectsOfType<TrashCounter>();
        foreach (TrashCounter trashCounter in trashCounters)
        {
            trashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
        }
    }

    private void OnDestroy()
    {
        Player.Instance.OnPickedSomthing -= Player_OnPickedSomthing;
        DeliveryManager.Instance.OnRecipeSuccess -= DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed -= DeliveryManager_OnRecipeFailed;
        foreach (BaseCounter baseCounter in baseCounters)
        {
            baseCounter.OnAnyObjectPlaced -= BaseCounter_OnAnyObjectPlaced;
        }
        foreach (CuttingCounter cuttingCounter in cuttingCounters)
        {
            cuttingCounter.OnAnyInteraction -= CuttingCounter_OnAnyInteraction;
        }
        foreach (TrashCounter trashCounter in trashCounters)
        {
            trashCounter.OnAnyObjectTrashed -= TrashCounter_OnAnyObjectTrashed;
        }
    }

    private void Player_OnPickedSomthing(object sender, EventArgs e)
    {
        PlaySound(audioClipReferencesSO.objectPickup, Player.Instance.transform.position);
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, DeliveryManager.OnRecipeSuccessEventArgs e)
    {
        DeliveryCounter deliveryCounter = e.deliveryCounter;
        PlaySound(audioClipReferencesSO.deliverySuccess, deliveryCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeFailed(object sender, DeliveryManager.OnRecipeFailedEventArgs e)
    {
        DeliveryCounter deliveryCounter = e.deliveryCounter;
        PlaySound(audioClipReferencesSO.deliveryFail, deliveryCounter.transform.position);
    }

    private void BaseCounter_OnAnyObjectPlaced(object sender, EventArgs e)
    {
        BaseCounter baseCounter = (BaseCounter)sender;
        PlaySound(audioClipReferencesSO.objectDrop, baseCounter.transform.position);
    }

    private void CuttingCounter_OnAnyInteraction(object sender, EventArgs e)
    {
        CuttingCounter cuttingCounter = (CuttingCounter)sender;
        PlaySound(audioClipReferencesSO.chop, cuttingCounter.transform.position);
    }

    private void TrashCounter_OnAnyObjectTrashed(object sender, TrashCounter.OnAnyObjectTrashedEventArgs e)
    {
        TrashCounter trashCounter = (TrashCounter)sender;
        KitchenObject kitchenObject = e.kitchenObject;

        AudioClip audioClip = kitchenObject is PlateKitchenObject ? audioClipReferencesSO.trash[0] 
            : audioClipReferencesSO.trash[^1];

        PlaySound(audioClip, trashCounter.transform.position);
    }

    public void PlayFootStepSound(Vector3 position, float volume)
    {
        PlaySound(audioClipReferencesSO.footstep, position, volume);
    }

    public void PlayCountdownSound()
    {
        PlaySound(audioClipReferencesSO.warning, Vector3.zero);
    }

    public void PlayWarningSound(Vector3 position)
    {
        PlaySound(audioClipReferencesSO.warning, position);
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipArray[UnityEngine.Random.Range(0, audioClipArray.Length)], position, volume);
    }
    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume * volumeMultiplier);
    }

    public void ChangeVolume()
    {
        volume += 0.1f;

        if (volume > 1f)
        {
            volume = 0f;
        }

        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float GetVolume() => volume;
}
