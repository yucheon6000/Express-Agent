using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene;
    [SerializeField] Image progressBar;
    [SerializeField] Image[] images;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    public static void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        nextScene = sceneName;
        SceneManager.LoadScene("Loading Scene");
    }

    IEnumerator LoadScene()
    {
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
