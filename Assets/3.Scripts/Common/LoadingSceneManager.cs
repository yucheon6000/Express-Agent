using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene;

    [SerializeField] Image progressBar;
    [SerializeField] Image[] images;

    [SerializeField] VideoPlayer videoPlayer;
    [SerializeField] GameObject skipMsg;

    [SerializeField] bool testWebVer;

    private void Start()
    {
        if (testWebVer)
        {
            StartCoroutine(LoadSceneWeb());
            return;
        }

#if UNITY_WEBGL 
        StartCoroutine(LoadSceneWeb());
#else
        StartCoroutine(LoadScene());
#endif
    }

    public static void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        nextScene = sceneName;
        SceneManager.LoadScene("Loading Scene");
    }

    IEnumerator LoadScene()
    {
        videoPlayer.gameObject.SetActive(true);
        progressBar.gameObject.SetActive(false);
        foreach (var img in images)
            img.gameObject.SetActive(false);

        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            if (op.progress >= 0.9f)
                break;
            yield return null;
        }

        skipMsg.SetActive(true);

        while (!Input.GetKeyDown(KeyCode.Escape) && videoPlayer.isPlaying)
            yield return null;

        op.allowSceneActivation = true;
        yield break;
    }

    IEnumerator LoadSceneWeb()
    {

        videoPlayer.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(true);
        foreach (var img in images)
            img.gameObject.SetActive(true);

        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;

            progressBar.fillAmount = Mathf.Lerp(0f, 1f, timer / 5);
            if (progressBar.fillAmount == 1.0f)
            {
                op.allowSceneActivation = true;
                yield break;
            }

            Color color = progressBar.color;
            color.a = Mathf.Lerp(1f, 0f, (timer - 4) / 0.5f);
            foreach (var image in images)
            {
                image.color = color;
                progressBar.color = color;
            }
        }
    }
}
