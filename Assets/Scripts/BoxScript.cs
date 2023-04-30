using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    public Vector3 next;
    public Vector3 current;
    public List<Vector3> points;
    public List<Vector3> worldPoints = new List<Vector3>();
    public Transform trajectoryTransform;
    public BoxHandler boxHandler;

    private void Start()
    {
        foreach (var point in points)
        {
            worldPoints.Add(trajectoryTransform.TransformPoint(point));
        }
        current = worldPoints[0];
        transform.position = worldPoints[0];
    }

    float i = 0.0f;
    int j = 0;
    // Update is called once per frame
    void Update()
    {
        // Check if its close to end
        if(j < worldPoints.Count - 2)
        {
            // Check if its distant enough to get next point in the curve
            if (Vector3.Distance(transform.position, current) > 0.3f && i <= 0.0)
            {

                i = 0.0f;
                j++;
                current = worldPoints[j];
                next = worldPoints[j + 1];

            }
            // Lerp between the last point and the next point
            if (i <= 1.0f)
            {
                transform.position = Vector3.Lerp(current, next, i);
                i += 1f;
            }
            else
            {
                i = 0;
            }
        } else
        {
            // Destroy the box and spawn a new one
            Destroy(gameObject);

            boxHandler.SpawnRandomBox();
        }

         
    }
}
