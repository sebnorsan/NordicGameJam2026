using UnityEngine;

public class IntroAnimation : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			ScreenSummoner.SummonScreen(Color.black, .5f, true);
			Invoke(nameof(Change), 1f);
		}
	}
	public void AE_FadeToBlack()
    {
        ScreenSummoner.SummonScreen(Color.black, 1f, true);
    }
	private void Change()
	{
		GetComponent<SceneSwitcher>().ChangeScene();
	}
}
