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
    private enum animState {idle, running, jumping, falling}
    private animState state = animState.idle;

    //Inspector Variables
    private float timer = 300;
    private float hDirection;
    private bool isPaused = false;
    public int maxHealth = 50;
    public int currentHealth;
    public HealthBar hpBar;
    [SerializeField] private Text timerText;
    [SerializeField] private Text collectableText;
    [SerializeField] private GameObject pauseText;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private int pCollectable = 0;
    [SerializeField] private int jumpForce = 20;
    [SerializeField] private float stuntForce = 20f;
    private Scene c_Scene;
    private string sceneName;


    // Start is called before the first frame update
    void Start()
    {
        //Unity will get the current loaded Scene to identify in which level the player is on.
        c_Scene = SceneManager.GetActiveScene();
        sceneName = c_Scene.name;

        rb = GetComponent<Rigidbody2D>();
        pAnim = GetComponent<Animator>();
        pColl = GetComponent<Collider2D>();
        currentHealth = maxHealth;
        hpBar.setMaxHealth(maxHealth);
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

    //movement has to be in FixedUpdate because it's using Unity's physics engine
    private void FixedUpdate(){
        Movement();
    }

    // Timer set, when the timer reaches 0 it will trigger player's death.
    private void GameTimer(){
        if (timer >= 0)
        {
            timer -= Time.deltaTime;
            timerText.text = "Time: " + (int)timer;
        }
        else
        {

            timerText.text = "Time: 0";
            resetScene();
            timer = 300;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // When player enters the triggerable box collider of a collectable tagged entity
        // it will have the collectable object removed from the game and player will get 250 pts added
        // to their score pool.
        if(collision.tag == "Collectable")
        {
            Destroy(collision.gameObject);
            pCollectable += 250;
            collectableText.text = pCollectable.ToString();
        }
        // When player collides with the object tagged as "Meetpoint", the game will review which scene is loaded
        // using the scene name, then load the next scene on the hierarchy.
        if(collision.tag == "Meetpoint")
        {
            if(sceneName == "Level1")
            {
                SceneManager.LoadScene("Level2");
            }
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
        if (Input.GetButtonDown("Jump") && IsGrounded()) 
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            state = animState.jumping;
        }
        else
        {
            if(!IsGrounded())
            {
                state = animState.jumping;
            }
        }
    }

    private bool IsGrounded()
    {
        float extraHeightText = .5f;
        RaycastHit2D raycastHit = Physics2D.Raycast(pColl.bounds.center, Vector2.down, pColl.bounds.extents.y + extraHeightText, ground);
        Color rayColor;
        if (raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(pColl.bounds.center, Vector2.down * (pColl.bounds.extents.y + extraHeightText));
        Debug.Log(raycastHit.collider);
        return raycastHit.collider != null;


    }

    private void FallToDeath()
    {
        if (sceneName == "Level1")
        {
            if (transform.position.y <= -7)
                death(-21f, 1.52f);
        }
        if (sceneName == "Level2")
        {
            if (transform.position.y <= -7)
                death(-20f, 12f);
        }
    }

    private void death(float x, float y)
    {
        transform.position = new Vector3(x, y, transform.position.z);
    }

    private void resetScene()
    {
        // Unity will get the name of the current scene and re-load it after the function is called.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Constrains()
    {
        //Constraints will force player character to not leave the playable area.
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

    private void OnCollisionEnter2D(Collision2D other) //Collision with enemies
    {
        if(other.gameObject.tag == "Enemy")
        {
            FlyingBot fbot = other.gameObject.GetComponent<FlyingBot>();
            if(state == animState.falling)
            {
                fbot.JumpedOn();
                //When player jumps on enemies, they will explode and remove them from the game.
                pCollectable += 50;
                collectableText.text = pCollectable.ToString();
                rb.velocity = new Vector2(rb.velocity.x, jumpForce/2);
                state = animState.jumping;
            }
            else{
                //Player will get 25 points substracted from their health pool after collisioning with an enemy
                takeDmg(25);
                //Checks if player has 0 health points, if currentHealth is equal or lower than 0 then triggerDeath will get called.
                triggerDeath();
                if (other.gameObject.transform.position.x > transform.position.x)
                {
                    //If player is next to the enemy by right side, then player moves to left.
                    rb.velocity = new Vector2(-stuntForce, rb.velocity.y);
                }
                else
                {
                    //If player is next to the enemy by left side, then player moves to right.
                    rb.velocity = new Vector2(stuntForce, rb.velocity.y);
                }
            }
        }
        if(other.gameObject.tag == "Hazard")
        {
            {
                //Player will get 50 points substracted from their health pool. Causing an immediate death.
                takeDmg(50);
                triggerDeath();
            }
        }
    }

    void triggerDeath()
    {
        if (currentHealth <= 0)
        {
            pAnim.SetTrigger("Explode");
        }
    }
    void takeDmg(int damage)
    {
        currentHealth -= damage;
        hpBar.setHealth(currentHealth);
    }
}
