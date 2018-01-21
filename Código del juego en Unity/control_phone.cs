using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class control_phone : MonoBehaviour {

    public GameObject mensaje;
    public GameObject boton;
    public GameObject recordatorio;
    public TextMesh recordatorio_texto;

    public void leer_mensaje()
    {
        mensaje.SetActive(true);
        boton.SetActive(true);
    }
    public void cerrar_mensaje()
    {
        mensaje.SetActive(false);
        boton.SetActive(false);
        recordatorio.SetActive(true);
        recordatorio_texto.text = "¿Por qué Leslie estaba tan enfada?...No lo recuerdo pero la echo tanto de menos...";
    }
}
