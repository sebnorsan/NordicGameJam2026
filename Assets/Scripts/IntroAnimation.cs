using UnityEngine;

public class IntroAnimation : MonoBehaviour
{
    public void AE_FadeToBlack()
    {
        ScreenSummoner.SummonScreen(Color.black, 1f, true);
    }
}
