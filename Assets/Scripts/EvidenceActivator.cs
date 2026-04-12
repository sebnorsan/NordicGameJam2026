using UnityEngine;

public class EvidenceActivator : InteractionProgression, IInteractable
{
	[Space(5)]

	[Header("Enable to use progression to pickup, instead of it being instantaneous")]
	[SerializeField] private bool progressionBased;

	[Space(5)]

	public bool specialActivation;
	public GameObject activeToWork;

	[SerializeField] private GameObject[] objToDestroy;

	[SerializeField] private int specialToActivate = -1;

	public bool canInteract { get; set; } = true;

	private void Start()
	{
		canInteract = true;
	}
	public void Interact()
	{
		if (!progressionBased)
		{
			FinishAnimation();
			return;
		}

		if (!canInteract)
			return;

		StartProgression();
	}

	protected override void FinishAnimation()
	{
		if (progressionBased)
			base.FinishAnimation();

		foreach (var obj in objToDestroy)
		{
			Destroy(obj);
		}

		EvidenceManager.instance.ResetSpecialEvidenceEvent();

		if (specialToActivate != -1)
			EvidenceManager.instance.SetSpecialEvidenceEvent(specialToActivate);

		Destroy(gameObject);
	}
}
