using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Xml.Serialization;

public class SettingData  // Yaml �������� class����
{
    public float bgmSound;  // BGM ���� ������ ����
    public float sfxSound;  // SFX ���� ������ ����
    public int resolutionSize;  // �ػ� ��ȣ ������ ����
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;  // DataManager �̱��� ����
    PlayerData playerData = new PlayerData();  // Player�� ������ Ŭ���� ��������

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

        if (!File.Exists(Path.Combine(Application.persistentDataPath, "settingData.yaml")))  // ���� ���� Ȯ��
        {
            // ������ �ʱ�ȭ
            settingData.bgmSound = 1f;
            settingData.sfxSound = 1f;
            settingData.resolutionSize = 0;

            string yamlData = serializer.Serialize(settingData);  // ������ ����ȭ
            string filePath = Path.Combine(Application.persistentDataPath, "settingData.yaml");  // ������ ������ ��ġ ������ ���� ����

            File.WriteAllText(filePath, yamlData);  // ���Ͽ� ����ȭ �� �����͸� ����
        }
        else
        {
            string filedata = File.ReadAllText(Path.Combine(Application.persistentDataPath, "settingData.yaml"));  // ������ ������ ��ġ ������ ���� ����
            settingData = deserializer.Deserialize<SettingData>(filedata);  // ������ ������ȭ �Ͽ� �����͸� ������ ��
        }
    }

    public void SaveSound(float bgm, float sfx)  // ���� Local ���� �޼ҵ�
    {
        settingData.bgmSound = bgm;
        settingData.sfxSound = sfx;

        string yamlData = serializer.Serialize(settingData);  // ������ ����ȭ
        string filePath = Path.Combine(Application.persistentDataPath, "settingData.yaml");  // ������ ������ ��ġ ������ ���� ����

        File.WriteAllText(filePath, yamlData);  // ���Ͽ� ����ȭ �� �����͸� ����
    }

    public float[] LoadSound()  // ���� ���� �������� �޼ҵ�
    {
        string filedata = File.ReadAllText(Path.Combine(Application.persistentDataPath, "settingData.yaml"));  // ������ ������ ��ġ ������ ���� ����
        settingData = deserializer.Deserialize<SettingData>(filedata);  // ������ ������ȭ �Ͽ� �����͸� ������ ��

        float[] sound = new float[2] { settingData.bgmSound, settingData.sfxSound };

        return sound;
    }

    public int SaveResolution(int resolution)  // �ػ� ���� Local ���� �ϴ� �޼ҵ�
    {
        settingData.resolutionSize = resolution;

        string yamlData = serializer.Serialize(settingData);  // ������ ����ȭ
        string filePath = Path.Combine(Application.persistentDataPath, "settingData.yaml");  // ������ ������ ��ġ ������ ���� ����

        File.WriteAllText(filePath, yamlData);  // ���Ͽ� ����ȭ �� �����͸� ����

        return settingData.resolutionSize;
    }

    public int LoadResolution()  // �ػ��� ������ �������� �޼ҵ�
    {
        string filedata = File.ReadAllText(Path.Combine(Application.persistentDataPath, "settingData.yaml"));  // ������ ������ ��ġ ������ ���� ����
        settingData = deserializer.Deserialize<SettingData>(filedata);  // ������ ������ȭ �Ͽ� �����͸� ������ ��

        return settingData.resolutionSize;
    }


    public void SettingData(int num)  // Player�� ����(xml ������ ����)�� Ȯ�� �ϰ� ������ �����ϰ� ������ Load �ϴ� �޼ҵ�
    {
        // ������ ��ο� �̸��� ����
        string filePath = Application.persistentDataPath + "/PlayerData" + 1 + ".xml";

        if (!File.Exists(filePath))  // xml�� �����Ϳ� ���� ���� �����͸� ���� �ʱ�ȭ ���Ŀ� �����Ͽ� �⺻���� ����
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

            XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));   // xml �����͸� ����ȭ
            using (StreamWriter writer = new StreamWriter(filePath))  // �ش� ������ ���� �ݴ´�.
            {
                serializer.Serialize(writer, playerData);  // ����ȭ �� Data�� ����
            }
        }
        else
        {
            LoadData(num);  // ������ ���� ��� �����͸� �о���� �޼ҵ� ����
        }

    }

    public void LoadData(int num)  // ����� xml�� �����͸� �������� �޼ҵ�
    {
        // ������ ��ο� �̸��� ����
        string filePath = Application.persistentDataPath + "/PlayerData" + num + ".xml";

        // ������ ������ ������ ������ȭ
        XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
        using (StreamReader reader = new StreamReader(filePath))  // ������ ����
        {
            PlayerData deserializedData = (PlayerData)serializer.Deserialize(reader);  // ������ȭ �� �����͸� ����

            // ������ȭ�� ��ũ��Ʈ �����͸� ����մϴ�.
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

    public void SaveData(PlayerData playerData, int num)  // ����� �����͸� �޾ƿ� �ٽ� xml���Ͽ� �����ϴ� �޼ҵ�
    {
        // ������ ��ο� �̸��� ����
        string filePath = Application.persistentDataPath + "/PlayerData" + num + ".xml";

        XmlSerializer serializers = new XmlSerializer(typeof(PlayerData));  // �޾ƿ� �����͸� ����ȭ
        using (StreamWriter writer = new StreamWriter(filePath))  // �����͸� ������ ������ ����
        {
            serializers.Serialize(writer, playerData);  // ����ȭ�� �����͸� ���Ͽ� ����
        }
    }
}
