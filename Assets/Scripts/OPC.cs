﻿using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System;

struct Pixel
{
    public byte r;
    public byte g;
    public byte b;

    public Pixel(byte r, byte g, byte b)
    {
        this.r = r;
        this.g = g;
        this.b = b;
    }
}

public class OPCClient
{
    public const int HDR_SIZE = 4;
    public byte[] payload = new byte[1 << 16];
    public byte[] hdr = new byte[HDR_SIZE];
    public byte command;
    public byte channel;
    public int payload_expected;
    public TcpClient client;

    public OPCClient(TcpClient client)
    {
        this.client = client;
    }
}

public class OPC : MonoBehaviour
{
    private bool mRunning;
    private Thread mThread;
    private TcpListener listener = null;

    // OPC broadcast channel.
    const int OPC_BROADCAST = 0;

    // Command codes.
    const int OPC_SET_PIXELS = 0;

    // Max number of OPC sinks or sources allowed.
    const int OPC_MAX_SINKS = 64;
    const int OPC_MAX_SORUCES = 64;

    // Max number of pixels per message.
    const int OPC_MAX_PIXELS_PER_MESSAGE = ((1 << 16) / 3);


    void Start()
    {
        listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7890);
        listener.Start();
        listener.BeginAcceptTcpClient(OPCHandler, null);
        Debug.Log("OPC: Server started listening for clients...");
    }

    private void OPCHandler(IAsyncResult accept_result)
    {
        TcpClient client = listener.EndAcceptTcpClient(accept_result);

        var opclient = new OPCClient(client);

        // Listen again.
        listener.BeginAcceptTcpClient(OPCHandler, null);

        Debug.Log("OPC: Client connected from " + client.Client.RemoteEndPoint.ToString());
        var stream = client.GetStream();

        stream.BeginRead(opclient.hdr, 0, OPCClient.HDR_SIZE, new AsyncCallback(ReadOPCHeader), opclient);
    }

    private void ReadOPCPayload(IAsyncResult payload_result)
    {
        var opclient = payload_result.AsyncState as OPCClient;
        NetworkStream stream = opclient.client.GetStream();
        int payload_len = stream.EndRead(payload_result);

        if (payload_len == 0)
        {
            Debug.Log("Disconnected ");
        }
        else
        {
            Debug.Log("Read " + payload_len);

            if (opclient.command == OPC_SET_PIXELS)
            {
                //PixelHandler(opclient.channel, (ushort)(payload_len / 3), opclient.payload);
            }
        }

        stream.BeginRead(opclient.hdr, 0, OPCClient.HDR_SIZE, new AsyncCallback(ReadOPCHeader), opclient);
    }

    private void ReadOPCHeader(IAsyncResult hdr_result)
    {
        var opclient = hdr_result.AsyncState as OPCClient;
        NetworkStream stream = opclient.client.GetStream();

        int hdr_len = stream.EndRead(hdr_result);
        opclient.channel = (byte)opclient.hdr[0];
        opclient.command = (byte)opclient.hdr[1];
        opclient.payload_expected = (((byte)opclient.hdr[2] << 8) | (byte)opclient.hdr[3]);
        Debug.Log("Packet ch " + opclient.channel + " cmd " + opclient.command + " => " + opclient.payload_expected);

        // Listen for next packet.
        if (opclient.payload_expected < opclient.payload.Length)
        {
            opclient.payload = new byte[opclient.payload_expected];
        }

        stream.BeginRead(opclient.payload, 0, opclient.payload_expected, new AsyncCallback(ReadOPCPayload), opclient);
    }

    void Update()
    {

    }

    void PixelHandler(ushort channel, ushort count, byte[] data)
    {
        for (ushort i = 0; i < count; i++)
        {
            Pixel p = new Pixel(data[i], data[i + 1], data[i + 2]);
            Debug.Log(p.r + " " + p.g + " " + p.b);
        }
    }
}