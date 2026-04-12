using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

public class Evidence : InteractionProgression, IInteractable
{
	[Space(5)]

	[Header("Enable to use progression to pickup, instead of it being instantaneous")]
	[SerializeField] private bool progressionBased;

	[Space(5)]

	public Tool toolToUse;
	public string evidenceName;

	[Space(5)]

	[TextArea]
	public string popupTextOnPickup;

	[Space(10)]

	[SerializeField] private GameObject[] objToDestroy;
	[SerializeField] private GameObject[] objToActivate;

	[Space(5)]

	public int specialToActivate = -1;
	public bool resetSpecial = false;

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

		if (resetSpecial)
			EvidenceManager.instance.ResetSpecialEvidenceEvent();

		if (specialToActivate != -1)
			EvidenceManager.instance.SetSpecialEvidenceEvent(specialToActivate);

		EvidenceManager.instance.RemoveEvidence(this);

		FindAnyObjectByType<Popup>().Fade(popupTextOnPickup);

		foreach (var obj in objToActivate)
		{
			obj.SetActive(true);
		}
		foreach (var obj in objToDestroy)
		{
			Destroy(obj);
		}

		Destroy(gameObject);
	}

	public bool GetToolAvailable(List<Tool> toolsAvailable)
	{
		if (toolsAvailable == null)
		{
			if (toolToUse == Tool.None)
				return true;
			else
				return false;
		}

		if (toolsAvailable.Contains(toolToUse))
			return true;
		else
			return false;
	}
	public bool IsSpecial() => specialToActivate != -1;
}

public enum Tool
{
    None,
    Mop,
	Knife
}