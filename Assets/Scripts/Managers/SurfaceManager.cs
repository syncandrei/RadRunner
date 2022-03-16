using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceManager : MonoBehaviour
{
    private Transform playerTransfrom;
    private List<GameObject> activeSurface;
    private List<GameObject> surfacesList;
    public int initialSurfaceAmount;
    private int index = 0;
    public GameObject[] structurePrefab;

    private float spawnZ = 0;// 92.0f;
    public float spawnSafeZone = 150; //meters that it get destroy after
    public float surfaceLength = 400.0f;
    public int amountSurfaceOnScreen = 7; //show the amount surface on screen

    // Use this for initialization
    void Start()
    {
        surfacesList = new List<GameObject>();
        activeSurface = new List<GameObject>();
        playerTransfrom = GameObject.FindGameObjectWithTag("Player").transform;

        //for (int i = 0; i < amountSurfaceOnScreen; i++)
        //{
        //    int randomIndex = Random.Range(0, structurePrefab.Length);
        //    GameObject go = Instantiate(structurePrefab[randomIndex]) as GameObject;
        //    go.SetActive(false);
        //    surfacesList.Add(go);
        //}

        for (int i = 0; i < amountSurfaceOnScreen; i++)
        {
            //var x = surfacesList[Random.Range(0, surfacesList.Count)];
            //PoolSurface(x);
            SpawnSurface();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransfrom.position.z - spawnSafeZone > (spawnZ - amountSurfaceOnScreen * surfaceLength))
        {
            //for (int i = 0; i < amountSurfaceOnScreen; i++)
            //{
            //    var x = surfacesList[Random.Range(0, surfacesList.Count)];
            //    PoolSurface(x);
            //}
            SpawnSurface();
        }
    }

    private void SpawnSurface(int prefabIndex = -1)
    {
        GameObject go = null; //When spawn first time
        int randomIndex = Random.Range(0, structurePrefab.Length);
        GameObject lastSurface = null;

        if (activeSurface.Count > 0)
        {
            lastSurface = activeSurface[index - 1];
            var spawnPos = MeshBoundSize(lastSurface);
            //for (int i = 0; i < amountSurfaceOnScreen; i++)
            go = Instantiate(structurePrefab[randomIndex], new Vector3(0, 0, spawnPos.z + lastSurface.transform.position.z), Quaternion.identity) as GameObject;
        }
        else
        {
            lastSurface = null;
            go = Instantiate(structurePrefab[randomIndex], transform.position, Quaternion.identity) as GameObject;
        }
        go.transform.SetParent(transform);
        spawnZ += CalculateMeshSize(go);
        activeSurface.Add(go);
        index++;
    }

    private void SpawnAllSurfaces()
    {
        int randomIndex = Random.Range(0, structurePrefab.Length);
        GameObject go = Instantiate(structurePrefab[randomIndex]) as GameObject;
        go.SetActive(false);
        surfacesList.Add(go);
    }

    private void PoolSurface(GameObject go, int prefabIndex = -1)
    {
        go.SetActive(true);
        GameObject lastSurface = null;

        if (activeSurface.Count > 0)
        {
            lastSurface = activeSurface[index - 1];
            var spawnPos = MeshBoundSize(lastSurface);
            go.transform.position = new Vector3(0, 0, spawnPos.z + lastSurface.transform.position.z);
        }
        else
        {
            lastSurface = null;
            go.transform.position = transform.position;
        }
        go.transform.SetParent(transform);
        spawnZ += CalculateMeshSize(go);
        activeSurface.Add(go);
        index++;
    }

    private void DestroySurface()
    {
        Destroy(activeSurface[0]);
        activeSurface.RemoveAt(0);
    }

    private Vector3 MeshBoundSize(GameObject go)
    {
        var mesh = go.GetComponentInChildren<MeshFilter>().mesh;
        return mesh.bounds.size;
    }

    private float CalculateMeshSize(GameObject go)
    {
        var mesh = go.GetComponentInChildren<MeshFilter>().mesh;
        return mesh.bounds.size.z * transform.localScale.z;
    }
}
