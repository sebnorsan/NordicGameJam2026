using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EvidenceManager : MonoBehaviour
{
    public static EvidenceManager instance;

    [SerializeField] private List<Evidence> evidenceOrder;

    [Space(5)]

    //For example bloodied hands / clothes, or the clothes being in the drier
    [SerializeField] private string specialEvidenceActive = "";

    [Space(5)]

    [SerializeField] private EvidenceEvents[] evidenceEvents;

    [Serializable]
    public class EvidenceEvents
    {
        public string evidenceBase;
        public int eventId;
    }
	private void Start()
	{
        var allEvidence = FindObjectsByType<Evidence>();

        foreach (var evidence in allEvidence)
        {
            if (!evidenceOrder.Contains(evidence))
                Debug.LogWarning("Evidence called: " + evidence.gameObject.name + " Has not been put in order!");
        }
	}
	public void SetSpecialEvidenceEvent(int id)
    {
        string eToUse = "";

        if (id != -1)
            foreach (var evidenceEvent in evidenceEvents)
            {
                if (evidenceEvent.eventId == id)
                    eToUse = evidenceEvent.evidenceBase;
            }

        specialEvidenceActive = eToUse;
    }
    public void ResetSpecialEvidenceEvent()
    {
        SetSpecialEvidenceEvent(-1);
    }
    public void RemoveEvidence(Evidence evidence)
    {
        if (evidenceOrder.Contains(evidence))
            evidenceOrder.Remove(evidence);
    }
    public string GetEvidenceRemaining()
    {
        Destroy(gameObject, .1f);

        if (!specialEvidenceActive.IsNullOrEmpty()) 
            return specialEvidenceActive;

        if (evidenceOrder.Count <= 0) 
            return evidenceOrder[0].evidenceName;

        return string.Empty;
    }
}
