using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ENote : MonoBehaviour
{
    public AudioSource eNote;

    // Start is called before the first frame update
    private void Update()
    {
        // Check if the 'C' key is pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Play the E note sound
            eNote.Play();
        }
    }
}
