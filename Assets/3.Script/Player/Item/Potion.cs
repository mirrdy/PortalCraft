using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public enum Type { Hp, MP };

    public Type potionType; //����Ÿ��
    public int healAmount;  //ȸ����

    public void Use()
    {
        if (potionType == Type.Hp)
        {
            StartCoroutine("DrinkPotion");
        }
        else if (potionType == Type.MP)
        {
            StartCoroutine("DrinkPotion");
        }
    }

    IEnumerator DrinkPotion()
    {
        yield return new WaitForSeconds(1f);
        //PlayerControl.instance.playerData.status.currentHp += healAmount;
        Debug.Log("ü��" + healAmount + "ȸ��");
    }
}
