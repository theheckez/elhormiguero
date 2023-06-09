using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{   
    [SerializeField]
    private GameObject food;
    [SerializeField]
    private GameObject predator;
 
    public float Radius = 1;
    private int x, y;
    public bool foodSpawn = true;
    
    public int maxApples = 3;
    public int actualFood = 0;

    void Start()
    {

    }

    void Update()
    {   
        if(Input.GetKeyDown(KeyCode.Space)) SpawnObjectAtRandom();
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(this.transform.position, Radius);
    }

    void SpawnObjectAtRandom() {
        Vector3 randomPos = Random.insideUnitCircle * Radius;

        if(foodSpawn && actualFood < maxApples) {
            Instantiate(food,randomPos,Quaternion.identity);
            foodSpawn = false;
            actualFood++;
        } else if(!foodSpawn){
            Instantiate(predator,randomPos,Quaternion.identity);
            foodSpawn = true;
        }
    }
   

}
