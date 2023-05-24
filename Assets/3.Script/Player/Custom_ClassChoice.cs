using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Custom_ClassChoice : MonoBehaviour
{
    //스크립트에서 버튼의 색상을 변경하기 위해서는 ColorBlock 개체를 생성하여 버튼의 Normal Color를 직접 지정해줘야 한다.
    //필요에 따라 Highlighted Color, Pressed Color, Disabled Color도 직접 지정할 수 있다.
    //Warrior_btn 누르면 

    public GameObject custom_charUI;
    public GameObject character_Warrior;
    public GameObject character_Archer;

    public void Warrior_Button()
    {
        character_Warrior.SetActive(true);
        character_Archer.SetActive(false);
        Debug.Log("전사클릭");
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(true);
        transform.GetChild(3).gameObject.SetActive(false);             
    }

    public void Archer_Button()
    {
        character_Warrior.SetActive(false);
        character_Archer.SetActive(true);
        Debug.Log("레인저클릭");
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
