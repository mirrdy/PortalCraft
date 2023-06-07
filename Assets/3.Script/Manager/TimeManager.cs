using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance = null;
    public float ingameTime;
    public string timeString;
    public bool isNight;
    public int timeSpeedMultiply = 24;
    public delegate void PassedSingleMinute();
    public PassedSingleMinute passedSingleMinute;

    private Light mapLight;

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
        mapLight = FindObjectOfType<Light>();
        LoadIngameTime();
    }
    private void LoadIngameTime()
    {
        if (PlayerControl.instance?.playerData?.gameTime == 0)
        {
            ingameTime = 3600 * 6;
        }
        else
        {
            ingameTime = PlayerControl.instance.playerData.gameTime;
        }
    }

    void FixedUpdate()
    {
        ingameTime += Time.fixedDeltaTime * timeSpeedMultiply;
        CheckItsNight();
    }
    public string GetInGameTimeString()
    {
        string timeStr;

        timeStr = System.TimeSpan.FromSeconds(ingameTime).ToString(@"hh\:mm");

        return timeStr;
    }
    private void CheckItsNight()
    {
        int.TryParse(GetInGameTimeString().Substring(0, 2), out int hour);

        if(isNight)
        {
            if(hour >= 6 && hour < 18)
            {
                isNight = false;
                mapLight.transform.Rotate(new Vector3(180, 0));
            }
        }
        else
        {
            if(hour < 6 || hour >= 18)
            {
                isNight = true;
                mapLight.transform.Rotate(new Vector3(180, 0));
            }
        }
    }
}
