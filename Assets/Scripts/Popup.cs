using TMPro;
using UnityEngine;

public class Popup : MonoBehaviour
{
    public void Fade(string t)
    {
        GetComponent<TextMeshProUGUI>().text = t;
        GetComponent<Animator>().SetTrigger("Pop");
    }
}
