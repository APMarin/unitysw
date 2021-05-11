using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    private Scene c_Scene;
    private string sceneName;
    // Start is called before the first frame update
    public Transform player;
    // Update is called once per frame
    void Start()
    {
        c_Scene = SceneManager.GetActiveScene();
        sceneName = c_Scene.name;
    }
    void Update()
    {
        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        Constrains();
    }

    private void Constrains()
    {
        if (sceneName == "Level1")
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -13.10f, 127.10f), Mathf.Clamp(transform.position.y, -1f, 22f), transform.position.z);
        }
        if (sceneName == "Level2")
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -16.10f, 32f), Mathf.Clamp(transform.position.y, -1f, 28f), transform.position.z);
        }
        
    }
}
