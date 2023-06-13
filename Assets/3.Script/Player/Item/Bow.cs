using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour, ISkill_Q, ISkill_E
{
    public int attackDamage; //데미지
    public float attackRate; //공속

    public float arrowForce; //화살이 날아가는 힘
    public float throwUpwardForce; //화살이 위로발사되는 힘

    public GameObject player; 
    public GameObject arrowPrefab;
    public GameObject arrowPrefab_Q;
    public GameObject arrowPrefab_E;
    public Transform arrowSpawn; 
    private GameObject mainCam;

    #region 스킬프로퍼티
    public int SkillDamage_1 { get; set; }
    public int SkillMP_1 { get; set; }
    public  float SkillCool_1 { get; set; } 
    public int SkillDamage_2 { get; set; }
    public int SkillMP_2 { get; set; }
    public float SkillCool_2 { get; set; }

    public float SkillCoolDelta_1 = 15;
    public float SkillCoolDelta_2 = 20;
    #endregion

    private InGameUIManager uiManager;

    private void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        player = GameObject.FindGameObjectWithTag("Player");
        uiManager = GameObject.Find("InGameUIManager").GetComponent<InGameUIManager>();
    }

    private void Update()
    {
        SkillCoolDelta_1 += Time.deltaTime;
        SkillCoolDelta_2 += Time.deltaTime;
    }




    public void Use()
    {
        GameObject Arrow = Instantiate(arrowPrefab, arrowSpawn.position, mainCam.transform.rotation);
        Rigidbody Arrow_Rb = Arrow.GetComponent<Rigidbody>();

        Vector3 forceDirection = mainCam.transform.forward;

        Vector3 forceToAdd = forceDirection * arrowForce + player.transform.up * throwUpwardForce;

        Arrow_Rb.AddForce(forceToAdd, ForceMode.Impulse);
    }




    public void Skill_1()
    {
        Status status = PlayerControl.instance.playerData.status;
        int skillLevel = PlayerControl.instance.playerData.skill[0].skillLevel;
        switch (skillLevel)
        {
            case 1:
            {
                SkillDamage_1 = 30; SkillMP_1 = 10; SkillCool_1 = 15;
                break;
            }
            case 2:
            {
                SkillDamage_1 = 30; SkillMP_1 = 15; SkillCool_1 = 10;
                break;
            }
            case 3:
            {
                SkillDamage_1 = 30; SkillMP_1 = 20; SkillCool_1 = 5;
                break;
            }
        }

        

        if (SkillCoolDelta_1 >= SkillCool_1 && status.currentMp >= SkillMP_1)
        {
            PlayerControl.instance.animator.SetTrigger("A1");
            SkillCoolDelta_1 = 0;
            StartCoroutine(ThreeShot());
            
        }
       
    }   
    IEnumerator ThreeShot()
    {
        for (int i = 0; i < 3; i++) 
        {
            ShootArrow_Q();
            yield return new WaitForSeconds(0.25f);
        }
    }
    private void ShootArrow_Q()
    {
        GameObject Arrow = Instantiate(arrowPrefab_Q, arrowSpawn.position, mainCam.transform.rotation);
        Rigidbody Arrow_Rb = Arrow.GetComponent<Rigidbody>();

        Vector3 forceDirection = mainCam.transform.forward;

        Vector3 forceToAdd = forceDirection * arrowForce + player.transform.up * throwUpwardForce;

        Arrow_Rb.AddForce(forceToAdd, ForceMode.Impulse);

        player.transform.rotation = Quaternion.Euler(0f, mainCam.transform.eulerAngles.y, 0f);
    }




    public void Skill_2()
    {
        Status status = PlayerControl.instance.playerData.status;
        int skillLevel = PlayerControl.instance.playerData.skill[1].skillLevel;
        switch (skillLevel)
        {
            case 1:
                {
                    SkillDamage_2 = 200; SkillMP_2 = 20; SkillCool_2 = 15;
                    break;
                }
            case 2:
                {
                    SkillDamage_2 = 250; SkillMP_2 = 25; SkillCool_2 = 10;
                    break;
                }
            case 3:
                {
                    SkillDamage_2 = 300; SkillMP_2 = 30; SkillCool_2 = 10;
                    break;
                }
        }

        

        if (SkillCoolDelta_2 >= SkillCool_2 && status.currentMp >= SkillMP_2)
        {
            PlayerControl.instance.animator.SetTrigger("A2");
            SkillCoolDelta_2 = 0;
            StartCoroutine(Snipe());

        }

    }
    IEnumerator Snipe()
    {
        player.transform.rotation = Quaternion.Euler(0f, mainCam.transform.eulerAngles.y, 0f);
        yield return new WaitForSeconds(0.9f);
        GameObject Arrow = Instantiate(arrowPrefab_E, arrowSpawn.position, mainCam.transform.rotation);
        Rigidbody Arrow_Rb = Arrow.GetComponent<Rigidbody>();
        Vector3 forceDirection = mainCam.transform.forward;
        Vector3 forceToAdd = forceDirection * arrowForce + player.transform.up * throwUpwardForce;
        Arrow_Rb.AddForce(forceToAdd, ForceMode.Impulse);      
    }
}
