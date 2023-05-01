using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class BoxScript : MonoBehaviour
{
    public Vector3 next;
    public Vector3 current;
    public List<Vector3> points;
    public List<Vector3> worldPoints = new List<Vector3>();
    public Transform trajectoryTransform;
    public BoxHandler boxHandler;
    public ScoreHandler scoreHandler;
    public RoadGenerator roadGenerator;
    bool collided = false;
    public bool isThrowing = false;
    public bool isBeingHold = false;

    public int gameSpeed;

    public int boxSoundEffect;

    public AudioClip boxSound;
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
            scoreHandler.score += score;
            scoreHandler.UpdateScore(score);

            // Play the sound effect
            GameObject soundObject = new GameObject();
            soundObject.transform.position = transform.position;
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.enabled = true;
            audioSource.spatialize = true;
            audioSource.spatialBlend = 1;
            // Set the minimum and maximum distances for the sound
            audioSource.minDistance = 10;
            audioSource.maxDistance = 10000;
            audioSource.volume = 0.3f;

            audioSource.clip = boxSound;
            audioSource.time = 0.43f;
            audioSource.volume = 0.2f;
            //audioSource.PlayOneShot(boxSound);
            audioSource.Play();
            audioSource.AddComponent<SoundObjectScript>();

            // Destroy the box and spawn a new one
            collided = true;
            Destroy(gameObject);
            boxHandler.SpawnRandomBox();

        }
    }

    List<Vector3> spacedPoints = new List<Vector3>(); // the new list of spaced points


    public void LocalToWorld(List<Vector3> points)
    {
        // Iterate over the points starting from the second point
        for (int i = 0; i < points.Count; i += 20)
        {
            spacedPoints.Add(points[i]);
            worldPoints.Add(trajectoryTransform.TransformPoint(points[i]));
        }
        transform.position = worldPoints[0];

    }
    // Update is called once per frame
    float deviation = 0;

    //public float speed = 100.001f; // Speed at which to follow the points
    private int currentPointIndex = 0; // Index of the current point to follow

    float speed = 1900;
    void Update()
    {
        gameSpeed = roadGenerator.gameSpeed;
        // Check if its close to end
        if (worldPoints.Count > 2)
        {
            deviation += -gameSpeed * Time.deltaTime;
            // Move towards the current point
            transform.position = Vector3.MoveTowards(transform.position, worldPoints[currentPointIndex] + new Vector3(0, 0, deviation), (speed * Time.deltaTime) / 3);

            // If the distance between the box and the next point is smaller than 0.3f, then increment currrentPointIndex to get next point
            if (Vector3.Distance(worldPoints[currentPointIndex] + new Vector3(0, 0, deviation), transform.position) <= 0.01f)
            {
                currentPointIndex++;
                // Destroy the box when it reaches the end of the trajectory without hitting something
                if (currentPointIndex >= worldPoints.Count - 1)
                {
                    Destroy(gameObject);
                    boxHandler.SpawnRandomBox();
                    scoreHandler.timeLeft -= 1f;
                }

            }
        }
        else if ((isThrowing || transform.position.z < -20) && !isBeingHold)
        {
            // Destroy the box and spawn a new one
            Destroy(gameObject);
            boxHandler.SpawnRandomBox();
        }
        if (!isBeingHold && !isThrowing)
        {
            transform.position += new Vector3(0, 0, -gameSpeed * Time.deltaTime);
            transform.GetComponent<Rigidbody>().velocity += new Vector3(0, -gameSpeed * Time.deltaTime, 0);
        }
    }
}
