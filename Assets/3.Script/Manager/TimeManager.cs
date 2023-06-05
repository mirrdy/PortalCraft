using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance = null;
    public float ingameTime;
    public string timeString;
    public bool isNight;
    public delegate void PassedSingleMinute();
    public PassedSingleMinute passedSingleMinute;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;   
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        LoadIngameTime();
    }
    private void LoadIngameTime()
    {
        if (PlayerControl.instance?.playerData?.gameTime == null)
        {
            ingameTime = 0;
        }
        else
        {
            ingameTime = PlayerControl.instance.playerData.gameTime;
        }
    }

    void FixedUpdate()
    {
        ingameTime += Time.fixedDeltaTime;
        CheckItsNight();
    }
    public string GetInGameTimeString()
    {
        string timeStr;
        timeStr = "" + ingameTime.ToString("00.00");
        timeStr = timeStr.Replace(".", ":");

        return timeStr;
    }
    private void CheckItsNight()
    {
        int.TryParse(GetInGameTimeString().Substring(0, 2), out int hour);

        if (hour < 6 && hour >= 18)
        {
            isNight = true;
        }
        else
        {
            isNight = false;
        }
    }
}
