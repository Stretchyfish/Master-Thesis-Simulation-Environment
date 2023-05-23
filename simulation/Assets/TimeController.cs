using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public float CurrentTimeScale = 1;
    public float DesiredTimeScale = 1;

    void Update()
    {
        CurrentTimeScale = Time.timeScale;
        Time.timeScale = DesiredTimeScale;
    }
}
