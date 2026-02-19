using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneEvents : MonoBehaviour
{

    public void LoadSceneByName(string LevelName)
    {
        if (LevelName != null) SceneManager.LoadScene(LevelName);
    }

   
    public void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (SceneManager.sceneCountInBuildSettings > nextSceneIndex)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }

        else
        {
            nextSceneIndex = 0;
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    public void LoadPreviousScene()
    {
        int prevSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (SceneManager.sceneCountInBuildSettings < prevSceneIndex)
        {
            SceneManager.LoadScene(prevSceneIndex);
        }

        else
        {
            prevSceneIndex = 0;
            SceneManager.LoadScene(prevSceneIndex);
        }
    }
}
