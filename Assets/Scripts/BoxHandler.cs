using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BoxHandler : MonoBehaviour
{
    public Transform handObject;
    public Transform cameraTransform;
    public bool isHoldingBox = false;
    public Transform box;

    public Transform trajectoryTransform;
    public Transform trajectoryTransform2;
    public float length = 10f;
    public float strength = 10f;
    public float gravity = -9.81f;
    public GameObject boxPrefab;
    public ScoreHandler scoreHandler;
    public int startingBoxes = 5;
    public bool preventSpam = false;
    public RoadGenerator roadGenerator;
    public PlayerController playerController;
    float lastBoxZ = 0;

    private void Start()
    {
        for(int i = 0; i < startingBoxes; i++)
        {
            SpawnRandomBox();
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!isHoldingBox && collider.transform.tag == "Box" && collider.GetComponent<BoxScript>().isThrowing == false)
        {
            // Set the new box to the script
            isHoldingBox = true;
            box = collider.transform;

            // Completely stops rigidBody
            Rigidbody boxRigidBody = box.GetComponent<Rigidbody>();
            boxRigidBody.velocity = Vector3.zero;
            boxRigidBody.position = Vector3.zero;
            boxRigidBody.angularVelocity = Vector3.zero;
            boxRigidBody.angularDrag = 0;
            boxRigidBody.freezeRotation = true;
            boxRigidBody.useGravity = false;
            boxRigidBody.isKinematic = true;


            // Move the new box to player's body
            box.transform.SetParent(cameraTransform);
            box.transform.localPosition = new Vector3(0.9399955f, -1.555926f, 1.083517f);
            box.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            box.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            box.GetComponent<BoxScript>().isBeingHold = true;
        }
    }

    private void Update()
    {
        // Show trajectory when holding a box
        if (isHoldingBox && !playerController.isRestarting)
        {
            //
            // Get the trajectory points
            List<Vector3> points = GetTrajectoryPoints(((int)length), length, gravity, strength);
            // Only continue if theres more than 1 point
            if (points.Count > 1)
            {
                // Enable trajectories
                trajectoryTransform.GetComponent<Renderer>().enabled = true;
                trajectoryTransform2.GetComponent<Renderer>().enabled = true;
                // Build the meshes with the points
                Mesh firstMesh= BuildMeshAlongPoints(points, 1.5f, false);
                Mesh secondMesh = BuildMeshAlongPoints(points, 1.5f, true);
                // Apply the meshes
                trajectoryTransform.GetComponent<MeshFilter>().mesh = firstMesh;
                trajectoryTransform2.GetComponent<MeshFilter>().mesh = secondMesh;
            }
            // Add boxScript to box when left click (while holding a box)
            if (Input.GetMouseButtonDown(0))
            {
                BoxScript boxScript = box.GetComponent<BoxScript>();
                boxScript.points = points;
                boxScript.LocalToWorld(points);
                boxScript.trajectoryTransform = trajectoryTransform;
                boxScript.isThrowing = true;
                boxScript.isBeingHold = false;
                boxScript.scoreHandler = scoreHandler;

                boxScript.boxHandler = this;
                box.parent = null;
                box = null;
                isHoldingBox = false;
            }
        } else
        {
            // Disable trajectory when no more holding right button
            trajectoryTransform.GetComponent<Renderer>().enabled = false;
            trajectoryTransform2.GetComponent<Renderer>().enabled = false;
        }
    }
    // Spawn new random boxes
    public void SpawnRandomBox()
    {
        // Forward
        float randomBoxZ = Random.Range(200 + roadGenerator.gameSpeed, 600);
        // Sides
        float randomBoxX = Random.Range(-15, 15);
        Transform newBox = Instantiate(boxPrefab).transform;
        BoxScript newBoxScript = newBox.GetComponent<BoxScript>();
        newBoxScript.trajectoryTransform = trajectoryTransform;
        newBoxScript.boxHandler = this;
        newBoxScript.scoreHandler = scoreHandler;
        newBoxScript.roadGenerator = roadGenerator;
        newBox.position = new Vector3(randomBoxX, 15, randomBoxZ);
        lastBoxZ = randomBoxZ;
    }


    // Get the trajectory points, with gravity and strength
    private List<Vector3> GetTrajectoryPoints(int pointLength, float length, float gravity, float strength)
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 start = Vector3.zero;
        points.Add(start);
        int i = 0;
        Vector3 pointEnd = Vector3.zero;
        Vector3 point = Vector3.zero;
        while (cameraTransform.TransformPoint(new Vector3(0, point.y, point.x)).y >= -4)
        {
            pointEnd = cameraTransform.TransformPoint(points[points.Count - 1]);
            float t = (float)i / (pointLength * gravity);
            point = new Vector3(points[points.Count - 1].x + 0.5f, 0, 0);
            point.y += gravity * t * t;
            points.Add(point);
            i++;
        }
        return points;
    }

    // Build mesh along points
    private Mesh BuildMeshAlongPoints(List<Vector3> points, float width, bool invertNormals)
    {
        List<Vector3> verts = new List<Vector3>();
        for (int i = 0; i < points.Count; i++)
        {
            verts.Add(new Vector3(points[i].x, points[i].y, width));
            verts.Add(new Vector3(points[i].x, points[i].y, -width));
        }

        // Create the uvs
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < points.Count; i++)
        {
            float completionPercent = 1 / (points.Count - 1);
            uvs.Add(new Vector2(0, completionPercent));
            uvs.Add(new Vector2(1, completionPercent));
        }

        // Create the triangles
        List<int> triangles = new List<int>();
        if (!invertNormals)
        {
            for (int i = 0; i < 2 * (points.Count - 1); i += 2)
            {
                triangles.Add(i);
                triangles.Add(i + 2);
                triangles.Add(i + 1);

                triangles.Add(i + 1);
                triangles.Add(i + 2);
                triangles.Add(i + 3);
            }
        }
        else
        {
            for (int i = 0; i < 2 * (points.Count - 1); i += 2)
            {
                triangles.Add(i);
                triangles.Add(i + 1);
                triangles.Add(i + 2);

                triangles.Add(i + 1);
                triangles.Add(i + 3);
                triangles.Add(i + 2);
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        return mesh;
    }
}
