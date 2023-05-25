using UnityEngine;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Xml.Serialization;
using System.Security.Cryptography;
using System;

public class SettingData  // Yaml 데이터형 class생성
{
    public float bgmSound;  // BGM 사운드 저장할 변수
    public float sfxSound;  // SFX 사운드 저장할 변수
    public int resolutionSize;  // 해상도 번호 저장할 변수
    public byte[] playerDataKey;  // 플레이어 데이터 정보를 열고 닫는데 필요한 데이터 변수
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;  // DataManager 싱글톤 생성
    public PlayerData playerData = new PlayerData();  // Player의 데이터 클래스 가져오기

    // Yaml 데이터형 변수 지정
    SettingData settingData = new SettingData();

    ISerializer serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();  // Yaml의 데이터 직렬화를 하는 변수 생성
    IDeserializer deserializer = new DeserializerBuilder().Build();  // Yaml의 데이터 역직렬화 하는 변수 생성

    private const int KeySize = 256;  // key의 비트수를 지정
    private const int SaltSize = 16;  // salt의 비트 값을 지정
    private const int Iterations = 10000;  // 해싱을 몇번 반복할지 횟수를 지정

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

        if (!File.Exists(Path.Combine(Application.persistentDataPath, "settingData.yaml")))  // 세팅파일 유무 확인
        {
            // 데이터 초기화
            settingData.bgmSound = 1f;
            settingData.sfxSound = 1f;
            settingData.resolutionSize = 0;

            YamlSet();
        }
        else
        {
            string fileData = File.ReadAllText(Path.Combine(Application.persistentDataPath, "settingData.yaml"));  // 세팅파일을 생성할 위치 지정후 파일 생성
            settingData = deserializer.Deserialize<SettingData>(fileData);  // 데이터 역직렬화 하여 데이터를 가지고 옴
        }
    }

    public void SaveSound(float bgm, float sfx)  // 사운드 Local 저장 메소드
    {
        settingData.bgmSound = bgm;
        settingData.sfxSound = sfx;

        YamlSet();
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

        YamlSet();

        return settingData.resolutionSize;
    }

    public int LoadResolution()  // 해상조 정보를 가져오는 메소드
    {
        string filedata = File.ReadAllText(Path.Combine(Application.persistentDataPath, "settingData.yaml"));  // 파일을 생성할 위치 지정후 파일 생성
        settingData = deserializer.Deserialize<SettingData>(filedata);  // 데이터 역직렬화 하여 데이터를 가지고 옴

        return settingData.resolutionSize;
    }

    public void PlayerDataSet(int num)  // Player의 정보(xml 파일의 유무)를 확인 하고 없으면 생성하고 있으면 Load 하는 메소드
    {
        // 파일의 경로와 이름을 지정
        string filePath = Application.persistentDataPath + "/PlayerData" + num + ".xml";

        if (!File.Exists(filePath))  // xml의 데이터에 따라서 저장 데이터를 전달 초기화 이후에 대입하여 기본값을 설정
        {
            playerData.staters = new Staters();

            playerData.hair = 0;
            playerData.eye = 0;
            playerData.mouth = 0;
            playerData.mouth = 0;
            playerData.body = 0;

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

            // XML 데이터를 문자열로 직렬화
            string serializedData = SerializeData(playerData);

            // 암호화 키 생성
            byte[] key = CreateKey(serializedData);

            settingData.playerDataKey = key;  // 키값 데이터 저장

            YamlSet();  // yaml 데이터 저장

            // 암호화된 데이터 생성
            byte[] encryptData = Encrypt(serializedData, key);

            SaveEncryptDataFile(encryptData, filePath);
        }
        else
        {
            playerData = PlayerDataGet(num);  // 파일이 있을 경우 데이터를 읽어오는 메소드 실행
        }
    }

    public PlayerData PlayerDataGet(int num)  // 저장된 xml의 데이터를 가져오는 메소드
    {
        // 파일의 경로와 이름을 지정
        string filePath = Application.persistentDataPath + "/PlayerData" + num + ".xml";

        // 암호화된 데이터를 파일에서 불러옴
        byte[] encryptedData = LoadEncryptDataFile(filePath);

        // 암호화된 데이터를 복호화하여 XML 데이터로 변환
        string decryptedData = Decrypt(encryptedData, settingData.playerDataKey);

        // XML 데이터를 역직렬화하여 객체로 변환
        PlayerData playerData = DeserializeData(decryptedData);

        return playerData;
    }

    public void SaveData(PlayerData playerData, int num)  // 사용한 데이터를 받아와 다시 xml파일에 저장하는 메소드
    {
        // 파일의 경로와 이름을 지정
        string filePath = Application.persistentDataPath + "/PlayerData" + num + ".xml";

        // XML 데이터를 문자열로 직렬화
        string serializedData = SerializeData(playerData);

        // 암호화 키 생성
        byte[] key = CreateKey(serializedData);

        settingData.playerDataKey = key;  // 키값 데이터 저장

        YamlSet();  // yaml 데이터 저장

        // 암호화된 데이터 생성
        byte[] encryptedData = Encrypt(serializedData, key);

        SaveEncryptDataFile(encryptedData, filePath);
    }

    private static string SerializeData(PlayerData data)  // 텍스트 데이터를 직렬화 해주는 메소드
    {
        XmlSerializer serializer = new XmlSerializer(typeof(PlayerData));
        using (StringWriter writer = new StringWriter())
        {
            serializer.Serialize(writer, data);
            return writer.ToString();
        }
    }

    private static PlayerData DeserializeData(string data)  // 텍스트 데이터를 역직렬화 해주는 메소드
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
        string yamlData = serializer.Serialize(settingData);  // 데이터 직렬화
        string settingfilePath = Path.Combine(Application.persistentDataPath, "settingData.yaml");  // 파일을 생성할 위치 지정후 파일 생성

        File.WriteAllText(settingfilePath, yamlData);  // 파일에 직렬화 한 데이터를 저장
    }

    private byte[] CreateKey(string data)  // 키를 생성 해주는 메소드
    {
        // 솔트 생성
        byte[] salt = CreateSalt();

        // PBKDF2를 사용하여 키 파생
        byte[] key = SyntheticKey(data, salt, Iterations, KeySize);

        return key;
    }

    private byte[] CreateSalt()  // 랜덤 Salt값 생성 하는 메소드
    {
        byte[] salt = new byte[SaltSize];  // Salt 값을 저장 할 byte배열을 생성 초기화

        // using 을 사용 하여 RNGCryptoServiceProvider를 사용 하고 자동으로 닫히고 정리 되게 한다.
        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())  // RNGCryptoServiceProvider은 암호학적으로 강력한 난수를 생성해주는 메소드이다.
        {
            rng.GetBytes(salt); // RNGCryptoServiceProvide를 사용하여 만든 안전한 난수를 GetBytes를 사용 하여 주어진 배열(Salt)에 난수를 무작위로 채워 준다.
        }

        using (SHA256 sha256 = SHA256.Create())  // SHA256를 사용하여 256바이트 해싱 변수 생성
        {
            byte[] hashedSalt = sha256.ComputeHash(salt);  // 해싱해준 salt byte배열을 변수에 저장
            return hashedSalt;
        }
    }

    private byte[] SyntheticKey(string password, byte[] salt, int iterations, int keySize)  // 솔트 값과 패스워드를 를 합쳐주는 메소드
    {
        using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))  // 생성자에 비밀번호, 솔트, 반복 횟수를 전달하여 객체를 초기화
        {
            // 메서드를 호출하여 파생된 키를 keySize / 8로 지정된 길이는 비트 단위로 표현된 키의 길이를 바이트 단위로 변환한 값 구하기
            byte[] key = pbkdf2.GetBytes(keySize / 8);
            return key;
        }
    }

    private byte[] Encrypt(string plainText, byte[] key)  // 암호화 메소드
    {
        using (AesManaged aes = new AesManaged())
        {
            aes.Key = key;  // 사용할 키 값
            aes.GenerateIV();  // 랜덤한 초기화 벡터 값 생성

            byte[] iv = aes.IV;  // 생성된 초기 벡터 값
            byte[] encryptedData;  // 암호화한 byte를 저장할 변수 선언

            using (ICryptoTransform encryptor = aes.CreateEncryptor())  // aes.CreateEncryptor() : aes알고리즘 암호화 작업을 수행하기 위한 'ICryptoTransform'객체를 반환 
            using (MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                // 초기화 벡터를 암호문의 앞부분에 추가
                memoryStream.Write(iv, 0, iv.Length);

                // CryptoStream 객체를 생성하여 memoryStream을 암호화 스트림(cryptoStream)에 연결합니다. 이때 암호화에 사용할 encryptor를 전달
                using (StreamWriter writer = new System.IO.StreamWriter(new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)))
                {
                    writer.Write(plainText);  // 객체를 사용하여 암호화 스트림(cryptoStream)에 평문 작성
                }
                encryptedData = memoryStream.ToArray();  // 암호화된 데이터를 바이트 배열로 변환
            }
            return encryptedData;
        }
    }

    private string Decrypt(byte[] encryptedData, byte[] key)  // 복호화 메소드
    {
        using (AesManaged aes = new AesManaged())
        {
            
            aes.Key = key;  // 사용할 키 값 

            // 벡터(iv)저장할 바이트 배열 선언
            byte[] iv = new byte[aes.BlockSize / 8];  // AES 암호화 객체의 블록 크기(BlockSize)를 가져와서 바이트 단위로 나누어 초기화 벡터 크기 결정
            Array.Copy(encryptedData, 0, iv, 0, iv.Length);  // 암호문(encryptedData)에서 초기화 벡터(iv)를 추출합니다.

            string decryptedText;  // 복호화한 string값을 저장할 변수 선언

            using (ICryptoTransform decryptor = aes.CreateDecryptor(key, iv))  // CreateDecryptor()메서드에 키와 초기화 벡터를 전달하여 복호화 객체를 생성
            // encryptedData 배열의 iv.Length 위치부터 시작하여 나머지 데이터를 읽어 오기
            using (MemoryStream memoryStream = new System.IO.MemoryStream(encryptedData, iv.Length, encryptedData.Length - iv.Length))
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))  /// CryptoSrream(MemoryStream, ICryptoTransform, CryptoStreamMode.(Write, Read));
            using (StreamReader reader = new System.IO.StreamReader(cryptoStream))  // StreamReader를 사용하여 복호화된 데이터를 읽어 오기
            {
                decryptedText = reader.ReadToEnd();  // StreamReader를 사용하여 복호화된 데이터를 끝까지 읽어와 decryptedText 변수에 할당
            }
            return decryptedText;
        }
    }
}
