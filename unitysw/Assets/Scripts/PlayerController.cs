using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private LayerMask ground;
    [SerializeField] private float hDirection;
    [SerializeField] private float walkSpeed = 5f;
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
        pAnim.SetInteger("state", (int)state);

    }

    private void FixedUpdate(){
        Movement();
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
        else if (Mathf.Abs(rb.velocity.x) > 2f)
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
            //rb.velocity = new Vector2(-walkSpeed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
        }
        else if (hDirection > 0)
        {
            //rb.velocity = new Vector2(walkSpeed, rb.velocity.y);
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
}
