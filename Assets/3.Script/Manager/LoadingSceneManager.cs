using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class LoadingSceneManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TMP_Text text_Loding;
    [SerializeField] Slider slider_Loding;
    [SerializeField] CanvasGroup canvasGroup;

    public static string scene_next;
    private string fill_Count;

    BlockMapGenerator blockMap;

    private static LoadingSceneManager instance;
    public static LoadingSceneManager Instance
    {
        get
        {
            if (instance == null)
            {
                LoadingSceneManager obj = FindObjectOfType<LoadingSceneManager>();
                if (obj != null)
                {
                    instance = obj;
                }
                else
                {
                    instance = Creat();
                }
            }
            return instance;
        }
    }

    private static LoadingSceneManager Creat()
    {
        return Instantiate(Resources.Load<LoadingSceneManager>("Loading Canvas"));
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

    }

    public void LoadScene(string sceneName)
    {
        gameObject.SetActive(true);
        SceneManager.sceneLoaded += OnSceneLoaded;
        scene_next = sceneName;
        StartCoroutine(nameof(LoadScene_co));
    }

    private IEnumerator LoadScene_co()
    {
        slider_Loding.value = 0f;
        yield return StartCoroutine(Fade_co(true));

        AsyncOperation op = SceneManager.LoadSceneAsync(scene_next);
        op.allowSceneActivation = false;

        float timer = 0f;

        while(!op.isDone)
        {
            yield return null;
            text_Loding.text = "Loading... " + (int)(slider_Loding.value * 100) + "%";
            if (op.progress < 0.9f)
            {
                slider_Loding.value = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                slider_Loding.value = Mathf.Lerp(0.9f, 1f, timer);
                if(slider_Loding.value >= 1f)
                {
                    op.allowSceneActivation = true;
                }
            }
        }
    }

    private IEnumerator Fade_co(bool isFadeIn)
    {
        float timer = 0f;
        while(timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 3f;
            canvasGroup.alpha = isFadeIn ? Mathf.Lerp(0f, 1f, timer) : Mathf.Lerp(1f, 0f, timer);
        }

        if(!isFadeIn)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(arg0.name.Equals("MapTest"))
        {
            StartCoroutine(nameof(MapLoading));
        }
        else if(arg0.name == scene_next)
        {
            StartCoroutine(Fade_co(false));
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    IEnumerator MapLoading()
    {
        blockMap = FindObjectOfType<BlockMapGenerator>();
        slider_Loding.value = 0f;
        while (blockMap.progress < 100f)
        {
            text_Loding.text = "Loading... " + (int)blockMap.progress + "%";
            slider_Loding.value = blockMap.progress/100f;
            yield return null;
        }
        StartCoroutine(Fade_co(false));
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
