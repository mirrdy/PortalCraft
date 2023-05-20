using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;

public class PlayerController : MonoBehaviour
{
    private Movement3D movement3D;
    private CharacterController charController;

    private void Awake()
    {
        TryGetComponent(out movement3D);
        TryGetComponent(out charController);
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        movement3D.MoveTo(new Vector3(x, 0, z));

        if (Input.GetKeyDown(KeyCode.Space) && charController.isGrounded)
        {
            movement3D.Jump();
        }
    }
}

[Serializable]
public class PlayerData  // 플레이어 데이터 관리 클레스
{
    [XmlElement]
    public string playerName;
    [XmlElement]
    public int playerLevel;
    [XmlElement]
    public float playerExp;
    [XmlElement]
    public Staters staters;
    [XmlElement]
    public Skill[] skill = new Skill[6];
    [XmlElement]
    public Inventory[] inventory = new Inventory[40];
}

[Serializable]
public class Staters  // 플레이어 스텟 관리 클래스
{
    [XmlElement]
    public int hp;
    [XmlElement]
    public int mp;
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
    public int slot;
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
}