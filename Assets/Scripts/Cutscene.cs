using UnityEngine;
using UnityEngine.SceneManagement;

public class Cutscene : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			ScreenSummoner.SummonScreen(Color.black, .5f, true);
			Invoke(nameof(Change), 1f);
		}
	}

	public void Switchthatshit()
	{
		ScreenSummoner.SummonScreen(Color.black, 2f, true);
		Invoke(nameof(Change), 3f);
	}
	private void Change()
	{
		SceneManager.LoadScene("MainMenu");
	}
}
