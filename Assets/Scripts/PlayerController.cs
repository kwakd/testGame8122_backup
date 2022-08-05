using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
    [SerializeField] private LayerMask       ground;

    [SerializeField] private float           speed     = 7f;
    [SerializeField] private float           jumpForce = 25f;
    [SerializeField] private float           hurtForce = 5f;

    [SerializeField] private AudioSource     appleSound;
    [SerializeField] private AudioSource     footstep;





    private void Start()
    {
        rb      = GetComponent<Rigidbody2D>();
        anim    = GetComponent<Animator>();
        coll    = GetComponent<Collider2D>();
        PermanentUI.perm.healthAmount.text = PermanentUI.perm.health.ToString();
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
            appleSound.Play();
            Destroy(collision.gameObject);
            PermanentUI.perm.apples += 1;
            PermanentUI.perm.appleText.text = PermanentUI.perm.apples.ToString();
        }

        if(collision.tag == "Powerup") 
        {
            Destroy(collision.gameObject);
            jumpForce = 35f;
            GetComponent<SpriteRenderer>().color = Color.yellow;
            StartCoroutine(ResetPower());
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
                HandleHealth(); // Deals with health, updating ui, and resets level if health is <= 0
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

    private void HandleHealth()
    {
        PermanentUI.perm.health -= 1;
        PermanentUI.perm.healthAmount.text = PermanentUI.perm.health.ToString();
        if(PermanentUI.perm.health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    }

    private void Footstep()
    {
        footstep.Play();
    }

    private IEnumerator ResetPower()
    {
        yield return new WaitForSeconds(10);
        jumpForce = 25f;
        GetComponent<SpriteRenderer>().color = Color.white;
    }



} //public class PlayerController : MonoBehavior
