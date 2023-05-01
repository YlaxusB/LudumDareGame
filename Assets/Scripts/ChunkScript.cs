using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkScript : MonoBehaviour
{
    public float speed = 70;
    public int chunkLength;
    public RoadGenerator roadGenerator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, 0, -speed * Time.deltaTime);

        if (transform.position.z < -chunkLength * 3)
        {
            Destroy(gameObject);
            roadGenerator.GenerateChunk();
        }
    }
}
