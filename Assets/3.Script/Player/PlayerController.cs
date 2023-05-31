using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;

public class PlayerController : MonoBehaviour
{
    //private Movement3D movement3D;
    private CharacterController charController;

    public PlayerData playerData;
    public ItemManager itemInfo;
    public SkillManager skillInfo;

    private void Awake()
    {
        //TryGetComponent(out movement3D);
        TryGetComponent(out charController);
        TryGetComponent(out itemInfo);
        TryGetComponent(out skillInfo);
        playerData = DataManager.instance.PlayerDataGet(DataManager.instance.saveNumber);
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        //movement3D.MoveTo(new Vector3(x, 0, z));

        if (Input.GetKeyDown(KeyCode.Space) && charController.isGrounded)
        {
            //movement3D.Jump();
        }
    }
}

[Serializable]
public class PlayerData  // 플레이어 데이터 관리 클레스
{
    [XmlElement]
    public string job;
    [XmlElement]
    public int hair;
    [XmlElement]
    public int eye;
    [XmlElement]
    public int mouth;
    [XmlElement]
    public int mustache;
    [XmlElement]
    public int body;
    [XmlElement]
    public string playerName;
    [XmlElement]
    public int playerLevel;
    [XmlElement]
    public float playerExp;
    [XmlElement]
    public Staters staters;
    [XmlElement]
    public Skill[] skill = new Skill[3];
    [XmlElement]
    public Inventory[] inventory = new Inventory[40];
}

[Serializable]
public class Staters  // 플레이어 스텟 관리 클래스
{
    [XmlElement]
    public int maxHp;
    [XmlElement]
    public int maxMp;
    [XmlElement]
    public float moveSpeed;
    [XmlElement]
    public float attackSpeed;
    [XmlElement]
    public int attack;
    [XmlElement]
    public int defens;
    [XmlElement]
    public int statersPoint;
    [XmlElement]
    public int skillPoint;
}

[Serializable]
public class Inventory  // 인벤토리 정보 관리 클레스
{
    [XmlElement]
    public int tag;
    [XmlElement]
    public int quantity;
    [XmlElement]
    public bool hasItem;
}

[Serializable]
public class Skill  // 스킬 정보 관리 클레스
{
    [XmlElement]
    public int skillNum;
    [XmlElement]
    public int skillLevel;
    [XmlElement]
    public bool hasSkill;
}