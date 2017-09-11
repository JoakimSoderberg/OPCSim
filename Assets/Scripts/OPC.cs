using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

public class OPC : MonoBehaviour
{
    private bool mRunning;
    private Thread mThread;
    private TcpListener listener = null;

    void Start()
    {
        mRunning = true;
        ThreadStart ts = new ThreadStart(Receive);
        mThread = new Thread(ts);
        mThread.Start();
        Debug.Log("Started OCP Thread...");
    }

    public void stopListening()
    {
        mRunning = false;
    }

    void Receive()
    {
        listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7890);
        listener.Start();
        Debug.Log("Server start");

        while (mRunning)
        {
            // check if new connections are pending, if not, be nice and sleep 100ms
            if (!listener.Pending())
            {
                Thread.Sleep(100);
            }
            else
            {
                TcpClient client = listener.AcceptTcpClient();
                Debug.Log("OPC: Client connected from " + client.Client.RemoteEndPoint.ToString());
                /*Socket ss = tcp_Listener.AcceptSocket();
                Debug.Log("OPC: Client connected from " + ss.a);
                byte[] tempbuffer = new byte[10000];
                ss.Receive(tempbuffer); // received byte array from client
                */
            }
        }
    }

    void Update()
    {

    }

    void OnApplicationQuit()
    {
        // stop listening thread
        stopListening(); // wait for listening thread to terminate (max. 500ms)
        mThread.Join(500);
    }
}