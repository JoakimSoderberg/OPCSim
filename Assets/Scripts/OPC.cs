using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System;
using UnityToolbag;

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
    public DateTime last_receive;

    public OPCClient(TcpClient client)
    {
        this.client = client;
        this.last_receive = DateTime.Now;
    }
}

public class OPC : MonoBehaviour
{
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
        listener = new TcpListener(IPAddress.Any, 7890);
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
            //Debug.Log("Read " + payload_len);

            if (opclient.command == OPC_SET_PIXELS)
            {
                // We skip drawing some frames so we don't get overwhelmed, only update every 20ms.
                if (DateTime.Now.Subtract(opclient.last_receive).Milliseconds >= 20)
                {
                    PixelHandler(opclient.channel, (ushort)(payload_len / 3), opclient.payload);
                    opclient.last_receive = DateTime.Now;
                }
            }
        }

        stream.BeginRead(opclient.hdr, 0, OPCClient.HDR_SIZE, new AsyncCallback(ReadOPCHeader), opclient);
    }

    private void ReadOPCHeader(IAsyncResult hdr_result)
    {
        var opclient = hdr_result.AsyncState as OPCClient;
        NetworkStream stream = opclient.client.GetStream();

        int hdr_len = stream.EndRead(hdr_result);
        if (hdr_len == 0)
        {
            Debug.Log("Disconnected");
            return;
        }
        opclient.channel = (byte)opclient.hdr[0];
        opclient.command = (byte)opclient.hdr[1];
        opclient.payload_expected = (((byte)opclient.hdr[2] << 8) | (byte)opclient.hdr[3]);
        //Debug.Log("Packet ch " + opclient.channel + " cmd " + opclient.command + " => " + opclient.payload_expected);

        // Listen for next packet.
        if (opclient.payload_expected > opclient.payload.Length)
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
        //Debug.Log("Channel => " + channel + " count => " + count);

        Dispatcher.Invoke(() =>
        {
            int led_addr = 0;
            for (ushort i = 0; i < count; i++)
            {
                if (i >= LEDCreator.all_leds.Count)
                    break;

                float r = data[led_addr] / 255.0f;
                float g = data[led_addr + 1] / 255.0f;
                float b = data[led_addr + 2] / 255.0f;
                led_addr += 3;

                LEDCreator.all_leds[i].SetColor(new Color(r, g, b));
            }
        });
    }
}