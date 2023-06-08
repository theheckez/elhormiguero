using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Camera movement controll
    public float movSpeed = 20f;
    public float BorderThikness = 10f;

    // Camera spectre limit
    public Vector2 limit;

    // Camera zoom controll
    public float scrollSpeed = 20f;
    public float minZ = 20f;
    public float maxZ = 120f;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;

        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - BorderThikness)
        {
            pos.y += movSpeed * Time.deltaTime;
        }

        if (Input.GetKey("s") || Input.mousePosition.y <= BorderThikness)
        {
            pos.y -= movSpeed * Time.deltaTime;
        }

        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - BorderThikness)
        {
            pos.x += movSpeed * Time.deltaTime;
        }

        if (Input.GetKey("a") || Input.mousePosition.x <= BorderThikness)
        {
            pos.x -= movSpeed * Time.deltaTime;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.z -= scroll * scrollSpeed * 100f * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, -limit.x, limit.x);
        pos.y = Mathf.Clamp(pos.y, -limit.y, limit.y);
        transform.position = pos;

        if (Input.GetMouseButton(0))
        {

        }
    }
}
