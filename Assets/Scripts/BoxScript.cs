using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    public Vector3 next;
    public Vector3 current;
    public List<Vector3> points;
    public List<Vector3> worldPoints = new List<Vector3>();
    public Transform trajectoryTransform;
    public BoxHandler boxHandler;
    public ScoreHandler scoreHandler;
    bool collided = false;
    public bool isThrowing = false;
    public bool isBeingHold = false;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("House") && !collided)
        {
            // Get the house object, if collided with a children of house then get its parent (house)
            Transform collidedHouse = collider.transform;
            if (!collider.gameObject.name.StartsWith("houseObject"))
            {
                collidedHouse = collider.transform.parent;
            }
            // The local position of the box in relation to the collided object
            Vector3 boxLocal = collidedHouse.transform.InverseTransformPoint(transform.position);
            boxLocal.y = boxLocal.y / 2;
            float distance = Vector3.Distance(collidedHouse.GetComponent<HouseScript>().deliverySpot, boxLocal);
            int score = ((int)Mathf.Clamp(200 / distance, 0, 100));
            Debug.Log(score);
            scoreHandler.score += score;
            scoreHandler.UpdateScore();
            // Destroy the box and spawn a new one
            collided = true;
            Destroy(gameObject);
            boxHandler.SpawnRandomBox();
        }
    }

    public void LocalToWorld(List<Vector3> points)
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
    float deviation = 0;
    void Update()
    {
        // Check if its close to end
        if (j < worldPoints.Count - 2)
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
            deviation += -70 * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, next + new Vector3(0, 0, deviation), 170 * Time.deltaTime);
        }
        else if((isThrowing || transform.position.z < -20) && !isBeingHold)
        {
            // Destroy the box and spawn a new one
            Destroy(gameObject);
            boxHandler.SpawnRandomBox();
        }
        if (!isBeingHold && !isThrowing)
        {
            transform.position += new Vector3(0, 0, -70 * Time.deltaTime);
        }
    }
}
