package com.google.mediapipe.apps.handtrackinggpu;

// Network
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;

import android.annotation.SuppressLint;
import android.os.AsyncTask;
import android.os.Build;


public class UDP_Client 
{

    private InetAddress IPAddress = null;
    private String message = "Hello Android!" ;
    private String ServerHost = "192.168.1.104";
    private AsyncTask<Void, Void, Void> async_cient;
    public String Message;


    @SuppressLint("NewApi")
    public void NachrichtSenden()
    {
        async_cient = new AsyncTask<Void, Void, Void>() 
        {
            @Override
            protected Void doInBackground(Void... params)
            {   
                DatagramSocket ds = null;

                try 
                {
                    InetAddress addr = InetAddress.getByName(ServerHost);
                    ds = new DatagramSocket(10010);
                    DatagramPacket dp;                          
                    dp = new DatagramPacket(Message.getBytes(), Message.getBytes().length, addr, 10010);
                    ds.setBroadcast(true);
                    ds.send(dp);
                } 
                catch (Exception e) 
                {
                    e.printStackTrace();
                }
                finally 
                {
                    if (ds != null) 
                    {   
                        ds.close();
                    }
                }
                return null;
            }

            protected void onPostExecute(Void result) 
            {
               super.onPostExecute(result);
            }
        };

        if (Build.VERSION.SDK_INT >= 11) async_cient.executeOnExecutor(AsyncTask.THREAD_POOL_EXECUTOR);
        else async_cient.execute();
    }
}
