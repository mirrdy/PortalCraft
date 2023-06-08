using UnityEngine;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System;

public class SettingData  // Yaml �������� class����
{
    public float bgmSound;  // BGM ���� ������ ����
    public float sfxSound;  // SFX ���� ������ ����
    public int resolutionSize;  // �ػ� ��ȣ ������ ����
    public SaveDataNumber[] dataKey = new SaveDataNumber[3];  // �÷��̾� ���� ���ÿ� ���� Ű ����
}

public class SaveDataNumber
{
    public byte[] playerDataKey;  // �÷��̾� ������ ������ ���� �ݴµ� �ʿ��� ������ ����
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;  // DataManager �̱��� ����
    public PlayerData playerData = new PlayerData();
    public MapData mapData = new MapData();

    // Yaml �������� ���� ����
    SettingData settingData = new SettingData();

    ISerializer serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();  // Yaml�� ������ ����ȭ�� �ϴ� ���� ����
    IDeserializer deserializer = new DeserializerBuilder().Build();  // Yaml�� ������ ������ȭ �ϴ� ���� ����

    private const int KeySize = 256;  // key�� ��Ʈ���� ����
    private const int SaltSize = 16;  // salt�� ��Ʈ ���� ����
    private const int Iterations = 10000;  // �ؽ��� ��� �ݺ����� Ƚ���� ����

    public int saveNumber = 0;

    private void Awake()
    {
        if (instance == null)  // �̱��� ����
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        for (int i = 0; i < settingData.dataKey.Length; i++)
        {
            settingData.dataKey[i] = new SaveDataNumber();
        }

        if (!File.Exists(Path.Combine(Application.persistentDataPath, "settingData.yaml")))  // �������� ���� Ȯ��
        {
            // ������ �ʱ�ȭ
            settingData.bgmSound = 1f;
            settingData.sfxSound = 1f;
            settingData.resolutionSize = 0;

            YamlSet();
        }
        else
        {
            string fileData = File.ReadAllText(Path.Combine(Application.persistentDataPath, "settingData.yaml"));  // ���������� ������ ��ġ ������ ���� ����
            settingData = deserializer.Deserialize<SettingData>(fileData);  // ������ ������ȭ �Ͽ� �����͸� ������ ��
        }
    }

    public void SaveSound(float bgm, float sfx)  // ���� Local ���� �޼ҵ�
    {
        settingData.bgmSound = bgm;
        settingData.sfxSound = sfx;

        YamlSet();
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

        YamlSet();

        return settingData.resolutionSize;
    }

    public int LoadResolution()  // �ػ��� ������ �������� �޼ҵ�
    {
        string filedata = File.ReadAllText(Path.Combine(Application.persistentDataPath, "settingData.yaml"));  // ������ ������ ��ġ ������ ���� ����
        settingData = deserializer.Deserialize<SettingData>(filedata);  // ������ ������ȭ �Ͽ� �����͸� ������ ��

        return settingData.resolutionSize;
    }

    public void PlayerDataSet(int num)  // Player�� ����(xml ������ ����)�� Ȯ�� �ϰ� ������ �����ϰ� ������ Load �ϴ� �޼ҵ�
    {
        // ������ ��ο� �̸��� ����
        string filePath = Application.persistentDataPath + "/PlayerData" + num + ".xml";

        if (!File.Exists(filePath))  // xml�� �����Ϳ� ���� ���� �����͸� ���� �ʱ�ȭ ���Ŀ� �����Ͽ� �⺻���� ����
        {
            playerData.status = new Status();

            playerData.job = null;

            playerData.hair = 0;
            playerData.eye = 0;
            playerData.mouth = 0;
            playerData.mouth = 0;
            playerData.body = 0;

            playerData.playerName = " ";
            playerData.playerLevel = 1;
            playerData.playerExp = 0.0f;

            playerData.status.maxHp = 100;
            playerData.status.currentHp = playerData.status.maxHp;
            playerData.status.maxMp = 100;
            playerData.status.currentMp = playerData.status.maxMp;
            playerData.status.attackSpeed = 1f;
            playerData.status.moveSpeed = 5f;
            playerData.status.skillPoint = 0;
            playerData.status.statusPoint = 5;
            playerData.status.attack = 10;
            playerData.status.defens = 5;

            for (int j = 0; j < playerData.skill.Length; j++)
            {
                playerData.skill[j] = new Skill();

                playerData.skill[j].skillNum = 0;
                playerData.skill[j].skillLevel = 0;
                playerData.skill[j].hasSkill = false;
            }
            for (int k = 0; k < playerData.inventory.Length; k++)
            {
                playerData.inventory[k] = new Inventory();

                playerData.inventory[k].tag = 0;
                playerData.inventory[k].type = null;
                playerData.inventory[k].quantity = 0;
                playerData.inventory[k].hasItem = false;
            }

            // XML �����͸� ���ڿ��� ����ȭ
            string serializedData = SerializeData(playerData);

            // ��ȣȭ Ű ����
            byte[] key = CreateKey(serializedData);

            settingData.dataKey[num - 1].playerDataKey = key;  // Ű�� ������ ����

            YamlSet();  // yaml ������ ����

            // ��ȣȭ�� ������ ����
            byte[] encryptData = Encrypt(serializedData, key);

            SaveEncryptDataFile(encryptData, filePath);
        }
        else
        {
            playerData = PlayerDataGet(num);  // ������ ���� ��� �����͸� �о���� �޼ҵ� ����
        }
    }

    public PlayerData PlayerDataGet(int num)  // ����� xml�� �����͸� �������� �޼ҵ�
    {
        // ������ ��ο� �̸��� ����
        string filePath = Application.persistentDataPath + "/PlayerData" + num + ".xml";

        // ��ȣȭ�� �����͸� ���Ͽ��� �ҷ���
        byte[] encryptedData = LoadEncryptDataFile(filePath);

        // ��ȣȭ�� �����͸� ��ȣȭ�Ͽ� XML �����ͷ� ��ȯ
        string decryptedData = Decrypt(encryptedData, settingData.dataKey[num - 1].playerDataKey);

        // XML �����͸� ������ȭ�Ͽ� ��ü�� ��ȯ
        PlayerData playerData = DeserializeData(decryptedData);

        return playerData;
    }

    public void SaveData(PlayerData playerData, int num)  // ����� �����͸� �޾ƿ� �ٽ� xml���Ͽ� �����ϴ� �޼ҵ�
    {
        // ������ ��ο� �̸��� ����
        string filePath = Application.persistentDataPath + "/PlayerData" + num + ".xml";

        // XML �����͸� ���ڿ��� ����ȭ
        string serializedData = SerializeData(playerData);

        // ��ȣȭ Ű ����
        byte[] key = CreateKey(serializedData);

        settingData.dataKey[num - 1].playerDataKey = key;  // Ű�� ������ ����

        YamlSet();  // yaml ������ ����

        // ��ȣȭ�� ������ ����
        byte[] encryptedData = Encrypt(serializedData, key);

        SaveEncryptDataFile(encryptedData, filePath);
    }

    private static string SerializeData(PlayerData data)  // �ؽ�Ʈ �����͸� ����ȭ ���ִ� �޼ҵ�
    {
        XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
        using (StringWriter writer = new StringWriter())
        {
            serializer.Serialize(writer, data);
            return writer.ToString();
        }
    }

    private static PlayerData DeserializeData(string data)  // �ؽ�Ʈ �����͸� ������ȭ ���ִ� �޼ҵ�
    {
        XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
        using (StringReader reader = new StringReader(data))
        {
            PlayerData playerData = (PlayerData)serializer.Deserialize(reader);

            return playerData;
        }
    }

    private static void SaveEncryptDataFile(byte[] encryptedData, string filePath)
    {
        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        {
            fileStream.Write(encryptedData, 0, encryptedData.Length);
        }
    }

    private static byte[] LoadEncryptDataFile(string filePath)
    {
        byte[] encryptedData;
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        {
            encryptedData = new byte[fileStream.Length];
            fileStream.Read(encryptedData, 0, encryptedData.Length);
        }
        return encryptedData;
    }

    private void YamlSet()
    {
        string yamlData = serializer.Serialize(settingData);  // ������ ����ȭ
        string settingfilePath = Path.Combine(Application.persistentDataPath, "settingData.yaml");  // ������ ������ ��ġ ������ ���� ����

        File.WriteAllText(settingfilePath, yamlData);  // ���Ͽ� ����ȭ �� �����͸� ����
    }

    private byte[] CreateKey(string data)  // Ű�� ���� ���ִ� �޼ҵ�
    {
        // ��Ʈ ����
        byte[] salt = CreateSalt();

        // PBKDF2�� ����Ͽ� Ű �Ļ�
        byte[] key = SyntheticKey(data, salt, Iterations, KeySize);

        return key;
    }

    private byte[] CreateSalt()  // ���� Salt�� ���� �ϴ� �޼ҵ�
    {
        byte[] salt = new byte[SaltSize];  // Salt ���� ���� �� byte�迭�� ���� �ʱ�ȭ

        // using �� ��� �Ͽ� RNGCryptoServiceProvider�� ��� �ϰ� �ڵ����� ������ ���� �ǰ� �Ѵ�.
        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())  // RNGCryptoServiceProvider�� ��ȣ�������� ������ ������ �������ִ� �޼ҵ��̴�.
        {
            rng.GetBytes(salt); // RNGCryptoServiceProvide�� ����Ͽ� ���� ������ ������ GetBytes�� ��� �Ͽ� �־��� �迭(Salt)�� ������ �������� ä�� �ش�.
        }

        using (SHA256 sha256 = SHA256.Create())  // SHA256�� ����Ͽ� 256����Ʈ �ؽ� ���� ����
        {
            byte[] hashedSalt = sha256.ComputeHash(salt);  // �ؽ����� salt byte�迭�� ������ ����
            return hashedSalt;
        }
    }

    private byte[] SyntheticKey(string password, byte[] salt, int iterations, int keySize)  // ��Ʈ ���� �н����带 �� �����ִ� �޼ҵ�
    {
        using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))  // �����ڿ� ��й�ȣ, ��Ʈ, �ݺ� Ƚ���� �����Ͽ� ��ü�� �ʱ�ȭ
        {
            // �޼��带 ȣ���Ͽ� �Ļ��� Ű�� keySize / 8�� ������ ���̴� ��Ʈ ������ ǥ���� Ű�� ���̸� ����Ʈ ������ ��ȯ�� �� ���ϱ�
            byte[] key = pbkdf2.GetBytes(keySize / 8);
            return key;
        }
    }

    private byte[] Encrypt(string plainText, byte[] key)  // ��ȣȭ �޼ҵ�
    {
        using (AesManaged aes = new AesManaged())
        {
            aes.Key = key;  // ����� Ű ��
            aes.GenerateIV();  // ������ �ʱ�ȭ ���� �� ����

            byte[] iv = aes.IV;  // ������ �ʱ� ���� ��
            byte[] encryptedData;  // ��ȣȭ�� byte�� ������ ���� ����

            using (ICryptoTransform encryptor = aes.CreateEncryptor())  // aes.CreateEncryptor() : aes�˰��� ��ȣȭ �۾��� �����ϱ� ���� 'ICryptoTransform'��ü�� ��ȯ 
            using (MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                // �ʱ�ȭ ���͸� ��ȣ���� �պκп� �߰�
                memoryStream.Write(iv, 0, iv.Length);

                // CryptoStream ��ü�� �����Ͽ� memoryStream�� ��ȣȭ ��Ʈ��(cryptoStream)�� �����մϴ�. �̶� ��ȣȭ�� ����� encryptor�� ����
                using (StreamWriter writer = new System.IO.StreamWriter(new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)))
                {
                    writer.Write(plainText);  // ��ü�� ����Ͽ� ��ȣȭ ��Ʈ��(cryptoStream)�� �� �ۼ�
                }
                encryptedData = memoryStream.ToArray();  // ��ȣȭ�� �����͸� ����Ʈ �迭�� ��ȯ
            }
            return encryptedData;
        }
    }

    private string Decrypt(byte[] encryptedData, byte[] key)  // ��ȣȭ �޼ҵ�
    {
        using (AesManaged aes = new AesManaged())
        {
            aes.Key = key;  // ����� Ű ��

            // ����(iv)������ ����Ʈ �迭 ����
            byte[] iv = new byte[aes.BlockSize / 8];  // AES ��ȣȭ ��ü�� ��� ũ��(BlockSize)�� �����ͼ� ����Ʈ ������ ������ �ʱ�ȭ ���� ũ�� ����
            Array.Copy(encryptedData, 0, iv, 0, iv.Length);  // ��ȣ��(encryptedData)���� �ʱ�ȭ ����(iv)�� �����մϴ�.

            string decryptedText;  // ��ȣȭ�� string���� ������ ���� ����

            using (ICryptoTransform decryptor = aes.CreateDecryptor(key, iv))  // CreateDecryptor()�޼��忡 Ű�� �ʱ�ȭ ���͸� �����Ͽ� ��ȣȭ ��ü�� ����
            // encryptedData �迭�� iv.Length ��ġ���� �����Ͽ� ������ �����͸� �о� ����
            using (MemoryStream memoryStream = new System.IO.MemoryStream(encryptedData, iv.Length, encryptedData.Length - iv.Length))
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))  /// CryptoSrream(MemoryStream, ICryptoTransform, CryptoStreamMode.(Write, Read));
            using (StreamReader reader = new System.IO.StreamReader(cryptoStream))  // StreamReader�� ����Ͽ� ��ȣȭ�� �����͸� �о� ����
            {
                decryptedText = reader.ReadToEnd();  // StreamReader�� ����Ͽ� ��ȣȭ�� �����͸� ������ �о�� decryptedText ������ �Ҵ�
            }
            return decryptedText;
        }
    }

    public void NewGameSlot()  // �� ���� ���� �� ���̺� ���� ���� Ȯ��
    {
        for (int i = 1; i < 4; i++)
        {
            // ������ ��ο� �̸��� ����
            string filePath = Application.persistentDataPath + "/PlayerData" + i + ".xml";

            if (!File.Exists(filePath))
            {
                saveNumber = i;  // �������� ��ȣ�� �����´�.
                break;
            }
            saveNumber = 0;  // ��� ������ ���� ������ ������ �Ұ�
        }
    }

    public void DeleteData(int num)
    {
        string filePath = Application.persistentDataPath + "/PlayerData" + num + ".xml";
        string mapfilePath = Application.persistentDataPath + "/MapData" + num + ".xml";
        File.Delete(filePath);
        File.Delete(mapfilePath);

    }

    

    public MapData MapDataGet(int num)  // ����� xml�� �����͸� �������� �޼ҵ�
    {
        // ������ ��ο� �̸��� ����
        string filePath = Application.persistentDataPath + "/MapData" + num + ".xml";

        // ��ȣȭ�� �����͸� ���Ͽ��� �ҷ���
        byte[] encryptedData = LoadEncryptDataFile(filePath);

        // ��ȣȭ�� �����͸� ��ȣȭ�Ͽ� XML �����ͷ� ��ȯ
        string decryptedData = Decrypt(encryptedData, settingData.dataKey[num - 1].playerDataKey);

        // XML �����͸� ������ȭ�Ͽ� ��ü�� ��ȯ
        MapData mapData = DeserializeDataMap(decryptedData);

        return mapData;
    }

    public void MapSaveData(MapData mapData, int num)  // ����� �����͸� �޾ƿ� �ٽ� xml���Ͽ� �����ϴ� �޼ҵ�
    {
        // ������ ��ο� �̸��� ����
        string filePath = Application.persistentDataPath + "/MapData" + num + ".xml";

        // XML �����͸� ���ڿ��� ����ȭ
        string serializedData = SerializeDataMap(mapData);

        byte[] key = settingData.dataKey[num - 1].playerDataKey;  // Ű�� ������ ����

        // ��ȣȭ�� ������ ����
        byte[] encryptedData = Encrypt(serializedData, key);

        SaveEncryptDataFile(encryptedData, filePath);
    }

    private static string SerializeDataMap(MapData data)  // �ؽ�Ʈ �����͸� ����ȭ ���ִ� �޼ҵ�
    {
        XmlSerializer serializer = new XmlSerializer(typeof(MapData));
        using (StringWriter writer = new StringWriter())
        {
            serializer.Serialize(writer, data);
            return writer.ToString();
        }
    }

    private static MapData DeserializeDataMap(string data)  // �ؽ�Ʈ �����͸� ������ȭ ���ִ� �޼ҵ�
    {
        XmlSerializer serializer = new XmlSerializer(typeof(MapData));
        using (StringReader reader = new StringReader(data))
        {
            MapData mapData = (MapData)serializer.Deserialize(reader);

            return mapData;
        }
    }
}