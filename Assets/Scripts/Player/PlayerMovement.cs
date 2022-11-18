using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

public class PlayerMovement : MonoBehaviour
{
    public float speed;

    public int numberOfJumps = 0;
    public int maxNumberOfJumps = 1;
    public float jumpForce;

    public float dashSpeed;
    bool canDash = true;
    public float currentDashTime;
    public float startDashTime;

    private bool crRunning;

    Vector2 movement = Vector2.zero;

    Rigidbody2D rb = null;
    SpriteRenderer sp = null;
    Animator animator = null;

    private enum Movementstate { idle, run, jump, fall, dash}


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

    }

    void Update()
    {
        rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);
        UpdateAnimationState();

    }

    IEnumerator DashCoroutine(Vector2 direction)
    {
        crRunning = true;
        canDash = false;
        currentDashTime = startDashTime;
        while (currentDashTime > 0f)
        {
            currentDashTime -= Time.deltaTime; 

            rb.velocity = direction * dashSpeed; 
                                                
            yield return null; 
        }

        rb.velocity = new Vector2(0f, 0f); // Stop dashing.

        canDash = true;
        crRunning = false;

    }


    public void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D contact = collision.GetContact(0);
        if (contact.normal.y > 0.7f)
        {
            numberOfJumps = maxNumberOfJumps;
        }
    }
    public void OnJump(InputValue val)
    {
        if (numberOfJumps > 0)
        {
            float innerValue = val.Get<float>();
            if (innerValue > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                numberOfJumps--;
            }
        }
    }

    public void OnMove(InputValue val)
    {
        movement = val.Get<Vector2>();
    }

    public void OnDash(InputValue val)
    {
        if (canDash && val.isPressed)
        {
            if (movement.x > 0)
            {
                StartCoroutine(DashCoroutine(Vector2.right));
            }
            else if (movement.x < 0)
            {
                StartCoroutine(DashCoroutine(Vector2.left));
            }
        }
    }

    private void UpdateAnimationState()
    {
        Movementstate State;

        if (movement.x > 0)
        {
            State = Movementstate.run;
            sp.flipX = false;
        }

        else if (movement.x < 0)
        {
            State = Movementstate.run;
            sp.flipX = true;
        }

        else
        {
            State = Movementstate.idle;
        }

        if (rb.velocity.y > .1f)
        {
            State = Movementstate.jump;
        }

        else if (rb.velocity.y < -.1f)
        {
            State = Movementstate.fall;
        }

        if (crRunning && movement.x < 0)
        {
            State = Movementstate.dash;
            sp.flipX = true;
        }

        else if (crRunning && movement.x > 0)
        {
            State = Movementstate.dash;
            sp.flipX = false;
        }

        animator.SetInteger("State", (int)State);
    }

}
