using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TriggeredAudioPlayer : MonoBehaviour
{
    public string ColliderTag = "Player";
    public float triggerRadius = 5f;
    public bool rewindOnPause = false;
    public float rewindSeconds = 5f;
    public bool resetOnTrigger = false;
    public bool is3DAudio = true;
    public float minAudioDistance = 15f;
    public float maxAudioDistance = 15f;

    private AudioSource audioSource;
    private Transform target;
    private bool checkDistance = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ConfigureAudioSource();

        SphereCollider triggerCollider = gameObject.AddComponent<SphereCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.radius = triggerRadius;
    }

    private void OnTriggerEnter(Collider other)
    {

        if ((ColliderTag == "") || other.tag == (ColliderTag))
        {
            target = other.transform;
            //Debug.Log("target " + target);
            checkDistance = true;
            //Debug.Log("checkDistance " + checkDistance);
            PlayAudio();
        }

    }


    private void Update()
    {
        if (checkDistance && target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            //Debug.Log("distance "+ distance);
            if (distance > audioSource.maxDistance)
            {
                PauseAudio();
                checkDistance = false;
            }


         

        }
    }

    private void OnTriggerExit(Collider other)
    {
     
    }

    private void PauseAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            if (rewindOnPause)
            {
                audioSource.time = Mathf.Max(0, audioSource.time - rewindSeconds);
            }
        }
    }

    private void PlayAudio()
    {
        if (resetOnTrigger && !audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.Play();
        }
        else if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

  

    private void ConfigureAudioSource()
    {
        audioSource.playOnAwake = false;
        // audioSource.loop = true; // Set to true or false based on your requirement

        if (is3DAudio)
        {
            audioSource.spatialBlend = 1.0f; // Fully 3D Audio
            audioSource.maxDistance = maxAudioDistance;
            audioSource.minDistance = minAudioDistance;
            // AudioSource.AudioRolloffMode.Linear;
        }
        else
        {
            audioSource.spatialBlend = 0.0f; // Fully 2D Audio
        }

        // Set volume rolloff to linear
        audioSource.rolloffMode = AudioRolloffMode.Linear;
    }
}
