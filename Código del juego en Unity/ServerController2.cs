using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerController2 : MonoBehaviour {

    public TextMesh dedos;
    static bool posicion_dedos;
    public static int count;
    public static int activar_cuatro;
    static bool activador_cuatro;
    public static int activar_dos;
    static bool activador_dos;
    public static int activar_cinco;
    static bool activador_cinco;
    static bool micro_on;
    static bool tengo_llave;
    public TextMesh num;
    public TextMesh ayuda;
    public GameObject ayuda_panel;
    public GameObject llave;

    // Use this for initialization
    void Start () {
        posicion_dedos = false;
        activar_cuatro = 0;
        activador_cuatro = false;
        activar_dos = 0;
        activador_dos = false;
        activar_cinco = 0;
        activador_cinco = false;
        tengo_llave = false;
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
        if (count == 2)
        {
            activador_dos = true;
        }
        if (count == 5)
        {
            activador_cinco = true;
        }
    }

        // Update is called once per frame
        void Update () {
        if (posicion_dedos)
        {
            dedos.text = "Nº de Dedos: " + count;
        }
        if (activador_cuatro)
        {
            ayuda_panel.SetActive(true);
            ayuda.text = "Micrófono activado";
            KeywordRoom2.Empezar();
            activador_cuatro = false;
        }
        if (activador_dos)
        {
            if ((llave.activeSelf) && (num.text == "2000"))
            {
                ayuda_panel.SetActive(true);
                ayuda.text = "Ya tengo la llave de la puerta, tengo que ver a Leslie.";
                llave.SetActive(false);
                tengo_llave = true;

            }
            else if(!tengo_llave)
            {
                ayuda_panel.SetActive(true);
                ayuda.text = "Aún no puedo ver a Leslie, necesito el código para coger la llave de la puerta.";
            }
            activador_dos = false;
        }
        if ((activador_cinco)&&(!tengo_llave))
        {
            if (num.text != "2000")
            {
                ayuda_panel.SetActive(true);
                ayuda.text = "Mmmmm, no recuerdo la combinación, quizás podría probar con el año en el que empecé a salir con Leslie...\nLa echo tanto de menos...";
            }
            else
            {
                ayuda_panel.SetActive(true);
                ayuda.text = "2000...el año que empezamos a salir...Necesito ver a Leslie, voy a salir de aquí...\nPulsa e o enseña 2 dedos para coger la llave.";
                llave.SetActive(true);
            }
            activador_cinco = false;
        }
    }
}
