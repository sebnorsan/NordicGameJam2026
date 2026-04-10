using System.Collections.Generic;
using UnityEngine;

public class EvidenceProgression : InteractionProgression, IInteractable
{
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
		if (!canInteract)
			return;

		StartProgression();
	}

	protected override void FinishAnimation()
	{
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
