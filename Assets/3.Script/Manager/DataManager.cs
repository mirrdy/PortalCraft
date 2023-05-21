using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Xml.Serialization;

public class SettingData  // Yaml 데이터형 class생성
{
    public float bgmSound;  // BGM 사운드 저장할 변수
    public float sfxSound;  // SFX 사운드 저장할 변수
    public int resolutionSize;  // 해상도 번호 저장할 변수
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;  // DataManager 싱글톤 생성
    PlayerData playerData = new PlayerData();  // Player의 데이터 클래스 가져오기

    // Yaml 데이터형 변수 지정
    SettingData settingData = new SettingData();

    ISerializer serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();  // Yaml의 데이터 직렬화를 하는 변수 생성
    IDeserializer deserializer = new DeserializerBuilder().Build();  // Yaml의 데이터 역직렬화 하는 변수 생성

    private void Awake()
    {
        if(instance == null)  // 싱글톤 설정
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (!File.Exists(Path.Combine(Application.persistentDataPath, "settingData.yaml")))  // 파일 유무 확인
        {
            // 데이터 초기화
            settingData.bgmSound = 1f;
            settingData.sfxSound = 1f;
            settingData.resolutionSize = 0;

            string yamlData = serializer.Serialize(settingData);  // 데이터 직렬화
            string filePath = Path.Combine(Application.persistentDataPath, "settingData.yaml");  // 파일을 생성할 위치 지정후 파일 생성

            File.WriteAllText(filePath, yamlData);  // 파일에 직렬화 한 데이터를 저장
        }
        else
        {
            string filedata = File.ReadAllText(Path.Combine(Application.persistentDataPath, "settingData.yaml"));  // 파일을 생성할 위치 지정후 파일 생성
            settingData = deserializer.Deserialize<SettingData>(filedata);  // 데이터 역직렬화 하여 데이터를 가지고 옴
        }
    }

    public void SaveSound(float bgm, float sfx)  // 사운드 Local 저장 메소드
    {
        settingData.bgmSound = bgm;
        settingData.sfxSound = sfx;

        string yamlData = serializer.Serialize(settingData);  // 데이터 직렬화
        string filePath = Path.Combine(Application.persistentDataPath, "settingData.yaml");  // 파일을 생성할 위치 지정후 파일 생성

        File.WriteAllText(filePath, yamlData);  // 파일에 직렬화 한 데이터를 저장
    }

    public float[] LoadSound()  // 사운드 정보 가져오는 메소드
    {
        string filedata = File.ReadAllText(Path.Combine(Application.persistentDataPath, "settingData.yaml"));  // 파일을 생성할 위치 지정후 파일 생성
        settingData = deserializer.Deserialize<SettingData>(filedata);  // 데이터 역직렬화 하여 데이터를 가지고 옴

        float[] sound = new float[2] { settingData.bgmSound, settingData.sfxSound };

        return sound;
    }

    public int SaveResolution(int resolution)  // 해상도 정보 Local 저장 하는 메소드
    {
        settingData.resolutionSize = resolution;

        string yamlData = serializer.Serialize(settingData);  // 데이터 직렬화
        string filePath = Path.Combine(Application.persistentDataPath, "settingData.yaml");  // 파일을 생성할 위치 지정후 파일 생성

        File.WriteAllText(filePath, yamlData);  // 파일에 직렬화 한 데이터를 저장

        return settingData.resolutionSize;
    }

    public int LoadResolution()  // 해상조 정보를 가져오는 메소드
    {
        string filedata = File.ReadAllText(Path.Combine(Application.persistentDataPath, "settingData.yaml"));  // 파일을 생성할 위치 지정후 파일 생성
        settingData = deserializer.Deserialize<SettingData>(filedata);  // 데이터 역직렬화 하여 데이터를 가지고 옴

        return settingData.resolutionSize;
    }


    public void SettingData(int num)  // Player의 정보(xml 파일의 유무)를 확인 하고 없으면 생성하고 있으면 Load 하는 메소드
    {
        // 파일의 경로와 이름을 지정
        string filePath = Application.persistentDataPath + "/PlayerData" + 1 + ".xml";

        if (!File.Exists(filePath))  // xml의 데이터에 따라서 저장 데이터를 전달 초기화 이후에 대입하여 기본값을 설정
        {
            playerData.staters = new Staters();

            playerData.playerName = " ";
            playerData.playerLevel = 0;
            playerData.playerExp = 0.0f;

            playerData.staters.hp = 100;
            playerData.staters.mp = 50;
            playerData.staters.attackSpeed = 5f;
            playerData.staters.moveSpeed = 3f;
            playerData.staters.skillPoint = 10;
            playerData.staters.statersPoint = 3;
            playerData.staters.attack = 10;
            playerData.staters.defens = 3;

            for (int j = 0; j < playerData.skill.Length; j++)
            {
                playerData.skill[j] = new Skill();

                playerData.skill[j].skillNum = j;
                playerData.skill[j].skillLevel = 0;
            }
            for (int k = 0; k < playerData.inventory.Length; k++)
            {
                playerData.inventory[k] = new Inventory();

                playerData.inventory[k].slot = k;
                playerData.inventory[k].hasItem = false;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));   // xml 데이터를 직렬화
            using (StreamWriter writer = new StreamWriter(filePath))  // 해당 파일을 열고 닫는다.
            {
                serializer.Serialize(writer, playerData);  // 직렬화 한 Data를 저장
            }
        }
        else
        {
            LoadData(num);  // 파일이 있을 경우 데이터를 읽어오는 메소드 실행
        }

    }

    public void LoadData(int num)  // 저장된 xml의 데이터를 가져오는 메소드
    {
        // 파일의 경로와 이름을 지정
        string filePath = Application.persistentDataPath + "/PlayerData" + num + ".xml";

        // 가져올 파일의 정보를 역직렬화
        XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
        using (StreamReader reader = new StreamReader(filePath))  // 파일을 열기
        {
            PlayerData deserializedData = (PlayerData)serializer.Deserialize(reader);  // 역직렬화 된 데이터를 저장

            // 역직렬화된 스크립트 데이터를 사용합니다.
            playerData.staters = new Staters();

            playerData.playerName = deserializedData.playerName;
            playerData.playerLevel = deserializedData.playerLevel;
            playerData.playerExp = deserializedData.playerExp;

            playerData.staters.hp = deserializedData.staters.hp;
            playerData.staters.mp = deserializedData.staters.mp;
            playerData.staters.attackSpeed = deserializedData.staters.attackSpeed;
            playerData.staters.moveSpeed = deserializedData.staters.moveSpeed;
            playerData.staters.skillPoint = deserializedData.staters.skillPoint;
            playerData.staters.statersPoint = deserializedData.staters.statersPoint;
            playerData.staters.attack = deserializedData.staters.attack;
            playerData.staters.defens = deserializedData.staters.defens;

            for (int j = 0; j < playerData.skill.Length; j++)
            {
                playerData.skill[j] = new Skill();

                playerData.skill[j].skillNum = deserializedData.skill[j].skillNum;
                playerData.skill[j].skillLevel = deserializedData.skill[j].skillLevel;
            }
            for (int k = 0; k < playerData.inventory.Length; k++)
            {
                playerData.inventory[k] = new Inventory();

                playerData.inventory[k].slot = deserializedData.inventory[k].slot;
                playerData.inventory[k].hasItem = deserializedData.inventory[k].hasItem;
            }
        }
    }

    public void SaveData(PlayerData playerData, int num)  // 사용한 데이터를 받아와 다시 xml파일에 저장하는 메소드
    {
        // 파일의 경로와 이름을 지정
        string filePath = Application.persistentDataPath + "/PlayerData" + num + ".xml";

        XmlSerializer serializers = new XmlSerializer(typeof(PlayerData));  // 받아온 데이터를 직렬화
        using (StreamWriter writer = new StreamWriter(filePath))  // 데이터를 저장할 파일을 열기
        {
            serializers.Serialize(writer, playerData);  // 직렬화된 데이터를 파일에 저장
        }
    }
}
