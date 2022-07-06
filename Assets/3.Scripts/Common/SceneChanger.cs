using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ChangeSceneWithStoryLoading(string sceneName)
    {
        LoadingSceneManager.LoadScene(sceneName, false);
    }

    public void ChangeSceneWithDroneLoading(string sceneName)
    {
        LoadingSceneManager.LoadScene(sceneName);
    }
}
