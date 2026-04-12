using UnityEngine;
using System.Linq;

public class InteractionHandler : MonoBehaviour
{
	[Header("Raycast")]
	[SerializeField] private float maxDistance = 3f;
	[SerializeField] private float sphereRadius = 0.2f; // forgiving up close
	[SerializeField] private LayerMask ignoreLayers;    // pick layers to ignore
	[SerializeField] private string[] ignoreTags;       // tags to ignore (optional)

	[Header("UI")]
	[SerializeField] private Animator interactionKeyAnimator;

	private PlayerController playerController;
	private GameObject objectInteracting;
	private NPC_Interactable npc;

	public static InteractionHandler singleton;
	[HideInInspector] public bool isTalking;

	// Build include mask by inverting ignored layers
	private int IncludeMask => ~ignoreLayers;

	private void Awake()
	{
		if (singleton != null && singleton != this)
		{
			Destroy(gameObject);
			return;
		}
		singleton = this;
	}

	private void Start()
	{
		playerController = FindAnyObjectByType<PlayerController>();
	}

	void Update()
	{
		// Use camera-based ray; this matches what the player actually sees
		var cam = Camera.main;
		var origin = cam.transform.position;
		var dir = cam.transform.forward;
		Ray ray = new Ray(origin, dir);

		// Visual state (compute once per frame)
		bool hasHit = TryGetFirstValidHit(ray, out RaycastHit hit);
		bool isLooking = hasHit && hit.transform.TryGetComponent<IInteractable>(out var lookInteractable) && lookInteractable.canInteract;
		if (hasHit && hit.transform.TryGetComponent<Evidence>(out var evidence))
		{
			isLooking = evidence.GetToolAvailable(ToolHolder.instance.GetToolsHeld);
			if (evidence.IsSpecial() && EvidenceManager.instance && EvidenceManager.instance.IsSpecialActive())
				isLooking = false;
		}
		if (hasHit && hit.transform.TryGetComponent<EvidenceActivator>(out var activator))
		{
			if (activator.specialActivation && activator.activeToWork.activeSelf)
				isLooking = true;
			else
				isLooking = false;
		}


		if (interactionKeyAnimator)
			interactionKeyAnimator.SetBool("LookingAtInteractable", isLooking);

		// Interact input
		if (Input.GetKeyDown(KeyCode.E))
		{
			if (isTalking)
			{
				npc?.ContinueTalk();
			}
			else
			{
				// Don’t rely on animator flag; use the actual hit we just computed
				if (hasHit && hit.transform.TryGetComponent<IInteractable>(out var interactable) && interactable.canInteract)
				{
					interactable.Interact();
					objectInteracting = hit.transform.gameObject;
				}
			}
		}

		// Cancel progression if we’re no longer looking at the object we were interacting with
		if (!isLooking && objectInteracting != null)
		{
			if (objectInteracting.TryGetComponent<InteractionProgression>(out var progressor))
				progressor.CancelProgression();

			objectInteracting = null;
		}
	}

	// Finds the nearest hit not on ignored layers/tags and not our own player root.
	private bool TryGetFirstValidHit(Ray ray, out RaycastHit bestHit)
	{
		// SphereCastAll is more reliable at close range than a thin Raycast
		var hits = Physics.SphereCastAll(ray, sphereRadius, maxDistance, IncludeMask, QueryTriggerInteraction.Ignore);

		if (hits.Length == 0)
		{
			bestHit = default;
			return false;
		}

		// Sort by distance (RaycastAll/SphereCastAll are NOT guaranteed to be sorted)
		System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

		Transform playerRoot = playerController ? playerController.transform.root : null;

		for (int i = 0; i < hits.Length; i++)
		{
			var h = hits[i];

			// skip our own player hierarchy if needed
			if (playerRoot && h.transform.root == playerRoot)
				continue;

			// tag ignore
			if (ignoreTags != null && ignoreTags.Length > 0 && ignoreTags.Contains(h.collider.tag))
				continue;

			// tiny epsilon to avoid “inside the collider” self-hits
			if (h.distance < 0.01f)
				continue;

			bestHit = h;
			return true;
		}

		bestHit = default;
		return false;
	}

	public void EnterInteraction_NPC(NPC_Interactable temp_npc)
	{
		npc = temp_npc;
		isTalking = true;
		if (playerController) playerController.canMove = false;
	}

	public void ExitInteraction_NPC()
	{
		npc = null;
		isTalking = false;
		if (playerController) playerController.canMove = true;
	}

	public void DisableInteractionKey()
	{
		if (interactionKeyAnimator)
			interactionKeyAnimator.SetBool("LookingAtInteractable", false);
	}
	public void EnableInteractionKey()
	{
		if (interactionKeyAnimator)
			interactionKeyAnimator.SetBool("LookingAtInteractable", true);
	}

	private void OnDrawGizmos()
	{
		var cam = Camera.main;
		if (!cam) return;

		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(cam.transform.position, cam.transform.forward * maxDistance);
		Gizmos.DrawWireSphere(cam.transform.position + cam.transform.forward * maxDistance, sphereRadius);
	}
}
