using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.IO;
using System;
using Unity.Loading;

public class ChatClient : MonoBehaviour
{
    public InputField NickInput;
    string clientName;

    bool socketReady;
    TcpClient socket;
    NetworkStream stream;
    StreamWriter writer;
    StreamReader reader;


    public void ConnectToServer()
    {
        if (socketReady) return;
        
        string ip = "127.0.0.1";
        int port = 7777;

        try
        {
            socket = new TcpClient(ip, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            socketReady = true;

        }
        catch (Exception e)
        {
            Chat.instance.ShowMessage($"Socket error : {e.Message}");
        }

    }

    void Update()
    {
        if (socketReady && stream.DataAvailable)
        {
            string data = reader.ReadLine();
            if (data != null)
            {
                OnIncomingData(data);
            }
        }
    }

    void OnIncomingData(string data)
    {
        if (data == "%NAME")
        {
            clientName = NickInput.text == "" ? "Guest" + UnityEngine.Random.Range(1000, 10000) : NickInput.text;
            Send($"&NAME|{clientName}");
            return;
        }

        Chat.instance.ShowMessage(data);
    }

    void Send(string data)
    {
        if (!socketReady) return;

        writer.WriteLine(data);
        writer.Flush();
    }

    public void OnSendButton(InputField SendInput)
    {
        #if (UNITY_EDITOR || UNITY_STANDALONE)
        if (!Input.GetButtonDown("Submit")) return;
        SendInput.ActivateInputField();
        #endif
        if (SendInput.text.Trim() == "") return;

        string message = SendInput.text;
        SendInput.text = "";
        Send(message);
    }
    
    void OnApplicationQuit()
    {
        CloseSocket();
    }

    void CloseSocket()
    {
        if (!socketReady) return;

        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }
}
