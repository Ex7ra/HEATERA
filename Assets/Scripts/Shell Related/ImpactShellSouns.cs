
using UnityEngine;
using System.Collections;

public class ImpactShellSounds : MonoBehaviour
{
    public AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayImpactSound(AudioClip clip)
    {
        if (audioSource == null || clip == null)
        {
            Destroy(gameObject);
            return;
        }

        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.Play();

        StartCoroutine(DestroyAfterSound(clip.length));
    }

    IEnumerator DestroyAfterSound(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
