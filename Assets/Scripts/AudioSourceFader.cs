using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceFader : MonoBehaviour
{
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private float fadeDuration = 1f;
	[SerializeField] private float targetVolume = 1f;

	private Coroutine fadeCoroutine;

	private void Awake()
	{
		if (audioSource == null)
			audioSource = GetComponent<AudioSource>();

		FadeIn();
	}

	public void FadeIn()
	{
		FadeIn(fadeDuration);
	}

	public void FadeOut()
	{
		FadeOut(fadeDuration);
	}

	public void FadeIn(float duration)
	{
		if (fadeCoroutine != null)
			StopCoroutine(fadeCoroutine);

		fadeCoroutine = StartCoroutine(FadeAudio(0f, targetVolume, duration, true));
	}

	public void FadeOut(float duration)
	{
		if (fadeCoroutine != null)
			StopCoroutine(fadeCoroutine);

		fadeCoroutine = StartCoroutine(FadeAudio(audioSource.volume, 0f, duration, false));
	}

	private IEnumerator FadeAudio(float startVolume, float endVolume, float duration, bool playOnStart)
	{
		if (playOnStart && !audioSource.isPlaying)
			audioSource.Play();

		audioSource.volume = startVolume;

		float time = 0f;

		while (time < duration)
		{
			time += Time.deltaTime;
			audioSource.volume = Mathf.Lerp(startVolume, endVolume, time / duration);
			yield return null;
		}

		audioSource.volume = endVolume;

		if (endVolume <= 0f)
			audioSource.Stop();

		fadeCoroutine = null;
	}
}