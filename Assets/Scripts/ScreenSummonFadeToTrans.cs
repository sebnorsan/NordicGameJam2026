using UnityEngine;

public class ScreenSummonFadeToTrans : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ScreenSummoner.SummonScreen(Color.black, 3f, false);
    }

    
}
