using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchingPerception : Perception {

    #region variables

    private GameObject watcher;
    private GameObject target;
    private MeshCollider colliderVision;
    private RaycastHit rayHit;
    private Ray ray;

    #endregion variables

    public WatchingPerception(GameObject watcher, GameObject target, MeshCollider colliderVision)
    {
        this.watcher = watcher;
        this.target = target;
        this.colliderVision = colliderVision;
    }

    public override bool Check()
    {
        if(colliderVision.bounds.Contains(target.transform.position)) {
            Vector3 direction = (target.transform.position - watcher.transform.position).normalized;
            ray = new Ray(watcher.transform.position + watcher.transform.up, direction * 20);

            if(Physics.Raycast(ray, out RaycastHit hit, 20) && hit.collider.gameObject.tag.Equals("Player")) {
                rayHit = hit;
                return true;
            }
        }

        return false;
    }

    public RaycastHit GetRaycastHit()
    {
        return rayHit;
    }

    public Ray GetRay()
    {
        return ray;
    }
}