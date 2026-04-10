using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody))]
public class NPC_Interactable : MonoBehaviour, IInteractable
{
	public ScriptableObject_NPC npcBase;

	public ScriptableObject_NPC_Dialogue nextDialogue;
	public bool canInteract { get; set; } = true;

	private int maxInteractions;
	private int currentInteractions;

	private float maxTime;
	private float currentTime;

	private bool dialogueEventActive = false;

	public event System.Action OnTalkEnded;

	private void Start()
	{
		if (nextDialogue.dialogueEvent_OnStart)
			ApplyDialogueEvent(nextDialogue);
	}
	public void Interact()
	{
		if (!canInteract)
			return;

		StartTalk();
	}
	public void ChangeDialogue(ScriptableObject_NPC_Dialogue d)
	{
		nextDialogue = d;
	}
	private ScriptableObject_NPC_Dialogue savedDialogueEvent;
	private void ApplyDialogueEvent(ScriptableObject_NPC_Dialogue dlg)
	{
		if (dialogueEventActive || !dlg.dialogueEvent_enabled)
			return;

		var tempEvent = dlg.dialogueEvent;

		switch (tempEvent)
		{
			case DialogueEvent.AfterSomeTime:
				maxTime = dlg.dialogueEvent_timeEvent;
				StartTimer();
				break;
			case DialogueEvent.AfterSomeInteractions:
				maxInteractions = dlg.dialogueEvent_interactionsEvent;
				currentInteractions = 0;
				break;
			case DialogueEvent.AfterTalkNullify:
				canInteract = false;
				break;
			default:
				break;
		}

		savedDialogueEvent = dlg.dialogueEvent_continuedDialogue;
		dialogueEventActive = true;
	}

	private AudioClip currentlyPlayingAudio = null;
	private bool exitOnFinish;
	private ScriptableObject_NPC_Dialogue prevDialogue;

	public void ContinueTalk()
	{
		if (initCheck)
			initCheck = false;
		else if(!NPC_Canvas.singleton.CheckIfFinished())
			return;

		if (prevDialogue != null)
		{
			DoDialogueActions(prevDialogue);
			if (!prevDialogue.dialogueEvent_OnStart)
				ApplyDialogueEvent(prevDialogue);
		}

		if (currentlyPlayingAudio != null)
		{
			EventManager.instance.StopThisSound(currentlyPlayingAudio);
			currentlyPlayingAudio = null;
		}

		if (exitOnFinish)
		{
			StopTalk();
			return;
		}

		prevDialogue = nextDialogue;

		NPC_Canvas.singleton.SetDialogue(npcBase.npcName, nextDialogue.dialogue, nextDialogue.affectedWords);

		Sprite picToEnable;

		if (nextDialogue.imageOverride != null)
			picToEnable = nextDialogue.imageOverride;
		else
			picToEnable = npcBase.npcPic;

		NPC_Canvas.singleton.SetNpcPicture(picToEnable);

		if (nextDialogue.npcVoiceLine.audioToPlay != null)
		{
			EventManager.instance.PlayThisSound(nextDialogue.npcVoiceLine);
			currentlyPlayingAudio = nextDialogue.npcVoiceLine.audioToPlay;
		}
		if (nextDialogue.continuedDialogue != null)
			ChangeDialogue(nextDialogue.continuedDialogue);
		else
			exitOnFinish = true;
	}

	bool initCheck = false;
	private void StartTalk()
	{
		initCheck = true;

		InteractionHandler.singleton.EnterInteraction_NPC(this);

		prevDialogue = null;
		exitOnFinish = false;

		NPC_Canvas.singleton.ActivateCanvas();

		CheckDialogueEvents();
		ContinueTalk();
	}
	private void CheckDialogueEvents()
	{
		if (dialogueEventActive)
		{
			if (maxTime >= 1)
			{
				if (isTimerDone)
				{
					ChangeDialogue(savedDialogueEvent);
					dialogueEventActive = false;
				}
			}
			else if (maxInteractions >= 1)
			{
				currentInteractions++;

				if (maxInteractions <= currentInteractions)
				{
					ChangeDialogue(savedDialogueEvent);
					dialogueEventActive = false;
				}
			}
		}
	}
	private void StopTalk()
	{
		InteractionHandler.singleton.ExitInteraction_NPC();
		NPC_Canvas.singleton.DeactivateCanvas();

		OnTalkEnded?.Invoke();
	}

	private void StartTimer()
	{
		currentTime = 0;
		StartCoroutine(Timer());
	}
	private bool isTimerDone;
	private IEnumerator Timer()
	{
		while (maxTime >= currentTime)
		{
			currentTime += Time.deltaTime;
			yield return null;
		}
		isTimerDone = true;
	}
	private void DoDialogueActions(ScriptableObject_NPC_Dialogue dlg)
	{
		foreach (var a in dlg.actions)
			ActionManager.Activate(a);
	}
}
