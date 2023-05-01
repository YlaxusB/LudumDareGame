using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkScript : MonoBehaviour
{
    public int chunkLength;
    public RoadGenerator roadGenerator;

    public int gameSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, 0, -roadGenerator.gameSpeed * Time.deltaTime);

        if (transform.position.z < -chunkLength * 3)
        {
            Destroy(gameObject);
            roadGenerator.GenerateChunk();
        }
    }
}
