using System;
using System.Collections.Generic;
using UnityEngine;

public static class ActionManager
{
	static Dictionary<string, GameObject> _lookup = new Dictionary<string, GameObject>();

	public static void Register(string id, GameObject go)
	{
		if (string.IsNullOrEmpty(id)) return;
		_lookup[id] = go;
	}

	public static void Unregister(string id)
	{
		if (string.IsNullOrEmpty(id)) return;
		_lookup.Remove(id);
	}

	public static void Activate(Action action)
	{
		switch (action.actionType)
		{
			case Action.ActionType.EnableObject:
				SetActive(action.targetID, true);
				break;
			case Action.ActionType.DisableObject:
				SetActive(action.targetID, false);
				break;
		}
	}

	public static void SetActive(string id, bool active)
	{
		if (_lookup.TryGetValue(id, out var go))
			go.SetActive(active);
		else
			Debug.LogWarning($"[ActionManager] No target registered with ID '{id}'");
	}
}

[Serializable]
public class Action
{
	public enum ActionType
	{
		EnableObject,
		DisableObject,
	}

	public ActionType actionType;

	[Tooltip("Must match a DialogueTarget.targetID in the scene")]
	public string targetID;
}