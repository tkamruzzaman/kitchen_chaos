using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private Player player;

    private float footStepTimer;
    private float footStepTimerMax = 0.1f;

    [Range(0.1f, 1.0f)]
    [SerializeField] private float footStepVolume = 1.0f;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        footStepTimer -= Time.deltaTime;

        if (footStepTimer < 0)
        {
            footStepTimer = footStepTimerMax;

            if (player.IsWalking)
            {
                SoundManager.Instance.PlayFootStepSound(player.transform.position, footStepVolume);
            }
        }
    }
}
