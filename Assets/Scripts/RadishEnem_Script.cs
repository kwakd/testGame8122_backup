using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadishEnem_Script : Enemy
{
    [SerializeField] private float leftCap;
    [SerializeField] private float rightCap;

    [SerializeField] private float jumpLength = 10f;
    [SerializeField] private float jumpHeight = 15f;
    [SerializeField] private LayerMask ground;

    private Collider2D coll;

    private bool facingLeft = true;

    protected override void Start()
    { 
        base.Start(); //base is whatever it inherits from
        coll = GetComponent<Collider2D>();
    }

    private void Update()
    { 
        //transition from jump to fall
        if(anim.GetBool("Jumping"))
        {
            if(rb.velocity.y < 0.1)
            {
                anim.SetBool("Falling", true);
                anim.SetBool("Jumping", false);
            }
        }

        //transition from fall to idle
        if(coll.IsTouchingLayers(ground) && anim.GetBool("Falling"))
        {
            anim.SetBool("Falling", false);
        }

    }

    private void RadishMove()
    {
        if(facingLeft)
        {
            // Test to see if we are beyond the leftCap
            if(transform.position.x > leftCap)
            {
                //Make sure sprite is facing right location and if it isn't faces the right direction
                if(transform.localScale.x != 1)
                {
                    transform.localScale = new Vector3(0.8f, 0.8f);
                }

                //Test to see if I am on the ground, if so jump
                if(coll.IsTouchingLayers(ground))
                {
                    //jump
                    rb.velocity = new Vector2(-jumpLength, jumpHeight);
                    anim.SetBool("Jumping", true);
                }
            }
            else
            {
                facingLeft = false;
            }
        }
        else
        {
            // Test to see if we are beyond the leftCap
            if (transform.position.x < rightCap)
            {
                //Make sure sprite is facing right location and if it isn't faces the right direction
                if (transform.localScale.x != -1)
                {
                    transform.localScale = new Vector3(-0.8f, 0.8f);
                }

                //Test to see if I am on the ground, if so jump
                if (coll.IsTouchingLayers(ground))
                {
                    //jump
                    rb.velocity = new Vector2(jumpLength, jumpHeight);
                    anim.SetBool("Jumping", true);
                }
            }
            else
            {
                facingLeft = true;
            }
        }
    }

}