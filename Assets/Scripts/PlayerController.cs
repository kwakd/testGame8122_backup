using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Start() variables
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;

    //FSM (Finite-state machine)
    private enum State { idle, running, jumping, falling }
    private State state = State.idle;

    //inpector variables
    [SerializeField]private LayerMask ground;
    [SerializeField]private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;




    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    } //private void Start()

    private void Update()
    {
        Movement();

        VelocityState();
        anim.SetInteger("state", (int)state); //sets animations based on enumerator state

    } //private void Update()

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
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            state = State.jumping;
        }
    } //private void InputManager()

    private void VelocityState()
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
