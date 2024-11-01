// Copyright 2019 The MediaPipe Authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

package com.google.mediapipe.apps.handtrackinggpu;

import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.content.pm.PackageManager.NameNotFoundException;
import android.os.Bundle;
import android.util.Log;
import com.google.mediapipe.formats.proto.LandmarkProto.NormalizedLandmark;
import com.google.mediapipe.formats.proto.LandmarkProto.NormalizedLandmarkList;
import com.google.mediapipe.framework.AndroidPacketCreator;
import com.google.mediapipe.framework.Packet;
import com.google.mediapipe.framework.PacketGetter;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.io.OutputStream;
// Network
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.Socket;

import android.annotation.SuppressLint;
import android.os.AsyncTask;
import android.os.Build;


import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.Socket;

/** Main activity of MediaPipe hand tracking app. */
public class MainActivity extends com.google.mediapipe.apps.basic.MainActivity {
  private static final String TAG = "MainActivity";

  private static final String INPUT_NUM_HANDS_SIDE_PACKET_NAME = "num_hands";
  private static final String INPUT_MODEL_COMPLEXITY = "model_complexity";
  private static final String OUTPUT_LANDMARKS_STREAM_NAME = "hand_landmarks";
  // Max number of hands to detect/process.
  private static final int NUM_HANDS = 2;

  // for TCP connection
  private static final String SERVER_IP = "192.168.1.104"; // Change to the server IP address
  private static final int SERVER_PORT = 10001; // Change to the server port

  private Socket socket;
  private BufferedReader reader;
  private OutputStream outputStream;


  // UDP client
  UDP_Client udp_client = new UDP_Client();
  // // TCP client
  TCP_Client tcp_client = new TCP_Client();


  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);

    ApplicationInfo applicationInfo;
    try {
      applicationInfo =
          getPackageManager().getApplicationInfo(getPackageName(), PackageManager.GET_META_DATA);
    } catch (NameNotFoundException e) {
      throw new AssertionError(e);
    }

    AndroidPacketCreator packetCreator = processor.getPacketCreator();
    Map<String, Packet> inputSidePackets = new HashMap<>();
    inputSidePackets.put(INPUT_NUM_HANDS_SIDE_PACKET_NAME, packetCreator.createInt32(NUM_HANDS));
    if (applicationInfo.metaData.containsKey("modelComplexity")) {
      inputSidePackets.put(
          INPUT_MODEL_COMPLEXITY,
          packetCreator.createInt32(applicationInfo.metaData.getInt("modelComplexity")));
    }
    processor.setInputSidePackets(inputSidePackets);

    // Send udp paket
    udp_client.Message = "Tracker Connected\n";
    udp_client.NachrichtSenden();

    // Create TCP socket and connect to server
    new ConnectTask().execute();
 //      try {
 //       tcp_client.connect();
 //       tcp_client.sendData("CV, Hello world!");
 //   } catch (IOException e) {
 //       Log.e(TAG, "Error connecting to TCP server: " + e.getMessage());
 //   }
    // To show verbose logging, run:
    // adb shell setprop log.tag.MainActivity VERBOSE
      processor.addPacketCallback(
          OUTPUT_LANDMARKS_STREAM_NAME,
          (packet) -> {
            Log.v(TAG, "Received multi-hand landmarks packet.");
            List<NormalizedLandmarkList> multiHandLandmarks =
                PacketGetter.getProtoVector(packet, NormalizedLandmarkList.parser());
            Log.v(
                TAG,
                "[TS:"
                    + packet.getTimestamp()
                    + "] "
                    + getMultiHandLandmarksDebugString(multiHandLandmarks));
          });
  }
/*
  private class ConnectTask extends AsyncTask<Void, Void, Void> {
      @Override
      protected Void doInBackground(Void... voids) {
          try {
              tcp_client.connect();
              tcp_client.sendData("CV, Hello world!");
          } catch (IOException e) {
              Log.e(TAG, "Error connecting to TCP server: " + e.getMessage());
          }
          return null;
      }
  }
*/ 
  private class ConnectTask extends AsyncTask<Void, Void, Void> {
    private static final int MAX_RETRIES = 5;
    private static final long RETRY_DELAY_MS = 5000; // 1 second delay between retries

    @Override
    protected Void doInBackground(Void... voids) {
        int retryCount = 0;
        while (retryCount < MAX_RETRIES) {
            try {
                tcp_client.connect();
                tcp_client.sendData("CV, Hello world!");
                // If successful, break out of the loop
                break;
            } catch (IOException e) {
                Log.e(TAG, "Error connecting to TCP server: " + e.getMessage());
                retryCount++;
                try {
                    Thread.sleep(RETRY_DELAY_MS);
                } catch (InterruptedException interruptedException) {
                    Log.e(TAG, "Error sleeping between connection retries: " + interruptedException.getMessage());
                }
            }
        }
        return null;
    }
}
  
  private String getMultiHandLandmarksDebugString(List<NormalizedLandmarkList> multiHandLandmarks) {
    if (multiHandLandmarks.isEmpty()) {
      return "No hand landmarks";
    }
    String multiHandLandmarksStr = "Number of hands detected: " + multiHandLandmarks.size() + "\n";
    String logToServer = ""; // log to udp server
    int handIndex = 0;
    for (NormalizedLandmarkList landmarks : multiHandLandmarks) {
      multiHandLandmarksStr +=
          "\t#Hand landmarks for hand[" + handIndex + "]: " + landmarks.getLandmarkCount() + "\n";
      int landmarkIndex = 0;
      for (NormalizedLandmark landmark : landmarks.getLandmarkList()) {
        multiHandLandmarksStr +=
            "\t\tLandmark ["
                + landmarkIndex
                + "]: ("
                + landmark.getX()
                + ", "
                + landmark.getY()
                + ", "
                + landmark.getZ()
                + ")\n";
        Double displayWidth = 720.0, displayHeight = 1280.0;
        Double wRatio =  1920.0 / displayHeight;
        Double hRatio = 1080.0 / displayWidth;
        Double wNew = landmark.getX() * hRatio, hNew = landmark.getY() * wRatio;
        String wTmp = String.format("%.5f", wNew), hTmp = String.format("%.5f", hNew);
        // Log.v(TAG, tmpOut);
        logToServer += "0," + landmarkIndex + "," + wTmp + "," + hTmp + "," + landmark.getZ() + "|";
        ++landmarkIndex;
      }
      ++handIndex;
    }
    // Send output 
    Log.v(TAG, logToServer);
    udp_client.Message = logToServer;
    udp_client.NachrichtSenden();
    return multiHandLandmarksStr;
  }
}
