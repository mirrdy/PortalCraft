using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Custom_ClassChoice : MonoBehaviour
{
    //��ũ��Ʈ���� ��ư�� ������ �����ϱ� ���ؼ��� ColorBlock ��ü�� �����Ͽ� ��ư�� Normal Color�� ���� ��������� �Ѵ�.
    //�ʿ信 ���� Highlighted Color, Pressed Color, Disabled Color�� ���� ������ �� �ִ�.
    //Warrior_btn ������ 

    public GameObject custom_charUI;
    public GameObject character_Warrior;
    public GameObject character_Archer;

    public void Warrior_Button()
    {
        character_Warrior.SetActive(true);
        character_Archer.SetActive(false);
        Debug.Log("����Ŭ��");
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(true);
        transform.GetChild(3).gameObject.SetActive(false);             
    }

    public void Archer_Button()
    {
        character_Warrior.SetActive(false);
        character_Archer.SetActive(true);
        Debug.Log("������Ŭ��");
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(true);
    }

    public void Confirm_Button()
    {
        gameObject.SetActive(false);
        custom_charUI.SetActive(true);
    }
}
