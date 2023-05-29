using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUIManager : MonoBehaviour
{
    [Header("Player UI")]
    [SerializeField] TMP_Text timer;  // ���� Ÿ�̸�
    [SerializeField] Image hpBar;  // hp��
    [SerializeField] Text hpCheck;  // hp ǥ��
    [SerializeField] Image mpBar;  // mp��
    [SerializeField] Text mpCheck;  // mp ǥ��
    [SerializeField] Text player_Level;  // Player ����
    [SerializeField] Slider expBar;  // ����ġ ��
    [SerializeField] Text expCheck;  // ����ġ �� ǥ��

    [Header("Menu")]
    [SerializeField] GameObject menuImage;  // �޴� �̹���
    [SerializeField] GameObject settingMenu;  // ����â
    [SerializeField] Slider slider_Bgm;  // bgm ��
    [SerializeField] Slider slider_Sfx;  // sfx ��
    [SerializeField] GameObject resolutionWindow;  // �ػ� ���� �� ǥ���� â
    [SerializeField] TMP_Text resolutionCount;  // �ػ� ���� â Ÿ�̸�

    [Header("Menu Icon")]
    [SerializeField] Image image_Bgm;  // ����� bgm �̹���
    [SerializeField] Image image_Sfx;  // ����� sfx �̹���
    [SerializeField] Sprite[] sprite_Bgm;  // bgm �̹��� ������ ��������Ʈ
    [SerializeField] Sprite[] sprite_Sfx;  // sfx �̹��� ������ ��������Ʈ

    [Header("Skill")]
    [SerializeField] GameObject[] image_Skill;  // ��ų ��Ÿ�� â
    [SerializeField] Image[] slider_Skill;  // ��ų ��Ÿ�� �ð� ���� �̹���
    [SerializeField] Text[] skillTimer;  // ��ų �� ���� �ð� ǥ��
    [SerializeField] Sprite[] sprite_Skill;  // ��ų ������

    [Header("Inventory")]
    [SerializeField] GameObject inventory;  // �κ��丮 â
    [SerializeField] Image[] itemFrame;  // ������ ������ ���� ������
    [SerializeField] Image[] itemSlot;  // ������ �̹��� �� ����
    [SerializeField] GameObject[] itemCount;  // ������ ���� ǥ�� â
    [SerializeField] Text[] itemQuantity;  // ������ ���� ǥ��
    [SerializeField] Sprite[] frameColor;  // ������ ������ ���� ������ �÷� ���� ��������Ʈ

    [Header("Player Stateers")]
    [SerializeField] Text playerStat;
}
