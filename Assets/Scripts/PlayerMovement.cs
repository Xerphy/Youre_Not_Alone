using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    private TrailRenderer trail;

    public float runSpeed = 40f;
    private Vector2 respawnPoint;

    PauseMenu pauseScript;

    RespawnPointHolder respawnScript;
    public Text checkpointText;
    string lastCheckpointReached;

    bool jump = false;
    bool dash = false;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    CollectibleTally collectibleScript;
    bool inCollectible = false;
    GameObject collectible = null;

    private Rigidbody2D rb;
    private PlayerInputActions playerInputActions;


    private void Awake()
    {
        //pauseScript = this.gameObject.GetComponent<PauseScript>;//collision.gameObject.GetComponent<RespawnPointHolder>();
        pauseScript = this.gameObject.GetComponent<PauseMenu>();

        lastCheckpointReached = "";
        checkpointText.enabled = false;

        SoundManager.Initialize();  // Maybe move this to a more apropriate place later
        SoundManager.PlaySound(SoundManager.Sound.BackgroundMusic);
        SoundManager.PlaySound(SoundManager.Sound.AmbientNoise);

        respawnPoint = transform.position;

        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.performed += Jump;
        playerInputActions.Player.Dash.performed += Dash;
        playerInputActions.Player.Interact.performed += Interact;
        playerInputActions.Player.Pause.performed += Pause;
    }

    private void FixedUpdate()
    {
        Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();

        controller.Move(inputVector.x * Time.fixedDeltaTime * runSpeed, inputVector.y, jump, dash, inputVector.x);
        dash = false;
        jump = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Better jumping related stuff
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !jump) // originally was !Input.GetButton("Jump")
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)  // true if the button was just hit
        {
            jump = true;
            //animator.SetBool("IsJumping", true);
            //Debug.Log("Jumped");
        }
    }

    public void UponLanding () //jumping animation call
    {
        //animator.SetBool("IsJumping", false);
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed)  // true if the button was just hit
        {
            dash = true;
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (inCollectible)
        {
            collectible.SetActive(false);

            if (collectible.name == "Merchant_Cup")
            {
                collectibleScript = rb.gameObject.GetComponent<CollectibleTally>();
                collectibleScript.setMerchantCup(true);
            }
        }
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (context.performed)  // true if the button was just hit
        {
            pauseScript.Pause(context);
        }
    }

        private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "DeathZone")
        {
            trail.emitting = false;
            transform.position = respawnPoint;
        }
        else if (collision.tag == "Enemy" || collision.tag == "Projectile")
        {
            trail.emitting = false;
            transform.position = respawnPoint;
        }
        else if (collision.tag == "CheckPoint")
        {
            respawnScript = collision.gameObject.GetComponent<RespawnPointHolder>();
            respawnPoint = respawnScript.getPoint();
            
            if (!lastCheckpointReached.Equals(collision.gameObject.name))   // If the last checkpoint reached is not the same as the last one reached
            {
                checkpointText.enabled = true;
                lastCheckpointReached = collision.gameObject.name;  // Set it as the last checkpoint reached
            }

            StartCoroutine(Coroutine());
        }
        else if (collision.tag == "Collectible")
        {
            inCollectible = true;
            collectible = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Collectible")
        {
            inCollectible = false;
        }
    }

    IEnumerator Coroutine()
    {
        yield return new WaitForSeconds(4f);

        checkpointText.enabled = false;
    }
}