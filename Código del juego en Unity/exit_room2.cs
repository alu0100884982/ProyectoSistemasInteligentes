using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class exit_room2 : MonoBehaviour
{

    public GameObject llave;
    public TextMesh num;
    public GameObject ayuda_panel;
    public TextMesh ayuda;
    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if ((llave.activeSelf) || (num.text != "2000"))
            {
                ayuda_panel.SetActive(true);

                ayuda.text = "Aún no puedes salir de la habitación.";
            }
            else
            {
                ayuda_panel.SetActive(true);
                ayuda.text = "Siguiente escena...";
                SceneManager.LoadScene("highway");
            }
        }
    }
}
