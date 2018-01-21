using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class video : MonoBehaviour {
    GameObject plane;
    public GameObject letras;
    public GameObject luz;

    // Use this for initialization
    void Start () {
        plane = GameObject.FindGameObjectWithTag("plane");
    }
	
	// Update is called once per frame
	public void Update () {
        var videoPlayer = plane.GetComponent<UnityEngine.Video.VideoPlayer>();
        if (!videoPlayer.isPlaying)
        {
            letras.SetActive(true);
            luz.SetActive(false);
        }

    }
}
