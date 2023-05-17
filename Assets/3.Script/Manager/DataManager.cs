using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class SettingData  // Yaml �������� class����
{
    public float bgmSound;  // BGM ���� ������ ����
    public float sfxSound;  // SFX ���� ������ ����
    public int resolutionSize;  // �ػ� ��ȣ ������ ����
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    // Yaml �������� ���� ����
    SettingData settingData = new SettingData();

    ISerializer serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();  // Yaml�� ������ ����ȭ�� �ϴ� ���� ����
    IDeserializer deserializer = new DeserializerBuilder().Build();  // Yaml�� ������ ������ȭ �ϴ� ���� ����

    private void Awake()
    {
        if(instance == null)  // �̱��� ����
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (!File.Exists(Path.Combine(Application.dataPath, "settingData.yaml")))  // ���� ���� Ȯ��
        {
            // ������ �ʱ�ȭ
            settingData.bgmSound = 1f;
            settingData.sfxSound = 1f;
            settingData.resolutionSize = 0;

            string yamlData = serializer.Serialize(settingData);  // ������ ����ȭ
            string filePath = Path.Combine(Application.dataPath, "settingData.yaml");  // ������ ������ ��ġ ������ ���� ����

            File.WriteAllText(filePath, yamlData);  // ���Ͽ� ����ȭ �� �����͸� ����
        }
        else
        {
            string filedata = File.ReadAllText(Path.Combine(Application.dataPath, "settingData.yaml"));  // ������ ������ ��ġ ������ ���� ����
            settingData = deserializer.Deserialize<SettingData>(filedata);  // ������ ������ȭ �Ͽ� �����͸� ������ ��
        }
    }

    public void SaveSound(float bgm, float sfx)
    {
        settingData.bgmSound = bgm;
        settingData.sfxSound = sfx;

        string yamlData = serializer.Serialize(settingData);  // ������ ����ȭ
        string filePath = Path.Combine(Application.dataPath, "settingData.yaml");  // ������ ������ ��ġ ������ ���� ����

        File.WriteAllText(filePath, yamlData);  // ���Ͽ� ����ȭ �� �����͸� ����
    }

    public float[] LoadSound()
    {
        string filedata = File.ReadAllText(Path.Combine(Application.dataPath, "settingData.yaml"));  // ������ ������ ��ġ ������ ���� ����
        settingData = deserializer.Deserialize<SettingData>(filedata);  // ������ ������ȭ �Ͽ� �����͸� ������ ��

        float[] sound = new float[2] { settingData.bgmSound, settingData.sfxSound };

        return sound;
    }

    public int SaveResolution(int resolution)
    {
        settingData.resolutionSize = resolution;

        string yamlData = serializer.Serialize(settingData);  // ������ ����ȭ
        string filePath = Path.Combine(Application.dataPath, "settingData.yaml");  // ������ ������ ��ġ ������ ���� ����

        File.WriteAllText(filePath, yamlData);  // ���Ͽ� ����ȭ �� �����͸� ����

        return settingData.resolutionSize;
    }

    public int LoadResolution()
    {
        string filedata = File.ReadAllText(Path.Combine(Application.dataPath, "settingData.yaml"));  // ������ ������ ��ġ ������ ���� ����
        settingData = deserializer.Deserialize<SettingData>(filedata);  // ������ ������ȭ �Ͽ� �����͸� ������ ��

        return settingData.resolutionSize;
    }

}
