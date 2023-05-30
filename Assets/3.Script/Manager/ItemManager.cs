using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public int tag;
    public string type;
    public string name;
    public string tooltip;
    public int quantity;
    public int maxQuantity;
    public bool canInstallation;
    public bool canWear;
}

[System.Serializable]
public class Block : Item
{
    public Block(int _tag, string _type, string _name, string _tooltip, int _quantity, int _hp, bool _canInstallation, bool _canWear)
    {
        tag = _tag;
        type = _type;
        name = _name;
        tooltip = _tooltip;
        quantity = _quantity;
        hp = _hp;
        canInstallation = _canInstallation;
        canWear = _canWear;
    }

    public int hp;
}

[System.Serializable]
public class Armor : Item
{
    public Armor(int _tag, string _type, string _name, string _tooltip, int _quantity, int _hp, int _moveSpeed, bool _canInstallation, bool _canWear)
    {
        tag = _tag;
        type = _type;
        name = _name;
        tooltip = _tooltip;
        quantity = _quantity;
        hp = _hp;
        moveSpeed = _moveSpeed;
        canInstallation = _canInstallation;
        canWear = _canWear;
    }

    public int hp;
    public int moveSpeed;
}

[System.Serializable]
public class Helmet : Item
{
    public Helmet(int _tag, string _type, string _name, string _tooltip, int _quantity, int _defens, bool _canInstallation, bool _canWear)
    {
        tag = _tag;
        type = _type;
        name = _name;
        tooltip = _tooltip;
        quantity = _quantity;
        defens = _defens;
        canInstallation = _canInstallation;
        canWear = _canWear;
    }

    public int defens;
}

[System.Serializable]
public class Arms : Item
{
    public Arms(int _tag, string _type, string _name, string _tooltip, int _quantity, int _attack, int _attackSpeed, bool _canInstallation, bool _canWear)
    {
        tag = _tag;
        type = _type;
        name = _name;
        tooltip = _tooltip;
        quantity = _quantity;
        attack = _attack;
        attackSpeed = _attackSpeed;
        canInstallation = _canInstallation;
        canWear = _canWear;
    }

    public int attack;
    public int attackSpeed;
}

[System.Serializable]
public class Etc : Item
{
    public Etc(int _tag, string _type, string _name, string _tooltip, int _quantity, bool _canInstallation, bool _canWear)
    {
        tag = _tag;
        type = _type;
        name = _name;
        tooltip = _tooltip;
        quantity = _quantity;
        canInstallation = _canInstallation;
        canWear = _canWear;
    }
}

[System.Serializable]
public class About : Item
{
    public About(int _tag, string _type, string _name, string _tooltip, int _quantity, int _hp, int _mp, bool _canInstallation, bool _canWear)
    {
        tag = _tag;
        type = _type;
        name = _name;
        tooltip = _tooltip;
        quantity = _quantity;
        hp = _hp;
        mp = _mp;
        canInstallation = _canInstallation;
        canWear = _canWear;
    }

    public int hp;
    public int mp;
}

public class ItemManager : MonoBehaviour
{
    public TextAsset itemInformation;
    public List<Block> list_Block;
    public List<Armor> list_Armor;
    public List<Helmet> list_Helmet;
    public List<Arms> list_Arms;
    public List<Etc> list_Etc;
    public List<About> list_About;

    public List<Item> list_AllItem;

    void Start()
    {
        string[] line = itemInformation.text.Substring(0, itemInformation.text.Length - 1).Split('\n');
        for(int i = 0; i < line.Length; i++)
        {
            string[] row = line[i].Split('\t');

            if (row[1].Equals("Block"))
            {
                list_Block.Add(new Block(Int32.Parse(row[0]), row[1], row[2], row[3], Int32.Parse(row[4]), Int32.Parse(row[5]), row[6].Equals("TRUE"), row[7].Equals("TRUE")));
            }
            else if (row[1].Equals("Armor"))
            {
                list_Armor.Add(new Armor(Int32.Parse(row[0]), row[1], row[2], row[3], Int32.Parse(row[4]), Int32.Parse(row[5]), Int32.Parse(row[6]), row[7].Equals("TRUE"), row[8].Equals("TRUE")));
            }
            else if (row[1].Equals("Helmet"))
            {
                list_Helmet.Add(new Helmet(Int32.Parse(row[0]), row[1], row[2], row[3], Int32.Parse(row[4]), Int32.Parse(row[5]), row[6].Equals("TRUE"), row[7].Equals("TRUE")));
            }
            else if (row[1].Equals("Arms"))
            {
                list_Arms.Add(new Arms(Int32.Parse(row[0]), row[1], row[2], row[3], Int32.Parse(row[4]), Int32.Parse(row[5]), Int32.Parse(row[6]), row[7].Equals("TRUE"), row[8].Equals("TRUE")));
            }
            else if (row[1].Equals("Etc"))
            {
                list_Etc.Add(new Etc(Int32.Parse(row[0]), row[1], row[2], row[3], Int32.Parse(row[4]), row[5].Equals("TRUE"), row[6].Equals("TRUE")));
            }
            else if (row[1].Equals("About"))
            {
                list_About.Add(new About(Int32.Parse(row[0]), row[1], row[2], row[3], Int32.Parse(row[4]), Int32.Parse(row[5]), Int32.Parse(row[6]), row[7].Equals("TRUE"), row[8].Equals("TRUE")));
            }
        }

        for (int i = 0; i < line.Length; i++)
        {
            string[] row = line[i].Split('\t');

            if (row[1].Equals("Block"))
            {
                list_AllItem.Add(list_Block[i]);
            }
            else if (row[1].Equals("Armor"))
            {
                list_AllItem.Add(list_Armor[i]);
            }
            else if (row[1].Equals("Helmet"))
            {
                list_AllItem.Add(list_Helmet[i]);
            }
            else if (row[1].Equals("Arms"))
            {
                list_AllItem.Add(list_Arms[i]);
            }
            else if (row[1].Equals("Etc"))
            {
                list_AllItem.Add(list_Etc[i]);
            }
            else if (row[1].Equals("About"))
            {
                list_AllItem.Add(list_About[i]);
            }
        }

        print(list_AllItem[1].name);
    }
}
