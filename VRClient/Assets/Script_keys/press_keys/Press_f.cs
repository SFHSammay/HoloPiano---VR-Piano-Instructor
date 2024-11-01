using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PUSH_f : MonoBehaviour
{
    public Transform Cube_c;
    public float pressheight = -0.0015f;
    public float rotationSpeed = 45.0f;
    public AudioSource cNote;

    bool pressing = false;

    private Renderer cubeRenderer;
    private Color originalColor;

    // Start is called before the first frame update
    void Start()
    {
        cubeRenderer = Cube_c.GetComponent<Renderer>();
        originalColor = cubeRenderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Cube_c local rotation: " + Cube_c.localPosition.y);
        // Check if the 'S' key is pressed
        if (Cube_c.localPosition.y < pressheight)
        {
            // Play the sound
            OnKeyPush();
        }
        else
            OnKeyRelease();
    }

    void OnKeyPush()
    {
        if (!pressing)
        {
            cNote.Play(); 
            pressing = true;
            cubeRenderer.material.color = Color.yellow;
        }
    }

    void OnKeyRelease()
    {
        if (pressing)
        {
            cNote.Stop();
            pressing = false;
            cubeRenderer.material.color = originalColor;
        }
    }
}
