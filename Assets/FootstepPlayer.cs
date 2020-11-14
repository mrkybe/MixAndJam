using UnityEngine;

public class FootstepPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField]
    public AudioClip[] footstepSounds;
    public float StepDistance = 0.4f;
    public float StepDistanceVariation = 0.1f;
    public float SpeedMultiplier = 0.4f;

    private float originalPitch;
    private float originalVolume;

    public float StereoPan = 0.25f;
    private bool leftRight = false;

    public float PitchVariation = 0.1f;
    public float VolumeVariation = 0.1f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = transform.root.GetComponent<AudioSource>();
        }

        originalPitch = audioSource.pitch;
        originalVolume = audioSource.volume;
        lastFootstepPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Footsteps();
    }

    private void PlayRandomFootstepSound()
    {
        audioSource.PlayOneShot(footstepSounds.Random());
    }

    private Vector3 lastFootstepPosition;
    private void Footsteps()
    {
        if ((lastFootstepPosition - transform.position).magnitude > StepDistance)
        {
            lastFootstepPosition = transform.position;

            audioSource.pitch = originalPitch + originalPitch * Random.Range(PitchVariation * -1, PitchVariation);
            audioSource.volume = originalVolume + originalVolume * Random.Range(VolumeVariation * -1, VolumeVariation);

            PlayRandomFootstepSound();
        }
    }
}
