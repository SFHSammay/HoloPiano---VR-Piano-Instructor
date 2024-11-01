using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handtracking : MonoBehaviour
{
    public socket socketScript;
    public GameObject[] handPoints;
    public LineRenderer[] lineRenderers;

    // Define finger order
    private int[][] fingerOrder = new int[][]
    {
        new int[] {0, 1, 2, 3, 4},       // Wrist to Thumb
        new int[] {0, 5, 6, 7, 8},       // Wrist to Index Finger
        new int[] {5, 9, 10, 11, 12},    // Index Finger to Middle Finger
        new int[] {9, 13, 14, 15, 16},   // Middle Finger to Ring Finger
        new int[] {13, 17},              // Ring Finger to Pinky
        new int[] {0, 17, 18, 19, 20}    // Wrist to Pinky
    };


    // Start is called before the first frame update
    void Start()
    {
        // Ensure that the socketScript field is set in the Unity Inspector
        if (socketScript == null)
        {
            Debug.LogError("Socket Script reference is not set. Please assign the socket script in the Unity Inspector.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Access the hand nodes from the socketScript and update handPoints positions
        if (socketScript != null)
        {
            if (handPoints.Length >= 21)  // Ensure you have the expected number of hand points
            {
                float[] x = new float[21];
                float[] y = new float[21];
                float[] z = new float[21];

                for (int i = 0; i < 21; i++)
                {
                    x[i] = socketScript.node[i].localPosition.x;
                    y[i] = socketScript.node[i].localPosition.y;
                    z[i] = socketScript.node[i].localPosition.z;

                    // Apply the positions to the hand points
                    handPoints[i].transform.localPosition = new Vector3(x[i], y[i], z[i]);

                    Debug.Log($"Point {i + 1} - X: {x[i]}, Y: {y[i]}, Z: {z[i]}");
                }

                // Update the LineRenderers based on the hand points positions
                UpdateLineRenderers(x, y, z);

            }
            else
            {
                Debug.LogError("Expected 21 hand points, but found " + handPoints.Length + " points in the array.");
            }
        }
    }

    // Method to update LineRenderers based on hand points positions
    void UpdateLineRenderers(float[] x, float[] y, float[] z)
    {
        // Ensure that the LineRenderer components are not null
        if (lineRenderers != null && lineRenderers.Length > 0)
        {
            for (int i = 0; i < lineRenderers.Length; i++)
            {
                // Set the number of positions in each LineRenderer
                lineRenderers[i].positionCount = fingerOrder[i].Length;

                // Set the positions of each LineRenderer based on the hand points
                for (int j = 0; j < fingerOrder[i].Length; j++)
                {
                    int pointIndex = fingerOrder[i][j];

                    // Check if the pointIndex is within bounds
                    if (pointIndex >= 0 && pointIndex < handPoints.Length)
                    {
                        lineRenderers[i].SetPosition(j, new Vector3(x[pointIndex], y[pointIndex], z[pointIndex]));
                    }
                    else
                    {
                        Debug.LogError($"Invalid point index {pointIndex} for finger {i}. Check the fingerOrder array and handPoints array.");
                    }
                }
            }
        }
        else
        {
            Debug.LogError("LineRenderer array is null or empty. Please assign LineRenderer components in the Unity Inspector.");
        }
    }
}

