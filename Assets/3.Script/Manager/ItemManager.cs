using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Block
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

    public int tag;
    public string type;
    public string name;
    public string tooltip;
    public int quantity;
    public int hp;
    public bool canInstallation;
    public bool canWear;
}

public class ItemManager : MonoBehaviour
{
    public TextAsset itemInformation;
    public List<Block> list_Block;

    void Start()
    {
        string[] line = itemInformation.text.Substring(0, itemInformation.text.Length - 1).Split('\n');
        for(int i = 0; i < line.Length; i++)
        {
            string[] row = line[i].Split('\t');

            list_Block.Add(new Block(Int32.Parse(row[0]), row[1], row[2], row[3], Int32.Parse(row[4]), Int32.Parse(row[5]), row[6].Equals("TRUE"), row[7].Equals("TRUE")));
        }
    }
}
