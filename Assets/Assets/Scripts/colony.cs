using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colony : MonoBehaviour
{
    public explorer explorerPrefab;
    public int numSpawn = 10;
    GameObject[] explorers;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numSpawn; i++)
        {
            SpawnAnt();
        }
        explorers = GameObject.FindGameObjectsWithTag("explorer");
    }

    void SpawnAnt()
    {
        explorer explorer = Instantiate(explorerPrefab, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject exp in explorers)
        {

        }
    }
}
