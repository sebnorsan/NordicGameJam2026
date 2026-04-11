using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ToolHolder : MonoBehaviour
{
    public static ToolHolder instance;

	[SerializeField] private List<Tool> toolsHeld = new List<Tool>();
	[SerializeField] private List<int> keyIdHeld = new List<int>();

	private void Awake()
	{
		if (instance != null)
			Destroy(gameObject);
		else
			instance = this;
	}
	private void Start()
	{
		toolsHeld.Add(Tool.None);
	}
	public List<Tool> GetToolsHeld => toolsHeld;

	public void AddTool(Tool tool)
	{
		if (toolsHeld.Contains(tool)) return;

		toolsHeld.Add(tool);
	}
	public void RemoveTool(Tool tool)
	{
		if (toolsHeld.Contains(tool))
			toolsHeld.Remove(tool);
	}
	public bool CheckKey(int id)
	{
		if (keyIdHeld.Contains(id))
		{
			RemoveKey(id);
			return true;
		}
		return false;
	}
	public void AddKey(int id)
	{
		keyIdHeld.Add(id);
	}
	public void RemoveKey(int id)
	{
		keyIdHeld.Remove(id);
	}
}
