using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class SettingData  // Yaml 데이터형 class생성
{
    public float bgmSound;  // BGM 사운드 저장할 변수
    public float sfxSound;  // SFX 사운드 저장할 변수
    public int resolutionSize;  // 해상도 번호 저장할 변수
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

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

        if (!File.Exists(Path.Combine(Application.dataPath, "settingData.yaml")))  // 파일 유무 확인
        {
            // 데이터 초기화
            settingData.bgmSound = 1f;
            settingData.sfxSound = 1f;
            settingData.resolutionSize = 0;

            string yamlData = serializer.Serialize(settingData);  // 데이터 직렬화
            string filePath = Path.Combine(Application.dataPath, "settingData.yaml");  // 파일을 생성할 위치 지정후 파일 생성

            File.WriteAllText(filePath, yamlData);  // 파일에 직렬화 한 데이터를 저장
        }
        else
        {
            string filedata = File.ReadAllText(Path.Combine(Application.dataPath, "settingData.yaml"));  // 파일을 생성할 위치 지정후 파일 생성
            settingData = deserializer.Deserialize<SettingData>(filedata);  // 데이터 역직렬화 하여 데이터를 가지고 옴
        }
    }

    public void SaveSound(float bgm, float sfx)
    {
        settingData.bgmSound = bgm;
        settingData.sfxSound = sfx;

        string yamlData = serializer.Serialize(settingData);  // 데이터 직렬화
        string filePath = Path.Combine(Application.dataPath, "settingData.yaml");  // 파일을 생성할 위치 지정후 파일 생성

        File.WriteAllText(filePath, yamlData);  // 파일에 직렬화 한 데이터를 저장
    }

    public float[] LoadSound()
    {
        string filedata = File.ReadAllText(Path.Combine(Application.dataPath, "settingData.yaml"));  // 파일을 생성할 위치 지정후 파일 생성
        settingData = deserializer.Deserialize<SettingData>(filedata);  // 데이터 역직렬화 하여 데이터를 가지고 옴

        float[] sound = new float[2] { settingData.bgmSound, settingData.sfxSound };

        return sound;
    }

    public int SaveResolution(int resolution)
    {
        settingData.resolutionSize = resolution;

        string yamlData = serializer.Serialize(settingData);  // 데이터 직렬화
        string filePath = Path.Combine(Application.dataPath, "settingData.yaml");  // 파일을 생성할 위치 지정후 파일 생성

        File.WriteAllText(filePath, yamlData);  // 파일에 직렬화 한 데이터를 저장

        return settingData.resolutionSize;
    }

    public int LoadResolution()
    {
        string filedata = File.ReadAllText(Path.Combine(Application.dataPath, "settingData.yaml"));  // 파일을 생성할 위치 지정후 파일 생성
        settingData = deserializer.Deserialize<SettingData>(filedata);  // 데이터 역직렬화 하여 데이터를 가지고 옴

        return settingData.resolutionSize;
    }

}
