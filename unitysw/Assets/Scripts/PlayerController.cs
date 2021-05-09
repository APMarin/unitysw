using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //Start() Variables
    private Rigidbody2D rb;
    private Animator pAnim;
    private Collider2D pColl;
    
    //FSM
    private enum animState { idle, running, jumping, falling, hurt, idleshoot, runningshoot}
    private animState state = animState.idle;

    //Inspector Variables

    private float timer = 300;
    private float hDirection;
    private bool isPaused = false;
    [SerializeField] private Text timerText;
    [SerializeField] private Text collectableText;
    [SerializeField] private GameObject pauseText;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private int pCollectable = 0;
    [SerializeField] private int jumpForce = 20;
    [SerializeField] private float stuntForce = 10f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pAnim = GetComponent<Animator>();
        pColl = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        hDirection = Input.GetAxisRaw("Horizontal");

        Jump();
        animationState();
        Constrains(); //constrains the movement to especific limits
        pAnim.SetInteger("state", (int)state);
        FallToDeath();
        GameTimer();
        PKeyToPause();

        //debug
        //Debug.Log(rb.velocity.y);
    }

    //movement has to be in FixedUpdate bc its using Unity's physics engine
    private void FixedUpdate(){
        Movement();
    }

    private void GameTimer(){
        if (timer >= 0)
        {
            timer -= Time.deltaTime;
            timerText.text = "Time: " + (int)timer;
        }
        else
        {

            timerText.text = "Time: 0";
            Death();
            timer = 300;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Collectable")
        {
            Destroy(collision.gameObject);
            pCollectable += 1;
            collectableText.text = pCollectable.ToString();
        }
    }

    private void animationState()
    {
        if (state == animState.jumping)
        {
            if (rb.velocity.y < .1f)
            {
                state = animState.falling;
            }
        }
        else if (state == animState.falling)
        {
            if (pColl.IsTouchingLayers(ground))
            {
                state = animState.idle;
            }
        }
        else if (Mathf.Abs(rb.velocity.x) > .1f)
        {
            //If velocity is greater than .1 in X axis is going to Right.
            state = animState.running;
        }
        else
        {
            state = animState.idle;
        }
    }

    private void Movement()
    {
        rb.velocity = new Vector2(hDirection * walkSpeed * Time.fixedDeltaTime, rb.velocity.y);

        if (hDirection < 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }
        else if (hDirection > 0)
        {
            transform.localScale = new Vector2(1, 1);
        }        
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && pColl.IsTouchingLayers(ground))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            state = animState.jumping;
        }
    }

    private void FallToDeath()
    {
        if(transform.position.y <= -7)
            Death();
    }

    private void Death()
    {
        transform.position = new Vector3(-21f,1.52f,transform.position.z);
    }

    private void Constrains()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -21.7f, 135f), Mathf.Clamp(transform.position.y, -8f, 100f), transform.position.z);
    }

    private void PKeyToPause()
    {
        if (Input.GetButtonDown("Pause"))
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        if(isPaused){
            Time.timeScale = 1;
            isPaused = false;
            pauseText.SetActive(false);
            pausePanel.SetActive(false);
        } else {
            Time.timeScale = 0;
            isPaused = true;
            pauseText.SetActive(true);
            pausePanel.SetActive(true);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) //Colision con enemigos
    {
        if(other.gameObject.tag == "Enemy")
        {
            FlyingBot fbot = other.gameObject.GetComponent<FlyingBot>();
            if(state == animState.falling)
            {
                fbot.JumpedOn();
                //si cae sobre el, explota el enemigo
                rb.velocity = new Vector2(rb.velocity.x, jumpForce/2);
                state = animState.jumping;
            }
            else{
                transform.position = new Vector3(-21f,1.52f,transform.position.z);
                //cualquier otra colision, causa respawn
            }
        }
    }
}
