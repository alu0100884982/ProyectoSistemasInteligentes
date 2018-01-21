using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class posicionJugador : MonoBehaviour {

    public Vector3 PosPlayer;
    public Quaternion RotPlayer;


    // Update is called once per frame
    void Update()
    {
        PosPlayer = GameObject.FindGameObjectWithTag("Player").transform.position;
        RotPlayer = GameObject.FindGameObjectWithTag("Player").transform.rotation;

    }
}
