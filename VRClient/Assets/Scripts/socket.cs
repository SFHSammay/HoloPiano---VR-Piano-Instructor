using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System.IO;

public class socket : MonoBehaviour
{
    [SerializeField]
    public List<Transform> node = new List<Transform>();

    private TcpClient tcpClient;
    private NetworkStream stream;
    
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;

    private bool isRecving;
    private int portNumber;

    float[] x = new float[21];
    float[] y = new float[21];
    float[] z = new float[21];

    void Start () {
        Debug.Log("Socket");
        node[0].localPosition = new Vector3(2.3612721680208f, 0, 0.1058638720083f);

        node[1].localPosition = new Vector3(2.462116284003f, 0, 0.1666133394674f);
        node[2].localPosition = new Vector3(2.5289406982081f, 0, 0.2273628069266f);
        node[3].localPosition = new Vector3(2.6297848141903f, 0, 0.2881122743858f);
        node[4].localPosition = new Vector3(2.7184790366807f, 0, 0.3719465394794f);

        node[5].localPosition = new Vector3(2.4669762413997f, 0, 0.3974613158122f);
        node[6].localPosition = new Vector3(2.4815561135899f, 0, 0.4849405489534f);
        node[7].localPosition = new Vector3(2.4985659644785f, 0, 0.5784947288405f);
        node[8].localPosition = new Vector3(2.508285879272f, 0, 0.6501791004424f);

        node[9].localPosition = new Vector3(2.375852040211f, 0, 0.4071812306057f);
        node[10].localPosition = new Vector3(2.3782820189094f, 0, 0.515315282683f);
        node[11].localPosition = new Vector3(2.3965068591471f, 0, 0.6161593986652f);
        node[12].localPosition = new Vector3(2.4f , 0, 0.7f);

        node[13].localPosition = new Vector3(2.3f, 0, 0.4f);
        node[14].localPosition = new Vector3(2.2871578177206f, 0, 0.4800805915567f);
        node[15].localPosition = new Vector3(2.2774379029272f, 0, 0.5772797394914f);
        node[16].localPosition = new Vector3(2.2677179881337f, 0, 0.661114004585f);

        node[17].localPosition = new Vector3(2.2349132757057f, 0, 0.3403568164006f);
        node[18].localPosition = new Vector3(2.2142584567696f, 0, 0.4083962199549f);
        node[19].localPosition = new Vector3(2.2045385419762f, 0, 0.4873705276518f);
        node[20].localPosition = new Vector3(2.1887436804368f, 0, 0.5626998673012f);

        x[0] = 0;
        z[0] = 0;
        // Start the coroutine to connect to the remote endpoint
        Debug.Log("1");
        StartCoroutine(Connect());
        Debug.Log("2");
    }

    IEnumerator Connect () {
        while (true) {
            try {
                Debug.Log("Try reconnect");
                
                tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Parse("192.168.1.106"), 10001);
                isRecving = false;
                stream = tcpClient.GetStream();

                StartCoroutine(SendPackets());
                yield break;
            }
            catch (SocketException) {
                
            }
            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator SendPackets () {
        while (true) {
            try {
                // Send data to the remote endpoint
                string message = "AR, Hello, world!\n";
                byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
            } catch (SocketException ex) {
                Debug.Log("Send: SocketException: " + ex.Message);
                StartCoroutine(Connect());
                yield break;
            } catch (IOException ex) {
                Debug.Log("Send: IOException: " + ex.Message);
                StartCoroutine(Connect());
                yield break;
            }
            // Wait for 3 seconds before sending another packet
            yield return new WaitForSeconds(5f);
        }
    }

    void Update () {
        // Check if data is available to be read from the network stream
        if (stream != null && stream.DataAvailable) {
            // Read data from the network stream
            byte[] receivedData = new byte[1024];
            int bytesReceived = stream.Read(receivedData, 0, receivedData.Length);
            string receivedMessage = System.Text.Encoding.UTF8.GetString(receivedData, 0, bytesReceived);
            Debug.Log("TCP Received message: " + receivedMessage);
            if (receivedMessage.StartsWith("UDP Port:")) {
                portNumber = int.Parse(receivedMessage.Substring("UDP Port: ".Length));
                isRecving = false;
                udp_start(portNumber);
            }
        }

        if (udpClient != null && udpClient.Available > 0) {
            // Receive data from the remote endpoint
            byte[] receivedData = udpClient.Receive(ref remoteEndPoint);
            string receivedMessage = System.Text.Encoding.UTF8.GetString(receivedData);
            //Debug.Log("UDP Received message: " + receivedMessage);
            isRecving = true;
            if (receivedMessage.StartsWith("hand: ")) {
                positionParser(receivedMessage.Substring("hand: ".Length));
                move();
            }
        }
    }

    void udp_start (int port) {
        udpClient = new UdpClient();
        // Set the remote endpoint to send data to (change this to your own IP address and port number)
        remoteEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.106"), port);
        // Send data to the remote endpoint
        StartCoroutine(CheckConnectivty(port));
    }

    IEnumerator CheckConnectivty (int pid) {
        while (isRecving == false && pid == portNumber) {
            // Send data to the remote endpoint
            string message = "Check Connectivty";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            udpClient.Send(data, data.Length, remoteEndPoint);

            // Wait for 3 seconds before sending another packet
            yield return new WaitForSeconds(1f);
        }
    }

    void OnDestroy () {
        // Close the TCP connection when the object is destroyed
        Debug.Log("Check Close");
        if (tcpClient != null) {
            Debug.Log("Close");
            tcpClient.Close();
        }
    }

    void positionParser (string data) {
        string[] nodeVector = data.Split('|');
        for (int i = 0; i < 21; i++) { 
            string[] values = nodeVector[i].Split(',');
            x[i] = (float.Parse(values[2])) * 1 / 2;
            y[i] = (float.Parse(values[3])) * -1;
            z[i] = (float.Parse(values[4])) ;
        }
    }

    void move() {
        for (int i = 0; i < 21; i++) {
            node[i].localPosition = new Vector3(x[i], y[i], z[i]);
            Debug.Log($"Node {i + 1} - X: {x[i]}, Y: {y[i]}, Z: {z[i]}");
        }
        //node[21].localPosition = node[0].localPosition - new Vector3(-0.154f, -0.5000003f, -0.37f);
        /*
        node[22].localPosition = node[21].localPosition + node[4].localPosition - node[0].localPosition;
        node[23].localPosition = node[21].localPosition + node[8].localPosition - node[0].localPosition;
        node[24].localPosition = node[21].localPosition + node[12].localPosition - node[0].localPosition;
        node[25].localPosition = node[21].localPosition + node[16].localPosition - node[0].localPosition;
        node[26].localPosition = node[21].localPosition + node[20].localPosition - node[0].localPosition;
        
        node[27].localPosition = node[21].localPosition + node[3].localPosition - node[0].localPosition;
        node[28].localPosition = node[21].localPosition + node[7].localPosition - node[0].localPosition;
        node[29].localPosition = node[21].localPosition + node[11].localPosition - node[0].localPosition;
        node[30].localPosition = node[21].localPosition + node[15].localPosition - node[0].localPosition;
        node[31].localPosition = node[21].localPosition + node[19].localPosition - node[0].localPosition;

        node[32].localPosition = node[21].localPosition + node[2].localPosition - node[0].localPosition;
        node[33].localPosition = node[21].localPosition + node[6].localPosition - node[0].localPosition;
        node[34].localPosition = node[21].localPosition + node[10].localPosition - node[0].localPosition;
        node[35].localPosition = node[21].localPosition + node[14].localPosition - node[0].localPosition;
        node[36].localPosition = node[21].localPosition + node[18].localPosition - node[0].localPosition;
        */
        //node[37].localPosition = node[21].localPosition + node[1].localPosition - node[0].localPosition;
        //node[38].localPosition = node[21].localPosition + node[5].localPosition - node[0].localPosition;
        //node[39].localPosition = node[21].localPosition + node[9].localPosition - node[0].localPosition;
        //node[40].localPosition = node[21].localPosition + node[13].localPosition - node[0].localPosition;
        //node[41].localPosition = node[21].localPosition + node[19].localPosition - node[0].localPosition;
    }
}
