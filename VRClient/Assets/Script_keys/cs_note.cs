using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSNote : MonoBehaviour
{
    public AudioSource csNote;

    // Start is called before the first frame update
    private void Update()
    {
        // Check if the 'C' key is pressed
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Play the E note sound
            csNote.Play();
        }
    }
}
