using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parallax is an effect to create the illusion of depth, on 2D games it's used to represent the movement relative
// to the camera the player is viewing, when the player moves, so does the background behind. Causing a natural effect
// of movement whenever the player moves between X and Y axis.

public class Parallax : MonoBehaviour
{
    private float length, startpos;
    public GameObject cam;
    public float parallaxEffect;

    void Start()
    {
        // We will get the start position of the sprite relative to the camera.
        startpos = transform.position.x;
        // We will get the length of the sprite used for the parallax effect with SpriteRenderer
        // to later check the whole length of the image on the X axis.
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);

        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }
}
