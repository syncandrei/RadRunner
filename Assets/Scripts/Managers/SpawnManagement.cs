using UnityEngine;

public class SpawnManagement : MonoBehaviour
{
    public static SpawnManagement Instance;

    [Header("SpawnAmps")]
    public Transform[] spawnAmps;
    [Header("SpawnObjects")]
    public Transform[] spawnPointSpeedBooster;
    public Transform[] spawnPointSlowTime;
    public Transform spawnPointDeathWall;

    private int numberOfSlowTimeSpawns;
    private int numberOfSpeedTimeSpawns;
    private int numberOfAmpsSpawns;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        SpawnAmps();
        SpawnSpeedBoost();
        SpawnSlowTime();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SpawnAmps()
    {
        for (int i = 0; i < spawnAmps.Length; i++)
        {
            var plm = Random.Range(0, PickUpManager.Instance.staticPointsObj.Length);
            GameObject go = Instantiate(PickUpManager.Instance.staticPointsObj[plm], spawnAmps[i].position, Quaternion.identity);
            go.transform.SetParent(this.transform);
        }

        numberOfAmpsSpawns = spawnAmps.Length;
    }

    public void SpawnSpeedBoost()
    {
        for (int i = 0; i < spawnPointSpeedBooster.Length; i++)
        {
            var plm = Random.Range(0, PickUpManager.Instance.speedBoostObj.Length);
            GameObject go = Instantiate(PickUpManager.Instance.speedBoostObj[plm], spawnPointSpeedBooster[i].position, Quaternion.identity);
            go.transform.SetParent(this.transform);
        }

        numberOfSpeedTimeSpawns = spawnPointSpeedBooster.Length;
    }

    public void SpawnSlowTime()
    {
        for (int i = 0; i < spawnPointSlowTime.Length; i++)
        {
            var plm = Random.Range(0, PickUpManager.Instance.slowTimeObject.Length);
            GameObject go = Instantiate(PickUpManager.Instance.slowTimeObject[plm], spawnPointSlowTime[i].position, Quaternion.identity);
            go.transform.SetParent(this.transform);
        }

        numberOfSlowTimeSpawns = spawnPointSlowTime.Length;
    }

    //public void SpawnDeathWall()
    //{
    //    if (Random.value > 0.8) //%20 percent chance
    //    {
    //        GameObject go = Instantiate(PickUpManager.Instance.deathWall, spawnPointDeathWall.position, Quaternion.identity);
    //        go.transform.SetParent(this.transform);
    //    }
    //}

    public int NumberOfSlowTimeSpawns()
    {
        return numberOfSlowTimeSpawns;
    }

    public int NumberOfSpeedTimeSpawns()
    {
        return numberOfSpeedTimeSpawns;
    }

    public int NumberOfAmpsSpawns()
    {
        return numberOfAmpsSpawns;
    }
}
