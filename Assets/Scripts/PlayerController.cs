using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //Start() variables
    private Rigidbody2D rb;
    private Animator    anim;
    private Collider2D  coll;

    //FSM (Finite-state machine)
    private enum  State { idle, running, jumping, falling, hurt }
    private State state = State.idle;

    //inpector variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private float     speed     = 7f;
    [SerializeField] private float     jumpForce = 10f;
    [SerializeField] private int       apples    = 0;
    [SerializeField] private Text      appleText;
    [SerializeField] private float     hurtForce = 5f;


    private void Start()
    {
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    } //private void Start()

    private void Update()
    {
        if(state != State.hurt)
        {
            Movement();
        }
        AnimationState();
        anim.SetInteger("state", (int)state); //sets animations based on enumerator state

    } //private void Update()

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Collectable") 
        {
            Destroy(collision.gameObject);
            apples += 1;
            appleText.text = apples.ToString();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if(state == State.falling)
            {
                enemy.JumpedOn();
                Jump();
            }
            else
            {
                state = State.hurt;
                if(other.gameObject.transform.position.x > transform.position.x)
                {
                    // Enemy is to my right so player is damaged and moved left
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                }
                else
                {
                    // Enemy is to my left so player is damaged and moved right
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                }
            }
        }
    }

    private void Movement()
    {
        float hDirection = Input.GetAxis("Horizontal"); //"Horizontal" is from Project Settings -> Input Manager

        //moving left
        if (hDirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-0.8f, 0.8f);
        }

        //moving right
        else if (hDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(0.8f, 0.8f);
        }

        //jumping
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            Jump();
        }
    } //private void InputManager()

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        state = State.jumping;
    }

    private void AnimationState()
    {
        if(state == State.jumping)
        {
           if(rb.velocity.y < 0.1f)
            {
                state = State.falling;
            }
        }

        else if(state == State.falling)
        {
            if (coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }
        else if(state == State.hurt)
        {
            if(Mathf.Abs(rb.velocity.x) < 0.1f)
            {
                state = State.idle;
            }
        }

        else if(Mathf.Abs(rb.velocity.x) > 2f)
        {
            //moving
            state = State.running;
        }
        else
        {
            state = State.idle;
        }

    } // private void VelocityState()



} //public class PlayerController : MonoBehavior
