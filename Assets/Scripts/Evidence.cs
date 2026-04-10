using UnityEngine;
using System.Collections.Generic;

public class Evidence : MonoBehaviour, IInteractable
{
    public Tool toolToUse;
    public string evidenceName;

	[Space(5)]

	[TextArea]
	public string popupTextOnPickup;

	public bool canInteract { get; set; }

	private void Start()
	{
		canInteract = true;
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
	public void Interact()
	{
		Destroy(gameObject);
	}
}

public enum Tool
{
    None,
    Mop
}