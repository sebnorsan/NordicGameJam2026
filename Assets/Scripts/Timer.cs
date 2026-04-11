using System.Threading;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float timeRemaining = 100;
    public TMP_Text timeDisplayed;

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            CountdownFinished();
        }
    }

    void CountdownFinished()
    {

    }

    
}
