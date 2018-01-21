using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class key_button : MonoBehaviour
{

    public Rigidbody rb;
    public GameObject key;
    public GameObject ayuda_panel;
    public TextMesh ayuda;

    public void press()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        ayuda_panel.SetActive(true);
        ayuda.text = "Busca por la habitación.";
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ayuda_panel.SetActive(true);
            ayuda.text = "Has cogido la llave. Ya puedes salir de la habitación.";
            key.SetActive(false);
        }
    }
}