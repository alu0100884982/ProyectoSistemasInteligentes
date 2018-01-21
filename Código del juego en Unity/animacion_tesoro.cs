using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animacion_tesoro : MonoBehaviour {

    public GameObject objetoanimado;

    public AnimationClip open;

    private static Animation abrirTesoro;

    public GameObject ayuda_panel;

    public TextMesh num1;

    public TextMesh num2;

    public TextMesh num3;

    public TextMesh num4;

    public TextMesh ayuda;

    // Use this for initialization
    void Start () {
        abrirTesoro = objetoanimado.AddComponent<Animation>();
        abrirTesoro.AddClip(open, "open_trasure");
        
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("e"))
        {
            if ((num1.text == "4") && (num2.text == "1") && (num3.text == "2") && (num4.text == "3"))
            {
                ayuda_panel.SetActive(false);
                abrirTesoro.CrossFade("open_trasure");
            }
            else
            {
                ayuda_panel.SetActive(true);
                ayuda.text = "Aún no puedes abrir el cofre.";
            }
        }
    }
    public static void abrir(bool abrir)
    {
        if (abrir)
        {
            abrirTesoro.CrossFade("open_trasure");
        }
    }

}
