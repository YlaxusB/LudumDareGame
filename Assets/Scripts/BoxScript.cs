using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    public Transform handObject;
    public bool isHoldingBox = false;
    public Transform box;

    public Transform debugObject;

    public Transform trajectoryTransform;
    public Transform trajectoryTransform2;
    public float length = 10f;
    public float strength = 10f;
    public float gravity = -9.81f;

    private void OnTriggerEnter(Collider collider)
    {
        if(!isHoldingBox && collider.transform.tag == "Box")
        {
            // Set the new box to the script
            isHoldingBox = true;
            box = collider.transform;

            // Move the new box to player's body
            box.transform.SetParent(handObject);
            box.transform.localPosition = new Vector3(0.94f, 0.38f, 1.092f);
            box.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
    }

    private void Update()
    {
        // Delethe the box with mouse left click
        if (Input.GetMouseButtonDown(0))
        {
            isHoldingBox = false;
            Destroy(box.gameObject);
        }

        // Show trajectory with right button
        //if (Input.GetMouseButtonDown(1))
        //{
            List<Vector3> points = GetTrajectoryPoints(((int)length), length, gravity, strength);
        if(points.Count > 0)
        {
            Mesh newMesh = BuildMeshAlongPoints(points, 0.3f, false);
            Mesh secondMesh = BuildMeshAlongPoints(points, 0.3f, true);
            trajectoryTransform.GetComponent<MeshFilter>().mesh = newMesh;
            trajectoryTransform2.GetComponent<MeshFilter>().mesh = secondMesh;
        }


        //      }
    }

    private List<Vector3> GetTrajectoryPoints(int pointLength, float length, float gravity, float strength)
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 start = Vector3.zero;
        points.Add(start);
        int i = 0;
        while (points[points.Count - 1].y >= 0)
        {
            if(points.Count > 2 && points[points.Count - 1].y - points[points.Count - 2].y <= 0.001f)
            {
                Debug.Log("loop break");
                break;
            }
            float t = (float)i / pointLength;
            //Vector3 point = Vector3.Lerp(start, end, t);
            Vector3 point = new Vector3(points[points.Count - 1].x + strength, 0, 0);
            point.y += gravity * t * t * t;
            points.Add(point);
            i++;
        }
        /*
        for (int i = 0; i < pointLength; i++)
        {
            float t = (float)i / pointLength;
            Vector3 point = Vector3.Lerp(start, end, t);
            point.y += gravity * t * t * t;
            points.Add(point);
        }
        */
        //points.Add(end);
        return points;
    }

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
        } else
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
