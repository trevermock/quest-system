using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 velocity = Vector2.zero;
    private Animator animator;
    private SpriteRenderer visual;

    private bool movementDisabled = false;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        visual = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start() 
    {
        GameEventsManager.instance.inputEvents.onMovePressed += MovePressed;
        GameEventsManager.instance.playerEvents.onDisablePlayerMovement += DisablePlayerMovement;
        GameEventsManager.instance.playerEvents.onEnablePlayerMovement += EnablePlayerMovement;
    }

    private void OnDestroy()
    {
        GameEventsManager.instance.inputEvents.onMovePressed -= MovePressed;
        GameEventsManager.instance.playerEvents.onDisablePlayerMovement -= DisablePlayerMovement;
        GameEventsManager.instance.playerEvents.onEnablePlayerMovement -= EnablePlayerMovement;
    }

    private void DisablePlayerMovement() 
    {
        movementDisabled = true;
        // also ensure we stop any current movement
        velocity = Vector2.zero;
    }

    private void EnablePlayerMovement() 
    {
        movementDisabled = false;
    }

    private void MovePressed(Vector2 moveDir) 
    {
        velocity = moveDir.normalized * moveSpeed;

        if (movementDisabled) 
        {
            velocity = Vector2.zero;
        }
    }

    private void Update() 
    {
        UpdateAnimations();
    }

    private void FixedUpdate() 
    {
        rb.velocity = velocity;
    }

    private void UpdateAnimations() 
    {
        // updat the animator parameters
        bool walking = (velocity.magnitude > 0.01f);
        animator.SetBool("walking", walking);
        animator.SetFloat("velocity_x", velocity.x);
        animator.SetFloat("velocity_y", velocity.y);
        // facing dir for idle animations
        if (walking) 
        {
            int facingX = 0;
            int facingY = 0;
            if (velocity.x != 0)
            {
                facingX = (int) Mathf.Clamp(velocity.normalized.x, -1, 1);
            }
            else if (velocity.y != 0)
            {
                facingY = (int) Mathf.Clamp(velocity.normalized.y, -1, 1);   
            }
            animator.SetInteger("facing_x", facingX);
            animator.SetInteger("facing_y", facingY);
        }
        // flip the sprite appropriately
        if (velocity.x < 0) 
        {
            visual.flipX = true;
        }
        else if (velocity.x > 0) 
        {
            visual.flipX = false;
        }
        
    }
}
