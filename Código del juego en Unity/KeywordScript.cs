using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class KeywordScript : MonoBehaviour
{
   
    private string[] m_Keywords = {"Libro azul uno", "Libro azul dos", "Libro azul tres", "Libro azul cuatro", "Libro rojo uno", "Libro rojo dos", "Libro rojo tres","Libro rojo cuatro", "Libro verde uno", "Libro verde dos", "Libro verde tres", "Libro verde cuatro", "Libro amarillo uno", "Libro amarillo dos", "Libro amarillo tres", "Libro amarillo cuatro"};

    private static KeywordRecognizer m_Recognizer;

    public TextMesh num1;

    public TextMesh num2;

    public TextMesh num3;

    public TextMesh num4;

    public TextMesh ayuda;

    public GameObject ayuda_panel;

    void Start()
    {
        m_Recognizer = new KeywordRecognizer(m_Keywords);
        m_Recognizer.OnPhraseRecognized += OnPhraseRecognized;

    }
    public static void Empezar()
    {
 
        m_Recognizer.Start();

    }
    public static void Parar()
    {

        m_Recognizer.Stop();
        PhraseRecognitionSystem.Shutdown();

    }
    public void Empezar_boton()
    {

        m_Recognizer.Start();

    }
    public void Parar_boton()
    {

        m_Recognizer.Stop();
        PhraseRecognitionSystem.Shutdown();

    }
    public void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} ({1}){2}", args.text, args.confidence, Environment.NewLine);
            builder.AppendFormat("\tTimestamp: {0}{1}", args.phraseStartTime, Environment.NewLine);
            builder.AppendFormat("\tDuration: {0} seconds{1}", args.phraseDuration.TotalSeconds, Environment.NewLine);
            Debug.Log(builder.ToString());
            Debug.Log(args.text);
            change_number(args.text);
    }
    public void get_key()
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
    }
    public void change_number(string word)
    {
        ayuda_panel.SetActive(false);
        if (word == "Libro azul uno")
        {
            num1.text = "1";
        }
        else if (word == "Libro azul dos")
        {
            num1.text = "2";
        }
        else if (word == "Libro azul tres")
        {
            num1.text = "3";
        }
        else if (word == "Libro azul cuatro")
        {
            num1.text = "4";
        }
        else if (word == "Libro rojo uno")
        {
            num2.text = "1";
        }
        else if (word == "Libro rojo dos")
        {
            num2.text = "2";
        }
        else if (word == "Libro rojo tres")
        {
            num2.text = "3";
        }
        else if (word == "Libro rojo cuatro")
        {
            num2.text = "4";
        }
        else if (word == "Libro verde uno")
        {
            num3.text = "1";
        }
        else if (word == "Libro verde dos")
        {
            num3.text = "2";
        }
        else if (word == "Libro verde tres")
        {
            num3.text = "3";
        }
        else if (word == "Libro verde cuatro")
        {
            num3.text = "4";
        }
        else if (word == "Libro amarillo uno")
        {
            num4.text = "1";
        }
        else if (word == "Libro amarillo dos")
        {
            num4.text = "2";
        }
        else if (word == "Libro amarillo tres")
        {
            num4.text = "3";
        }
        else if (word == "Libro amarillo cuatro")
        {
            num4.text = "4";
        }
    }
}

