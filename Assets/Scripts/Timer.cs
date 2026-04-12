using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public float timeRemaining = 60;
    public TMP_Text timeDisplayed;
    public int minutes;
    public int seconds;

    bool finished = false;

    void Update()
    {
        if (finished) return;

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
        finished = true;

        timeDisplayed.text = "Cops have arrived at your doorstep...";

        FindAnyObjectByType<PlayerController>().canMove = false;
		ScreenSummoner.SummonScreen(Color.black, 2f, true);
        Invoke(nameof(Change), 3f);
	}
    private void Change()
    {
		SceneManager.LoadScene("CourtRoomFinished");
	}

    
}
