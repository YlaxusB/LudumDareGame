using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    public List<GameObject> housesPrefabList = new List<GameObject>();
    public GameObject chunks;
    public GameObject terrainPrefab;
    public GameObject roadPrefab;
    public GameObject invisibleWallRightPrefab;
    public GameObject invisibleWallLeftPrefab;
    public GameObject invisibleFloorPrefab;
    public int chunkLength = 2400;
    public int housesPerChunk = 40;
    private int chunkIndex = 1;
    private float lastZ = 0;

    public Transform lastChunk;
    void Start()
    {
        GenerateChunk();
        GenerateChunk();
        GenerateChunk();
        GenerateChunk();
        GenerateChunk();
        GenerateChunk();
    }
    public GameObject GenerateChunk()
    {
        // Create a new chunk, just an empty gameobject
        Transform newChunk = new GameObject("Chunk" + chunkIndex).transform;
        // Create the objects that will holder houses and boxes
        GameObject housesObject = new GameObject("Houses");
        housesObject.transform.parent = newChunk;
        GameObject boxesObject = new GameObject("Boxes");
        boxesObject.transform.parent = newChunk;


        int newZ = chunkLength * chunkIndex - chunkLength / 2;
        if (lastChunk)
        {
            float chunkNewZ = (lastChunk.TransformPoint(0, 0, chunkLength)).z - 20f;
            newChunk.transform.position = new Vector3(0, 0, chunkNewZ);
            newZ = ((int)chunkNewZ);
            newZ += chunkLength / 2;
        }

        lastChunk = newChunk;
        newChunk.parent = chunks.transform;
        // Create road, terrain and invisible colliders so the player cant go to the terrain
        Transform terrain = Instantiate(terrainPrefab, new Vector3(0, -0.9f, newZ), new Quaternion(), newChunk).transform;
        Transform road = Instantiate(roadPrefab, new Vector3(0, 0, newZ), new Quaternion(), newChunk).transform;
        Transform invisibleWallRight = Instantiate(invisibleWallRightPrefab, new Vector3(20, 150, newZ), Quaternion.Euler(new Vector3(0, 0, 90)), newChunk).transform;
        Transform invisibleWallLeft = Instantiate(invisibleWallLeftPrefab, new Vector3(-20, 150, newZ), Quaternion.Euler(new Vector3(0, 0, -90)), newChunk).transform;
        Transform invisibleFloor = Instantiate(invisibleFloorPrefab, new Vector3(0, 300, newZ), Quaternion.Euler(new Vector3(0, 0, 180)), newChunk).transform;

        // Create random houses
        for (int i = 0; lastZ + chunkLength / housesPerChunk * 2 < chunkLength; i++)
        {
            // Create a value between 0 and 1, or right and left
            int randomSide = Random.Range(0, 2);
            float[] sides = new float[2] { 80, -80 };
            float[] rotations = new float[2] { -90, 90 };
            // Get random Z value, this is to determine how frontwards it will be
            float randomZ = Random.Range(lastZ + 50, lastZ + 50 + (chunkLength / housesPerChunk * 1.25f));
            // Get a random house mesh from the prefab list
            int randomHouseIndex = Random.Range(0, housesPrefabList.Count - 1);
            GameObject randomHousePrefab = housesPrefabList[randomHouseIndex];
            // Positionate the house to left or right and frontwards
            Vector3 randomHousePosition = new Vector3(sides[randomSide], -1f, 0) + newChunk.TransformPoint(new Vector3(0, 0, randomZ));
            // Instantiate and set some parameters
            Transform newRandomHouse = Instantiate(randomHousePrefab, randomHousePosition, Quaternion.Euler(new Vector3(0, rotations[randomSide], 0)), housesObject.transform).transform;
            newRandomHouse.name = "house" + i;

            lastZ = randomZ;
        }
        lastZ = 0;

        chunkIndex++;

        ChunkScript newChunkScript = newChunk.AddComponent<ChunkScript>();
        newChunkScript.chunkLength = chunkLength;
        newChunkScript.roadGenerator = this;
        return newChunk.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            GenerateChunk();
        }
    }
}
