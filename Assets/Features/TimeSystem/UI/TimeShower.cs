using System;
using TMPro;
using UnityEngine;

public class TimeShower : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;

    [SerializeField] private float startMinutes = 480;
    [SerializeField] private float endMinutes = 1440;

    void Update()
    {
        float normTime = TimeSystem.instance.NormTime;

        int valMinutes = (int)Mathf.Lerp(startMinutes, endMinutes, normTime);

        float hour = valMinutes / 60;
        float min = valMinutes % 60;

        timeText.text = hour + " : " + min + " " + TimeSystem.instance.GetCurrentStateName();
    }
}
