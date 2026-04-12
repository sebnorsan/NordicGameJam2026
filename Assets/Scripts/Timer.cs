using System.Threading;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float timeRemaining = 30;
    public TMP_Text timeDisplayed;
    public int minutes;
    public int seconds;

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

        minutes = Mathf.FloorToInt(timeRemaining / 60); // FLOORTOINT - runder ned
        seconds = Mathf.FloorToInt(timeRemaining % 60);

        timeDisplayed.text = string.Format("{00:00}:{1:00}", minutes, seconds);

    }

    void CountdownFinished()
    {

    }

    
}
