using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
	public static EventManager instance;

	[Header("Audio / Subtitles")]
	[SerializeField] private GameObject audioSourceTemplate;
	[SerializeField] private GameObject subtitleTemplate;
	[SerializeField] private AudioLibrary[] audioLibraries;

	[Header("UI")]
	[Tooltip("If not assigned, SummonScreen will try Resources/Prefabs/Screen")]
	[SerializeField] private GameObject screenSummonPrefab;

	[Header("Legacy (optional)")]
	[Tooltip("Optional reference for legacy bonus room logic")]
	public Scene bonusRoomScene;

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
	}

	private void Start()
	{
		// Kept from old version so those notes remain—commented intentionally.
		// PlayerController player = FindAnyObjectByType<PlayerController>();
		// if (playIntro) StartCoroutine(StartEvent());
	}

	#region ===== Legacy Intro Notes (kept, commented) =====
	// public bool playIntro = true;
	// [SerializeField] private GameObject tower;
	// [SerializeField] private GameObject lights;

	// private IEnumerator StartEvent()
	// {
	//     SummonScreen(Color.black, 1f, false);
	//     player.canMove = false;
	//     tower.SetActive(false);
	//     lights.SetActive(false);
	//     yield return new WaitForSeconds(1.5f);
	//     PlayThisSound("Voicelines", "Tutorial");
	//     yield return new WaitForSeconds(GetThisSound("Voicelines", "Tutorial").audioToPlay.length - 1.2f);
	//     tower.SetActive(true);
	//     lights.SetActive(true);
	//     player.canMove = true;
	// }
	#endregion

	#region ===== PlaySound-Subtitles =====
	[System.Serializable]
	public class AudioLibrary
	{
		public string libName = "New Audio Library";
		[Space(30)] public AudioToPlay[] audios;
	}

	public void RandomizePitchOnSound(string libId, string audioId, float min, float max)
	{
		var src = GetThisSoundInScene(libId, audioId);
		if (src) src.pitch = Random.Range(min, max);
	}
	public void RandomizePitchOnSound(AudioClip clip, float min, float max)
	{
		var audioSources = FindObjectsByType<AudioSource>();
		foreach (var a in audioSources)
			if (a.clip == clip)
				a.pitch = Random.Range(min, max);
	}

	public void PlayThisSound(string libId, string audioId)
	{
		var data = GetThisSound(libId, audioId);
		if (data == null) return;
		SpawnAndPlay(data);
	}
	public void PlayThisSound(AudioToPlay data)
	{
		if (data == null) return;
		SpawnAndPlay(data);
	}

	public void StopThisSound(string libId, string audioId)
	{
		var src = GetThisSoundInScene(libId, audioId);
		if (src) Destroy(src.gameObject);
	}
	public void StopThisSound(AudioClip clip)
	{
		var audioSources = FindObjectsByType<AudioSource>();
		foreach (var a in audioSources)
			if (a.clip == clip)
				Destroy(a.gameObject);
	}

	public AudioToPlay GetThisSound(string libId, string audioId)
	{
		AudioLibrary curLib = null;
		foreach (var lib in audioLibraries)
			if (lib.libName == libId) { curLib = lib; break; }

		if (curLib != null)
			foreach (var audio in curLib.audios)
				if (audio.audioName == audioId)
					return audio;

		Debug.LogWarning("No audio library / audio found");
		return null;
	}

	private AudioSource GetThisSoundInScene(string libId, string audioId)
	{
		var data = GetThisSound(libId, audioId);
		if (data == null) return null;

		var audioSources = FindObjectsByType<AudioSource>();
		foreach (var a in audioSources)
			if (a.clip == data.audioToPlay)
				return a;

		return null;
	}

	private void SpawnAndPlay(AudioToPlay data)
	{
		var go = Instantiate(audioSourceTemplate, transform.position, Quaternion.identity);
		var src = go.GetComponent<AudioSource>();

		src.loop = data.loop;
		src.clip = data.audioToPlay;
		src.volume = data.audioVolume;
		src.spatialBlend = data.spatialBlend;
		src.maxDistance = data.maxDistance;
		src.Play();

		if (!data.loop)
			Destroy(go, data.audioToPlay.length + 1f);

		if (data.subtitlesEnabled && data.subtitles != null)
			foreach (var sub in data.subtitles)
				StartCoroutine(CreateSubtitle(sub.timeStarted, sub.timeDestroyed, sub.subtitleText));
	}

	private IEnumerator CreateSubtitle(float timeUntilStarted, float timeUntilDestroyed, string text)
	{
		yield return new WaitForSeconds(timeUntilStarted);

		var inst = Instantiate(subtitleTemplate, subtitleTemplate.transform.position, Quaternion.identity);
		var tmp = inst.GetComponentInChildren<TextMeshProUGUI>();
		tmp.text = text;

		Destroy(inst, timeUntilDestroyed - timeUntilStarted);
	}
	#endregion

	#region ===== Activate-Deactivate-GameObject/Colliders =====
	public void ActivateObjects(GameObject[] objs)
	{
		foreach (var o in objs) if (o) o.SetActive(true);
	}
	public void DeactivateObjects(GameObject[] objs)
	{
		foreach (var o in objs) if (o) o.SetActive(false);
	}
	public void ActivateColliders(Collider[] cols)
	{
		foreach (var c in cols) if (c) c.enabled = true;
	}
	public void DeactivateColliders(Collider[] cols)
	{
		foreach (var c in cols) if (c) c.enabled = false;
	}
	#endregion

	#region ===== Fog / Skybox (static utilities) =====
	public static void CallDensityChange(float changedDensity)
	{
		CoroutineRunner.instance.StartCoroutine(DensityChangeLerp(changedDensity));
	}
	public static void CallDensityChange(float changedDensity, Color newFogColor)
	{
		RenderSettings.fogColor = newFogColor;
		CoroutineRunner.instance.StartCoroutine(DensityChangeLerp(changedDensity));
	}
	private static IEnumerator DensityChangeLerp(float target)
	{
		while (Mathf.Abs(RenderSettings.fogDensity - target) > 0.001f)
		{
			RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, target, 5f * Time.deltaTime);
			yield return null;
		}
		RenderSettings.fogDensity = target;
	}

	public static void CallSkyboxChange(Material newSkybox)
	{
		if (newSkybox) RenderSettings.skybox = newSkybox;
	}
	public static void CallSkyboxChange(Material newSkybox, Color mainCameraViewportColor)
	{
		if (newSkybox) RenderSettings.skybox = newSkybox;
		if (Camera.main) Camera.main.backgroundColor = mainCameraViewportColor;
	}
	public static void CallSkyboxOff()
	{
		if (Camera.main) Camera.main.clearFlags = CameraClearFlags.SolidColor;
	}
	public static void CallSkyboxOn()
	{
		if (Camera.main) Camera.main.clearFlags = CameraClearFlags.Skybox;
	}
	#endregion

	#region ===== Player =====
	public static void TeleportPlayer(Vector3 tpPos)
	{
		var player = FindAnyObjectByType<PlayerController>();
		if (!player) return;

		player.characterController.enabled = false;
		player.transform.position = tpPos;
		player.characterController.enabled = true;
		player.moveDirection = Vector3.zero;
	}
	#endregion

	#region ===== Scene Management (with transition) =====
	public static void LoadSceneWithTransition(Color screenColor, float lerpTime, string sceneName)
	{
		CoroutineRunner.instance.StartCoroutine(SceneLoader(screenColor, lerpTime, sceneName));
		Object.DontDestroyOnLoad(CoroutineRunner.instance.gameObject);
	}
	public static void LoadSceneWithTransition(Color screenColor, float lerpTime, Scene scene)
	{
		CoroutineRunner.instance.StartCoroutine(SceneLoader(screenColor, lerpTime, scene.name));
		Object.DontDestroyOnLoad(CoroutineRunner.instance.gameObject);
	}
	private static IEnumerator SceneLoader(Color screenColor, float lerpTime, string sceneName)
	{
		ScreenSummoner.SummonScreen(screenColor, lerpTime, true);
		yield return new WaitForSeconds(lerpTime);

		yield return new WaitForSeconds(.5f);
		SceneManager.LoadScene(sceneName);
		yield return null;

		ScreenSummoner.SummonScreen(screenColor, lerpTime, false);
	}
	#endregion
}

[System.Serializable]
public class AudioToPlay
{
	public string audioName = "New Audio";

	[Space(30)] public bool loop = false;

	[Space(15)] public AudioClip audioToPlay;
	[Range(0, 1)] public float audioVolume = 1f;

	[Space(15)]
	[Range(0, 1)] public int spatialBlend = 0;
	[Range(0, 500)] public float maxDistance = 500f;

	[Space(15)] public bool subtitlesEnabled = false;
	public Subtitle[] subtitles;

	[System.Serializable]
	public class Subtitle
	{
		[TextArea] public string subtitleText;
		public float timeStarted = 0f;
		public float timeDestroyed = 1f;
	}
}

public abstract class Event : MonoBehaviour
{
	[Header("Execution Gates")]
	public bool debugOnly = false;                 // kept from old
	public bool onRemote = false;                  // if true, only run when Execute() is called manually
	public bool onCollision = false;               // false = trigger-based, true = collision-based
	public bool onEnable = false;                  // auto-run on enable

	[Tooltip("Skip OnTrigger/OnCollision checks entirely (manual/remote only)")]
	public bool ignoreCollisionTriggerChecks = false;

	[Header("Gizmos")]
	public bool showGizmo = true;
	public Color gizmoColor = Color.white;

	private void OnEnable()
	{
		if (onRemote) return;
		if (!onEnable) return;
		CallEvent();
	}

	protected virtual void OnDrawGizmos()
	{
		if (onRemote) return;
		if (!showGizmo) return;

		Gizmos.color = gizmoColor;

		var col = GetComponent<Collider>();
		if (col && col.isTrigger)
			Gizmos.DrawCube(transform.position, transform.localScale);
	}

	// Public wrapper so external systems can trigger safely
	public void Execute()
	{
		CallEvent();
	}

	protected abstract void CallEvent();

	private void OnTriggerEnter(Collider other)
	{
		if (onRemote) return;
		if (ignoreCollisionTriggerChecks) return;

		var col = GetComponent<Collider>();
		if (!col || onCollision) return; // triggers only when onCollision == false

		if (other.gameObject.CompareTag("GameController"))
			CallEvent();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (onRemote) return;
		if (ignoreCollisionTriggerChecks) return;

		var col = GetComponent<Collider>();
		if (!col || !onCollision) return; // collisions only when onCollision == true

		if (collision.gameObject.CompareTag("GameController"))
			CallEvent();
	}
}
