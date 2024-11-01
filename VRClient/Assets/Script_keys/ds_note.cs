using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DSNote : MonoBehaviour
{
    public AudioSource dsNote;

    // Start is called before the first frame update
    private void Update()
    {
        // Check if the 'C' key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Play the E note sound
            dsNote.Play();
        }
    }
}
