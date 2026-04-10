using UnityEngine;

public class ActionTarget : MonoBehaviour
{
	[Tooltip("Unique key this object responds to.")]
	public string targetID;
	[SerializeField] private bool teleportToPlayer = false;
	private void Awake()
	{
		OnRegister();
		gameObject.SetActive(false);
	}
	private void OnEnable()
	{
		if (teleportToPlayer)
			Invoke(nameof(Teleport), .1f);
	}
	private void OnDisable()
	{
		if (teleportToPlayer)
			CancelInvoke(nameof(Teleport));
	}
	private void Teleport()
	{
		if (teleportToPlayer)
			transform.position = FindAnyObjectByType<PlayerController>().transform.position;
	}
	private void OnDestroy()
	{
		OnUnregister();
	}
	private void OnRegister()
	{
		ActionManager.Register(targetID, gameObject);
	}

	private void OnUnregister()
	{
		ActionManager.Unregister(targetID);
	}
}