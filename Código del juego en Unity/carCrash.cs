using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class carCrash : MonoBehaviour
{
    public GameObject car;

    public GameObject objetoanimado;

    public AnimationClip caerse;

    private static Animation cayendo;

    void Start()
    {
        /*cayendo = objetoanimado.AddComponent<Animation>();
        cayendo.AddClip(caerse, "caer");*/

    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            car.SetActive(false);
            SceneManager.LoadScene("end");
            // cayendo.CrossFade("caer");

        }
    }
}