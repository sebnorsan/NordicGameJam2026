using UnityEngine;

public class ToolPickup : InteractionProgression, IInteractable
{
	[Space(5)]

	[Header("Enable to use progression to pickup, instead of it being instantaneous")]
	[SerializeField] private bool progressionBased;

	[Space(5)]

	public Tool toolToAquire;


	[Header("If key ID = -1 then no key will be added to the ToolHolder")]

	public int keyIdToAquire = -1;

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

		if (keyIdToAquire != -1)
			ToolHolder.instance.AddKey(keyIdToAquire);
		if (toolToAquire != Tool.None)
			ToolHolder.instance.AddTool(toolToAquire);

		Destroy(gameObject);
	}
}
