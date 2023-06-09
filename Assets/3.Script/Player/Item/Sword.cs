using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, ISkill_Q, ISkill_E
{
    public int attackDamage;
    public float attackRate;

    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;
    public TrailRenderer trailEffect_Q;

    #region 스킬프로퍼티
    public int SkillDamage_1 { get; set; }
    public int SkillMP_1 { get; set; }
    public float SkillCool_1 { get; set; }
    public int SkillDamage_2 { get; set; }
    public int SkillMP_2 { get; set; }
    public float SkillCool_2 { get; set; }

    public float SkillCoolDelta_1 = 100;
    public float SkillCoolDelta_2 = 100;
    private bool isSkill_1;
    private bool isSkill_2;
    #endregion

    private InGameUIManager uiManager;

    private void Awake()
    {
        uiManager = GameObject.Find("InGameUIManager").GetComponent<InGameUIManager>();
    }


    private void Update()
    {
        SkillCoolDelta_1 += Time.deltaTime;
        SkillCoolDelta_2 += Time.deltaTime;
    }



    public void Use()
    {
        StartCoroutine("Swing");
    }
    IEnumerator Swing()
    {
        meleeArea.enabled = true;
        yield return new WaitForSeconds(0.1f);
        trailEffect.enabled = true;
        yield return new WaitForSeconds(0.36f);
        meleeArea.enabled = false;
        trailEffect.enabled = false;
    }
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Monster"))
        {
            Status status = PlayerControl.instance.playerData.status;

            coll.TryGetComponent(out MonsterControl monsterControl);
            int damage;
            if (isSkill_1)
            {
                damage = status.attack + attackDamage + SkillDamage_1;
            }
            else if(isSkill_2)
            {
                damage = status.attack + attackDamage + SkillDamage_2;
            }
            else
            {
                damage = status.attack + attackDamage;
            }
            monsterControl.TakeDamage(damage);
        }
        if (coll.CompareTag("Boss"))
        {
            Status status = PlayerControl.instance.playerData.status;

            coll.TryGetComponent(out BossControl monsterControl);
            int damage;
            if (isSkill_1)
            {
                damage = status.attack + attackDamage + SkillDamage_1;
            }
            else if (isSkill_2)
            {
                damage = status.attack + attackDamage + SkillDamage_2;
            }
            else
            {
                damage = status.attack + attackDamage;
            }
            monsterControl.TakeDamage(damage);
        }
    }





    public void Skill_1()
    {
        Status status = PlayerControl.instance.playerData.status;
        int skillLevel = PlayerControl.instance.playerData.skill[0].skillLevel;
        switch (skillLevel)
        {
            case 1:
                {
                    SkillDamage_1 = 80; SkillMP_1 = 10; SkillCool_1 = 5;
                    break;
                }
            case 2:
                {
                    SkillDamage_1 = 100; SkillMP_1 = 15; SkillCool_1 = 10;
                    break;
                }
            case 3:
                {
                    SkillDamage_1 = 120; SkillMP_1 = 20; SkillCool_1 = 10;
                    break;
                }
        }

        if (SkillCoolDelta_1 >= SkillCool_1 && status.currentMp >= SkillMP_1)
        {
            PlayerControl.instance.animator.SetTrigger("W1");
            SkillCoolDelta_1 = 0;
            StartCoroutine(Strike());

        }

    }
    IEnumerator Strike()
    {
        isSkill_1 = true;
        yield return new WaitForSeconds(0.317f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        yield return new WaitForSeconds(0.67f);
        meleeArea.enabled = false;
        trailEffect.enabled = false;
        isSkill_1 = false;
    }






    public void Skill_2()
    {
        Status status = PlayerControl.instance.playerData.status;
        int skillLevel = PlayerControl.instance.playerData.skill[1].skillLevel;
        switch (skillLevel)
        {
            case 1:
                {
                    SkillDamage_2 = 20; SkillMP_2 = 10; SkillCool_2 = 10;
                    break;
                }
            case 2:
                {
                    SkillDamage_2 = 30; SkillMP_2 = 15; SkillCool_2 = 10;
                    break;
                }
            case 3:
                {
                    SkillDamage_2 = 40; SkillMP_2 = 20; SkillCool_2 = 10;
                    break;
                }
        }

       
        if (SkillCoolDelta_2 >= SkillCool_2 && status.currentMp >= SkillMP_2)
        {
            PlayerControl.instance.animator.SetTrigger("W2");
            SkillCoolDelta_2 = 0;
            StartCoroutine(WhirlWind());

        }
   
    }
    IEnumerator WhirlWind()
    {
        isSkill_2 = true;
        meleeArea.enabled = true;
        trailEffect_Q.enabled = true;
        yield return new WaitForSeconds(3f);
        meleeArea.enabled = false;
        PlayerControl.instance.animator.SetTrigger("W2Done");
        yield return new WaitForSeconds(0.15f);
        trailEffect_Q.enabled = false;
        isSkill_2 = false;   
    }
}
