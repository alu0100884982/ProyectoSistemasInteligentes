using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using UnityEngine.SceneManagement;

public class Server : MonoBehaviour
{
    Thread receiveThread;
    UdpClient client;
    public int port;
    public int aux;
    Scene currentScene;
    string sceneName;
    // Use this for initialization
    void Start()
    {
        init();
        currentScene = SceneManager.GetActiveScene();
    }

    private void init()
    {
        Debug.Log("UPDSend.init()");
        port = 5065;
        Debug.Log("Sending to 127.0.0.1 : " + port);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);
                int position = Int32.Parse(text);
                position--;
                Debug.Log("Los dedos mostrados son : " + position);
                aux = position;
            }
            catch (Exception e)
            {
                Debug.Log("ERROR : " + e.ToString());
            }
        }
    }

    void OnApplicationQuit()
    {
        if (receiveThread != null)
        {
            receiveThread.Abort();
            client.Close();
            Debug.Log(receiveThread.IsAlive); //must be false
        }
    }
    public void Update()
    {
        sceneName = currentScene.name;
        if (sceneName == "Room_childhood")
        {
            ServerController.get_position(aux);
        }
        else if (sceneName == "Room_teen")
        {
            ServerController2.get_position(aux);
        }
    }
}
