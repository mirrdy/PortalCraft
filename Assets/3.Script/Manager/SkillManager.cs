using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class SkillInfo
{
    public SkillInfo(int _tag, string _name, int _level, int _levelLimit, string _job, float _damage, int _mp, int _coolTime, string _tooltip, bool _hasSkill, int _skillUpPoint)
    {
        tag = _tag;
        name = _name;
        level = _level;
        levelLimit = _levelLimit;
        job = _job;
        damage = _damage;
        mp = _mp;
        coolTime = _coolTime;
        tooltip = _tooltip;
        hasSkill = _hasSkill;
        skillUpPoint = _skillUpPoint;
    }

    public int tag;
    public string name;
    public int level;
    public int levelLimit;
    public string job;
    public float damage;
    public int mp;
    public int coolTime;
    public string tooltip;
    public bool hasSkill;
    public int skillUpPoint;
}

public class SkillManager : MonoBehaviour
{
    public TextAsset skillInformation;
    public List<SkillInfo> list_Skill;

    void Start()
    {
        string[] line = skillInformation.text.Substring(0, skillInformation.text.Length - 1).Split('\n');
        for(int i = 0; i < line.Length; i++)
        {
            string[] row = line[i].Split('\t');
            list_Skill.Add(new SkillInfo(Int32.Parse(row[0]), row[1], Int32.Parse(row[2]), Int32.Parse(row[3]), row[4], float.Parse(row[5]), Int32.Parse(row[6]), Int32.Parse(row[7]), row[8], row[9].Equals("TRUE"), Int32.Parse(row[10])));
        }
    }


}
