using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteButtonScript : MonoBehaviour
{

    public GameObject player;
    void Start()
    {
        transform.GetComponent<Button>().onClick.AddListener(MuteMusic);
    }
    private void MuteMusic()
    {
        player.GetComponent<AudioSource>().mute = !player.GetComponent<AudioSource>().mute;
    }
}
