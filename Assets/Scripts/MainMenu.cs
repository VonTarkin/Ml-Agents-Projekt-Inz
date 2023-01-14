using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void PlayLevelOnePlayer()
    {
        SceneManager.LoadScene("Race_Track_Player_1");
    }

    public void PlayLevelTwoPlayer()
    {
        SceneManager.LoadScene("Race_Track_Player_2");
    }

    public void PlayLevelThreePlayer()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene())
    }

    public void PlayLevelOneAI()
    {
        SceneManager.LoadScene("Race_Track_Ai_1");
    }

    public void PlayLevelTwoAI()
    {
        SceneManager.LoadScene("Race_Track_Ai_2");
    }

    public void PlayLevelThreeAI()
    {
        SceneManager.LoadScene("Race_Track_Ai_3");
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
