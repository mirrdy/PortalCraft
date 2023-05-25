using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TMP_Text text_Loding;
    [SerializeField] Image slider_Loding;

    public static string scene_next;

    private void Start()
    {
        StartCoroutine(nameof(LoadScene_co));
    }

    public static void LoadScene(string sceneName)
    {
        scene_next = sceneName;
        SceneManager.LoadScene("Loading");
    }

    IEnumerator LoadScene_co()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(scene_next);
        op.allowSceneActivation = false;
        float timer = 0.0f;

        while(!op.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if (op.progress < 0.9f)
            {
                slider_Loding.fillAmount = Mathf.Lerp(slider_Loding.fillAmount, op.progress, timer);
                text_Loding.text = "Loading... " + (slider_Loding.fillAmount * 100);
                if (slider_Loding.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                slider_Loding.fillAmount = Mathf.Lerp(slider_Loding.fillAmount, 1f, timer);
                if (slider_Loding.fillAmount >= 1.0f)
                {
                    if (Input.anyKeyDown)
                    {
                        op.allowSceneActivation = true;
                        yield break;
                    }
                }
            }
        }
    }
}
