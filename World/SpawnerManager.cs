using System.Linq;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] private GameObject[] foodPrefabs;
    [SerializeField] Transform[] spawnPoints;
    private float minX, maxX, minY, maxY;
    public static SpawnerManager Instance {get; private set;}

    public void SpawnFood(FoodType type)
    {
        Vector2 randomPos = new Vector2(Random.Range(minX,maxX),Random.Range(minY,maxY));
        Instantiate(foodPrefabs[(int)type], randomPos, Quaternion.identity);
    }
    void Start()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        } 
        else 
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        maxX = spawnPoints.Max(point => point.position.x);
        minX = spawnPoints.Min(point => point.position.x);
        maxY = spawnPoints.Max(point => point.position.y);
        minY = spawnPoints.Min(point => point.position.y);
    }

    void Update()
    {
         if(Input.GetMouseButtonDown(0)) SpawnFood(FoodType.Fruits);
    }


    
}

public enum FoodType {Fruits, Vegetables, Seeds}


