using UnityEngine;

public class ScreenSummonFadeToTrans : MonoBehaviour
{
    [SerializeField] private float timeTaken = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ScreenSummoner.SummonScreen(Color.black, timeTaken, false);
    }

    
}
