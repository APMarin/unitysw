using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingBot : MonoBehaviour
{
    // Start is called before the first frame update   
    [SerializeField] private float leftCap;
    [SerializeField] private float rightCap;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float moveAmount = 5f;
    private bool facingLeft = true;
    private Collider2D coll;
    private Rigidbody2D rb;
    private Animator anim;
    
    private void Start()
    {
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void Update() 
    {
        Move();
    }
    private void Move(){

        if(facingLeft){
            if(transform.position.x > leftCap){
                if(transform.localScale.x != 1)
                {
                    transform.localScale = new Vector3(1,1);
                }
                rb.velocity = new Vector2(-moveAmount,0);
            }
            else{
                facingLeft = false;
            }
        }
        else
        {
            if(transform.position.x < rightCap){
                if(transform.localScale.x != -1)
                {
                    transform.localScale = new Vector3(-1,1);
                }
                rb.velocity = new Vector2(moveAmount,0);
            }
            else{
                facingLeft = true;
            }
        }
    }
    public void JumpedOn()
    {
        anim.SetTrigger("Explode");
    }

    private void Death()
    {
        Destroy(this.gameObject);
    }
}
