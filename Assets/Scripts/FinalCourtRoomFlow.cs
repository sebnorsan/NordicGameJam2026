using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalCourtRoomFlow : MonoBehaviour
{
	public GameObject[] objFlow;
	public int currentFlow = 0;

	private bool canFlow = false;
	private bool canVerdictFlow = false;


	public GameObject[] notguiltyObjFlow;
	public GameObject[] guiltyObjFlow;

	private string guiltyVerdict;

	[SerializeField] private TextMeshProUGUI guiltyText;

	private bool guilty = false;

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
		if (Input.GetKeyDown(KeyCode.E) && (canFlow || canVerdictFlow))
		{
			if (objFlow.Length <= currentFlow + 1 && !canVerdictFlow)
			{
				guiltyVerdict = FindAnyObjectByType<EvidenceManager>().GetEvidenceRemaining();

				guiltyText.text = guiltyText.text.Replace("EVIDENCE", guiltyVerdict);

				if (!guiltyVerdict.IsNullOrEmpty())
					guilty = true;

				canFlow = false;
				canVerdictFlow = true;

				objFlow[currentFlow].SetActive(false);
				currentFlow = 0;

				if (guilty)
					guiltyObjFlow[currentFlow].SetActive(true);
				else
					notguiltyObjFlow[currentFlow].SetActive(true);

				return;
			}
			else if(canVerdictFlow)
			{
				if (guilty && guiltyObjFlow.Length <= currentFlow + 1)
				{
					canVerdictFlow = false;
					ChangeScenes();
					return;
				}
				if (!guilty && notguiltyObjFlow.Length <= currentFlow + 1)
				{
					canVerdictFlow = false;
					ChangeScenes();
					return;
				}
			}

			if (canVerdictFlow)
			{
				if (guilty)
				{
					guiltyObjFlow[currentFlow].SetActive(false);
					currentFlow++;
					guiltyObjFlow[currentFlow].SetActive(true);
				}
				else
				{
					notguiltyObjFlow[currentFlow].SetActive(false);
					currentFlow++;
					notguiltyObjFlow[currentFlow].SetActive(true);
				}
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
		if (guilty)
			//Guilty cutscene
			SceneManager.LoadScene("GuiltyScene");
		else
			//Inoccent cutscene
			SceneManager.LoadScene("NotGuiltyScene");
	}
	public void AE_FinishInitialAnimation()
	{
		canFlow = true;
	}
}
