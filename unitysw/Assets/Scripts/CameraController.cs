using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform player;
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        Constrains();
    }

    private void Constrains()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -13.10f, 127.10f), Mathf.Clamp(transform.position.y, -1f, 22f), transform.position.z);
    }
}
