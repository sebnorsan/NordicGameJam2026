using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialCourtRoomFlow : MonoBehaviour
{
    public GameObject[] objFlow;
    public int currentFlow = 0;

	private bool canFlow = false;


	private void Start()
	{
		ScreenSummoner.SummonScreen(Color.black, 1f, false);

		currentFlow = 0;

		foreach (var obj in objFlow)
		{
			obj.SetActive(false);
		}
	}
	void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canFlow)
        {
			if (objFlow.Length <= currentFlow + 1)
			{
				canFlow = false;
				ChangeScenes();
				return;
			}

			objFlow[currentFlow].SetActive(false);
            currentFlow++;
			objFlow[currentFlow].SetActive(true);
		}
    }
	private void ChangeScenes()
	{
		ScreenSummoner.SummonScreen(Color.black, 2f, true);
		Invoke(nameof(Change), 3f);
	}
	private void Change()
	{
		SceneManager.LoadScene("ApartmentLevel");
	}
	public void AE_FinishInitialAnimation()
	{
		canFlow = true;
	}
}
