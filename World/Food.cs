using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField] FoodData fruits;
    private SpriteRenderer spriteRenderer;
    private int maxSpriteIndex;


    public float GetFoodValue()
    {
        return fruits.nutritionValue;
    }

    public float GetFoodWeight()
    {
        return fruits.weight;
    }

    public string GetFoodName()
    {
        return gameObject.name;
    }
    void Start()
    {
      
        maxSpriteIndex = fruits.foodSprites.Length;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = fruits.foodSprites[Random.Range(0,maxSpriteIndex)];
        
    }
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D c) 
    {
        if(c.gameObject.tag == "Ant") Destroy(gameObject); 
    }

   
}
