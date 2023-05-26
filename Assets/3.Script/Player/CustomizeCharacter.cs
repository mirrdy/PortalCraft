using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CustomizeCharacter : MonoBehaviour
{
    [SerializeField] private GameObject customChar;

    public Text text_Hair;
    public Text text_Eye;
    public Text text_Mouth;
    public Text text_Mustache;
    public Text text_Body;

    //파츠들
    [SerializeField] private GameObject[] hairParts;
    [SerializeField] private GameObject[] eyeParts;
    [SerializeField] private GameObject[] mouthParts;
    [SerializeField] private GameObject[] mustacheParts;
    [SerializeField] private GameObject[] bodyParts;

    //변수선언
    private int curHair = 1;
    private int curEye = 1;
    private int curMouth = 1;
    private int curMustache = 1;
    private int curBody = 1;

    private int indexHair = 0;
    private int indexEye = 0;
    private int indexMouth = 0;
    private int indexMustache = 0;
    private int indexBody = 0;

    //상수선언
    private const int MAX_HAIR = 14;
    private const int MAX_EYE = 12;
    private const int MAX_MOUTH = 12;
    private const int MAX_MUSTACHE = 13;
    private const int MAX_BODY = 5;

    private string name;
    private PlayerData playerData;
    [SerializeField] private CustomizeClass playerJob;

    [Header("Player Name")]
    [SerializeField] InputField playerName;
    [SerializeField] GameObject button_Check;
    [SerializeField] GameObject image_Check;
    [SerializeField] Text duplicateCheck;

    private void Start()
    {
        customChar = GameObject.FindWithTag("Player");

        hairParts = new GameObject[MAX_HAIR];
        eyeParts = new GameObject[MAX_EYE];
        mouthParts = new GameObject[MAX_MOUTH];
        mustacheParts = new GameObject[MAX_MUSTACHE];
        bodyParts = new GameObject[MAX_BODY];

        Transform head = customChar.transform.GetChild(0).GetChild(23).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(4).GetChild(0);

        for (int i = 0; i < MAX_HAIR; i++)
        {
            hairParts[i] = head.GetChild(0).GetChild(i).gameObject;
        }

        for (int i = 0; i < MAX_EYE; i++)
        {
            eyeParts[i] = head.GetChild(1).GetChild(i).gameObject;
        }

        for (int i = 0; i < MAX_MOUTH; i++)
        {
            mouthParts[i] = head.GetChild(2).GetChild(i).gameObject;
        }

        for (int i = 0; i < MAX_MUSTACHE; i++)
        {
            mustacheParts[i] = head.GetChild(3).GetChild(i).gameObject;
        }

        for (int i = 0; i < MAX_BODY; i++)
        {
            bodyParts[i] = customChar.transform.GetChild(0).GetChild(i).gameObject;
        }
    }

    private void Update()
    {
        text_Hair.text = string.Format("{0}", curHair);
        text_Eye.text = string.Format("{0}", curEye);
        text_Mouth.text = string.Format("{0}", curMouth);
        text_Mustache.text = string.Format("{0}", curMustache);
        text_Body.text = string.Format("{0}", curBody);
    }

    public void Hair_Change_R()
    {
        if (curHair >= MAX_HAIR && indexHair >= MAX_HAIR - 1)
        {
            Debug.Log("마지막 헤어");
        }
        else
        {
            curHair++;

            hairParts[indexHair].SetActive(false);
            indexHair++;
            hairParts[indexHair].SetActive(true);
        }
    }
    public void Hair_Change_L()
    {
        if (curHair <= 1 && indexHair <= 0)
        {
            Debug.Log("마지막 헤어");
        }
        else
        {
            curHair--;

            hairParts[indexHair].SetActive(false);
            indexHair--;
            hairParts[indexHair].SetActive(true);
        }
    }

    public void Eye_Change_R()
    {
        if (curEye >= MAX_EYE && indexEye >= MAX_EYE - 1)
        {
            Debug.Log("마지막 눈");
        }
        else
        {
            curEye++;

            eyeParts[indexEye].SetActive(false);
            indexEye++;
            eyeParts[indexEye].SetActive(true);
        }
    }
    public void Eye_Change_L()
    {
        if (curEye <= 1 && indexEye <= 0)
        {
            Debug.Log("마지막 눈");
        }
        else
        {
            curEye--;

            eyeParts[indexEye].SetActive(false);
            indexEye--;
            eyeParts[indexEye].SetActive(true);
        }
    }

    public void Mouth_Change_R()
    {
        if (curMouth >= MAX_MOUTH && indexMouth >= MAX_MOUTH - 1)
        {
            Debug.Log("마지막 입");
        }
        else
        {
            curMouth++;

            mouthParts[indexMouth].SetActive(false);
            indexMouth++;
            mouthParts[indexMouth].SetActive(true);
        }
    }
    public void Mouth_Change_L()
    {
        if (curMouth <= 1 && indexMouth <= 0)
        {
            Debug.Log("마지막 입");
        }
        else
        {
            curMouth--;

            mouthParts[indexMouth].SetActive(false);
            indexMouth--;
            mouthParts[indexMouth].SetActive(true);
        }
    }

    public void Mustache_Change_R()
    {
        if (curMustache >= MAX_MUSTACHE && indexMustache >= MAX_MUSTACHE - 1)
        {
            Debug.Log("마지막 수염");
        }
        else
        {
            curMustache++;

            mustacheParts[indexMustache].SetActive(false);
            indexMustache++;
            mustacheParts[indexMustache].SetActive(true);
        }
    }
    public void Mustache_Change_L()
    {
        if (curMustache <= 1 && indexMustache <= 0)
        {
            Debug.Log("마지막 수염");
        }
        else
        {
            curMustache--;

            mustacheParts[indexMustache].SetActive(false);
            indexMustache--;
            mustacheParts[indexMustache].SetActive(true);
        }
    }

    public void Body_Change_R()
    {
        if (curBody >= MAX_BODY && indexBody >= MAX_BODY - 1)
        {
            Debug.Log("마지막 몸");
        }
        else
        {
            curBody++;

            bodyParts[indexBody].SetActive(false);
            indexBody++;
            bodyParts[indexBody].SetActive(true);
        }
    }
    public void Body_Change_L()
    {
        if (curBody <= 1 && indexBody <= 0)
        {
            Debug.Log("마지막 몸");
        }
        else
        {
            curBody--;

            bodyParts[indexBody].SetActive(false);
            indexBody--;
            bodyParts[indexBody].SetActive(true);
        }
    }

    public void Confirm_Button()
    {
        DataManager.instance.PlayerDataSet(DataManager.instance.saveNumber);

        playerData = DataManager.instance.playerData;

        playerData.playerName = playerName.text;
        playerData.job = playerJob.job;
        playerData.hair = indexHair;
        playerData.eye = indexEye;
        playerData.mouth = indexMouth;
        playerData.mustache = indexMustache;
        playerData.body = indexBody;

        DataManager.instance.SaveData(playerData, DataManager.instance.saveNumber);

        LoadingSceneManager.Instance.LoadScene("MapTest");
    }

    public void OnInputValueChanged()
    { 
        button_Check.SetActive(false);
        image_Check.SetActive(true);
    }

    public void OnInputClick()
    {
        for(int i = 1; i < 4; i++)
        {
            // 파일의 경로와 이름을 지정
            string filePath = Application.persistentDataPath + "/PlayerData" + i + ".xml";

            if(File.Exists(filePath))
            {
                if(playerName.text.Equals(DataManager.instance.playerData.playerName))
                {
                    button_Check.SetActive(false);
                    image_Check.SetActive(true);

                    StartCoroutine(nameof(TextAlpa_co));
                    break;
                }
            }
            else
            {
                break;
            }
        }
        button_Check.SetActive(true);
        image_Check.SetActive(false);
    }

    IEnumerator TextAlpa_co()
    {
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.unscaledDeltaTime * 3f;
            duplicateCheck.color = new Color(0f, 0f, 0f, timer);
            yield return null;
        }
        while (timer > 0f)
        {
            timer -= Time.unscaledDeltaTime * 3f;
            duplicateCheck.color = new Color(0f, 0f, 0f, timer);
            yield return null;
        }
    }
}
