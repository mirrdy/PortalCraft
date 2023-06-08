using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TMP_Text text_Loading;
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
        AudioManager.instance.StopBGM();
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
            text_Loading.text = "Loading... " + (int)(slider_Loding.value * 100) + "%";
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
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(arg0.name.Equals("In Game"))
        {
            StartCoroutine(nameof(MapLoading));
            AudioManager.instance.PlayerBGM("InGame");
        }
        else if(arg0.name == scene_next)
        {
            StartCoroutine(Fade_co(false));
            SceneManager.sceneLoaded -= OnSceneLoaded;
            AudioManager.instance.PlayerBGM("Title");
        }
    }

    IEnumerator MapLoading()
    {
        blockMap = FindObjectOfType<BlockMapGenerator>();
        slider_Loding.value = 0f;
        while (blockMap.progress < 100f)
        {
            yield return null;
            text_Loading.text = "Loading... " + (int)blockMap.progress + "%";
            slider_Loding.value = blockMap.progress * 0.01f;
        }
        StartCoroutine(Fade_co(false));
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadPortal(PortalController portal)
    {
        gameObject.SetActive(true);

        StartCoroutine(LoadPortal_co(portal));
    }

    private IEnumerator LoadPortal_co(PortalController portal)
    {
        text_Loading.text = "Loading... " + 0 + "%";
        slider_Loding.value = 0f;
        yield return StartCoroutine(Fade_co(true));

        // 일단 최소 약 5초는 기다리게 함 (SetActive 조작 시 유니티가 단계별로 제어를 돌려주지 않고 프리징을 걸어서 눈속임용)
        for(int i=0; i<100; i++)
        {
            yield return new WaitForSeconds(0.05f);
            int min = (int)portal.moveProgress < i ? (int)portal.moveProgress : i;
            text_Loading.text = "Loading... " + min + "%";
            slider_Loding.value = min * 0.01f;
        }

        while (portal.moveProgress < 100f)
        {
            yield return null;
            text_Loading.text = "Loading... " + (int)portal.moveProgress + "%";
            slider_Loding.value = portal.moveProgress * 0.01f;
        }

        StartCoroutine(Fade_co(false));
    }
}
