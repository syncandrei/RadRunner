using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpManager : MonoBehaviour
{
    public static PickUpManager Instance;

    [Header("Objects")]
    public GameObject[] staticPointsObj;
    public GameObject[] speedBoostObj;
    public GameObject[] slowTimeObject;
    //public GameObject deathWall;

    [Header("Speed Settings Booster")]
    public float speedBoost = 0.0f;

    [Header("Slow Time Settings Booster")]
    public float slowLerpTimeTo = 0.0f;
    public float timeToTake = 0.0f;

    [Header("Others")]
    public float impulseForce = 0.0f;

    [Header("Durations")]
    public float slowTimeDuration = 0.0f;
    public float speedBoostDuration = 0.0f;

    [Header("Points")]
    public int ForSpeedBooster = 0;
    public int ForSlowTimeBooster = 0;
    public int ForAmps = 0;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public float SpeedBoost()
    {
        return speedBoost;
    }

    public float SlowLerpTimeTo()
    {
        return slowLerpTimeTo;
    }

    public float TimeToTake()
    {
        return timeToTake;
    }
}
