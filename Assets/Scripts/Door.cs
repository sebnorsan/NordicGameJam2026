using EZCameraShake;
using UnityEngine;

public class Door : MonoBehaviour
{
	public bool locked = false;
	public int keyIdLocked = 0;

	public ParticleSystem smokePfx;

	public GameObject unbrokenDoor;
	public GameObject brokenDoor;

	private void OnTriggerEnter(Collider other)
	{
		if (locked)
			if (!ToolHolder.instance.CheckKey(keyIdLocked))
				return;
			else
				locked = false;

		if (!brokenDoor.activeSelf)
		{
			unbrokenDoor.SetActive(false);
			brokenDoor.SetActive(true);

			smokePfx.Play();

			CameraShaker.Instance.ShakeOnce(3, 9, .1f, 3f);

			foreach (var item in GetComponents<BoxCollider>())
			{
				item.enabled = false;
			}
		}
	}
}
