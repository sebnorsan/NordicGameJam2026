using UnityEngine;
using System.Collections.Generic;

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
}

public enum Tool
{
    None,
    Mop,
	Screwdriver,
	Knife
}