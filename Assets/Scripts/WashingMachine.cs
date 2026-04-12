using TMPro;
using UnityEngine;

public class WashingMachine : MonoBehaviour
{
	[SerializeField] private TMP_Text tmpText;
	[SerializeField] private float timeToWash = 10f;

	private float currentTime;
	private bool isRunning;

	[SerializeField] private GameObject[] objToDestroy;
	[SerializeField] private GameObject[] objToActivate;

	private void Start()
	{
		StartWash();
	}

	private void Update()
	{
		if (!isRunning) return;

		currentTime -= Time.deltaTime;

		if (currentTime <= 0f)
		{
			currentTime = 0f;
			isRunning = false;
			UpdateVisual();
			WashDone();
			return;
		}

		UpdateVisual();
	}

	public void StartWash()
	{
		currentTime = timeToWash;
		isRunning = true;
		UpdateVisual();
	}

	private void UpdateVisual()
	{
		int seconds = Mathf.FloorToInt(currentTime);
		int centiseconds = Mathf.FloorToInt((currentTime - seconds) * 100f);

		tmpText.text = $"{seconds:00}:{centiseconds:00}";
	}

	private void WashDone()
	{
		Debug.Log("Washing done.");

		tmpText.text = "FINISHED!";
		tmpText.color = new Color32(255, 155, 155, 255);

		foreach (var obj in objToActivate)
		{
			obj.SetActive(true);
		}

		foreach (var obj in objToDestroy)
		{
			Destroy(obj);
		}
	}
}