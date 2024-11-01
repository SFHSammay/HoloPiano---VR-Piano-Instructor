using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CNote : MonoBehaviour
{
    public AudioSource cNote;

    // Start is called before the first frame update
    private void Update()
    {
        // Check if the 'C' key is pressed
        if (Input.GetKeyDown(KeyCode.S))
        {
            // Play the E note sound
            cNote.Play();
        }
    }
}
