using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Press_a : MonoBehaviour
{
    public Transform Cube_c;
    public float pressheight = -0.15f;
    public float rotationSpeed = 45.0f;
    public AudioSource cNote;

    bool pressing = false;
    private Renderer cubeRenderer;
    private Color originalColor;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Renderer component of the cube
        cubeRenderer = Cube_c.GetComponent<Renderer>();

        // Store the original color
        originalColor = cubeRenderer.material.color;
    }
    // Update is called once per frame
    void Update()
    {
        // Check if the 'S' key is pressed
        if (Input.GetKey(KeyCode.J))
        {
            // Play the sound
            OnKeyPress();
        }
        else
            OnKeyRelease();
    }

    void OnKeyPress()
    {
        if (!pressing)
        {
            cNote.Play(); 
            pressing = true;
            cubeRenderer.material.color = Color.yellow;
            transform.Rotate(Vector3.left, 5.0f);
        }
    }

    void OnKeyRelease()
    {
        if (pressing)
        {
            cNote.Stop();
            pressing = false;
            cubeRenderer.material.color = originalColor;
            transform.rotation = Quaternion.identity;
        }
    }
}
