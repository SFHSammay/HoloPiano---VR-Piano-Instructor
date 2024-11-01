package com.google.mediapipe.apps.handtrackinggpu;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.Socket;

public class TCP_Client {
    private static final String SERVER_IP = "192.168.1.104"; // Change to the server IP address
    private static final int SERVER_PORT = 10001; // Change to the server port

    private Socket socket;
    private BufferedReader reader;
    private OutputStream outputStream;

    public void connect() throws IOException{
        try {
            socket = new Socket(SERVER_IP, SERVER_PORT);
            reader = new BufferedReader(new InputStreamReader(socket.getInputStream()));
            outputStream = socket.getOutputStream();
        } catch (IOException e) {
            e.printStackTrace();
            throw e;
        }
    }

    public void sendData(String message) throws IOException{
        try {
            byte[] data = message.getBytes("UTF-8");
            outputStream.write(data);
        } catch (IOException e) {
            e.printStackTrace();
            throw e;
        }
    }

    public void disconnect() {
        try {
            if (outputStream != null) {
                outputStream.close();
            }
            if (reader != null) {
                reader.close();
            }
            if (socket != null) {
                socket.close();
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}
