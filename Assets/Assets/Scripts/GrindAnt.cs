using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindAnt : MonoBehaviour
{

    StateManager stateManager;

    //Perceptions
    public float life;
    bool grindOrder;
    bool clicked = false;
    bool hit = false;
    public UnityEngine.AI.NavMeshAgent playerAgent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Debug.Log("Clicked");
            FindClickPos();
        }
        if (Input.GetMouseButton(1) && clicked == true)
        {
            Debug.Log("I clicked right mouse");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                MoveTo(hit);
            }
        }
    }

    private void FindClickPos()
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
        if (hit == true)
        {
            Debug.Log("I hit " + hitInfo.collider.name);
            if (hitInfo.collider.tag == "recolectora")
            {
                Debug.Log("I hit" + hitInfo.collider.tag + "!");
                clicked = true;
            }
            else
            {
                Debug.Log("I didnt hit a unit :/");
                clicked = false;
            }
        }
        else
        {
            Debug.Log("I didnt hit anything...");
            clicked = false;
        }
    }
    void MoveTo(RaycastHit hit)
    {
        Debug.Log("Position: " + hit.point);
        playerAgent.SetDestination(hit.point);
    }
}
