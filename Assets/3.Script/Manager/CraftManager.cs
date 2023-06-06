using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Craft
{
    public Craft(string _tab, string _job, int _tag, string _name, int _quantity, int _materialTag1, int _material1, int _materialTag2, int _material2, int _materialTag3, int _material3, int _materialTag4, int _material4)
    {
        tab = _tab;
        job = _job;
        tag = _tag;
        name = _name;
        quantity = _quantity;
        materialTag1 = _materialTag1;
        material1 = _material1;
        materialTag2 = _materialTag2;
        material2 = _material2;
        materialTag3 = _materialTag3;
        material3 = _material3;
        materialTag4 = _materialTag4;
        material4 = _material4;
    }

    public string tab;
    public string job;
    public int tag;
    public string name;
    public int quantity;
    public int materialTag1;
    public int material1;
    public int materialTag2;
    public int material2;
    public int materialTag3;
    public int material3;
    public int materialTag4;
    public int material4;
}

public class CraftManager : MonoBehaviour
{
    public TextAsset CraftInfomation;
    public List<Craft> list_Craft;

    private void Start()
    {
        string[] line = CraftInfomation.text.Substring(0, CraftInfomation.text.Length - 1).Split('\n');
        for (int i = 0; i < line.Length; i++)
        {
            string[] row = line[i].Split('\t');
            list_Craft.Add(new Craft(row[0], row[1], Int32.Parse(row[2]), row[3], Int32.Parse(row[4]), Int32.Parse(row[5]), Int32.Parse(row[6]), Int32.Parse(row[7]), Int32.Parse(row[8]), Int32.Parse(row[9]), Int32.Parse(row[10]), Int32.Parse(row[11]), Int32.Parse(row[12])));
        }
    }
}
