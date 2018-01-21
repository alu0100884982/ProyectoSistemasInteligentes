using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class exit : MonoBehaviour {

    public GameObject llave;
    public GameObject ayuda_panel;
    public TextMesh ayuda;
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (llave.activeSelf)
            {
                ayuda_panel.SetActive(true);
                ayuda.text = "Aún no puedes salir de la habitación.";
            }
            else
            {
                ayuda_panel.SetActive(true);
                ayuda.text = "Siguiente escena...";
                SceneManager.LoadScene("Room_teen");
            }
        }
    }
}
