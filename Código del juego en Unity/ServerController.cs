using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerController : MonoBehaviour {

    public static int count;
    public static int activar_cuatro;
    static bool activador_cuatro;
    public static int activar_tres;
    static bool activador_tres;
    public static int activar_dos;
    static bool activador_dos;
    public static int activar_cinco;
    static bool activador_cinco;
    static bool micro_on;
    public TextMesh ayuda;
    public GameObject ayuda_panel;
    public TextMesh num1;
    public TextMesh num2;
    public TextMesh num3;
    public TextMesh num4;
    public TextMesh dedos;
    static bool posicion_dedos;
    static bool abrir_cofre;

    // Use this for initialization
    void Start()
    {
        activar_cuatro = 0;
        activador_cuatro = false;
        activar_tres = 0;
        activador_tres = false;
        activar_dos = 0;
        activador_dos = false;
        activar_cinco = 0;
        activador_cinco = false;
        abrir_cofre = false;
        posicion_dedos = false;
        micro_on = false;
    }
	
    public static void get_position(int position)
    {
        count = position;
        posicion_dedos = true;
        if (position == 4)
        {
            ++activar_cuatro;
            comprobar_dedos();
        }
        if (position == 3)
        {
            ++activar_tres;
            comprobar_dedos();
        }
        if (position == 2)
        {
            ++activar_dos;
            comprobar_dedos();
        }
        if (position == 5)
        {
            ++activar_cinco;
            comprobar_dedos();
        }
    }
    public static void comprobar_dedos()
    {
        if (((count == 4) && (activar_cuatro == 1)) || ((count == 4) && (micro_on == false)))
        {
            activador_cuatro = true;
            micro_on = true;
        }
       /*if (((count == 3) && (activar_tres == 1)) || ((count == 3) && (micro_on == true)))
        {
            activador_tres = true;
            micro_on = false;
        }*/
        if ((count == 2) && (activar_dos < 5))
        {
            activador_dos = true;
        }
        if ((count == 5) && (activar_cinco < 5))
        {
            activador_cinco = true;
        }
    }
    // Update is called once per frame
    public void Update () {
        if (activador_cuatro)
        {
            ayuda_panel.SetActive(true);
            ayuda.text = "Micrófono activado";
            KeywordScript.Empezar();
            activador_cuatro = false;
        }
        else if (activador_tres)
        {
            ayuda_panel.SetActive(true);
            ayuda.text = "Micrófono desactivado";
            KeywordScript.Parar();
            activador_tres = false;
        }
        else if (activador_dos)
        {
            if ((num1.text == "4") && (num2.text == "1") && (num3.text == "2") && (num4.text == "3"))
            {
                abrir_cofre = true;
            }
            else
            {
                ayuda_panel.SetActive(true);
                ayuda.text = "Aún no puedes abrir el cofre.";
            }
            animacion_tesoro.abrir(abrir_cofre);
            activador_dos = false;
        }
        else if (activador_cinco)
        {
            if ((num1.text == "4") && (num2.text == "1") && (num3.text == "2") && (num4.text == "3"))
            {
                ayuda_panel.SetActive(true);
                ayuda.text = "Los libros tienen el número correcto, ya puedes abrir el cofre.";

            }
            else
            {
                ayuda_panel.SetActive(true);
                ayuda.text = "Los libros no tienen el número correcto, prueba otra vez.";
            }
            activador_cinco = false;
        }
        if (posicion_dedos)
        {
            dedos.text = "Nº de Dedos: " + count;
        }
    }
}
