using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNote : MonoBehaviour
{
    public AudioSource dNote;

    // Start is called before the first frame update
    private void Update()
    {
        // Check if the 'C' key is pressed
        if (Input.GetKeyDown(KeyCode.D))
        {
            // Play the E note sound
            dNote.Play();
        }
    }
}
