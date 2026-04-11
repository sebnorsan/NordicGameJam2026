using UnityEngine;

public class MainMenu_Start : MonoBehaviour
{
    private Animator anim;
	bool animFinished = false;

	private void Start()
	{
		anim = GetComponent<Animator>();
	}
	void Update()
    {
		if (animFinished) return;

		if (Input.anyKey)
			anim.SetBool("Held", true);
		else
			anim.SetBool("Held", false);
	}
	public void AE_FinishedProgress()
	{
		if (animFinished) return;

		animFinished = true;

		GetComponentInParent<SceneSwitcher>().GetComponent<Animator>().SetTrigger("Finish");

		ScreenSummoner.SummonScreen(Color.black, 1f, true);
	}
}
