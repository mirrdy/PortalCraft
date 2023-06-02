using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeClass : MonoBehaviour
{
    public GameObject custom_charUI;

    public GameObject player;

    public GameObject character_Warrior;
    public GameObject character_Archer;

    public string job = null;

    public void Warrior_Button()
    {
        character_Warrior.SetActive(true);
        character_Archer.SetActive(false);
        Debug.Log("전사선택");

        job = "전사";

        transform.GetChild(3).gameObject.SetActive(true);
        transform.GetChild(4).gameObject.SetActive(false);
    }

    public void Archer_Button()
    {
        character_Warrior.SetActive(false);
        character_Archer.SetActive(true);
        Debug.Log("레인저선택");

        job = "레인저";

        transform.GetChild(3).gameObject.SetActive(false);
        transform.GetChild(4).gameObject.SetActive(true);
    }

    public void Confirm_Button()
    {
        PlayerData playerData = DataManager.instance.playerData;

        playerData.job = job;

        player.SetActive(true);
        character_Warrior.SetActive(false);
        character_Archer.SetActive(false);
        gameObject.SetActive(false);
        custom_charUI.SetActive(true);
    }
}
