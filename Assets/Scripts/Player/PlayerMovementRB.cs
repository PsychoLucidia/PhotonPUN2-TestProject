using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView), typeof(Rigidbody), typeof(PhotonRigidbodyView))]
public class PlayerMovementRB : MonoBehaviourPunCallbacks
{
    #region Variables

    [Header("Components")]
    public Rigidbody rb;

    [Header("Player Settings")]
    public float speed = 10;
    public float jumpForce = 10f;

    public float jumpCooldown = 1.5f;

    [Header("Player Settings (Private)")]
    [SerializeField] float _pressNHoldThreshold = 0.6f;

    public bool isGrounded = false;

    public LayerMask groundLayer;

    // Non-Serialized Variables
    float _horizontalInput;
    float _verticalInput;
    float _currentJumpCooldown;

    #endregion


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Initializes the Rigidbody component.
    /// </summary>
    void Awake()
    {
        // Get the Rigidbody component attached to this GameObject
        rb = this.gameObject.GetComponent<Rigidbody>();
    }

    void Start()
    {
        rb.isKinematic = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isMultiplayer) { DetectInputMultiplayer(); }
        else if (!GameManager.Instance.isMultiplayer) { DetectInput(); }

        JumpCooldownCountdown();
    }

    void FixedUpdate()
    {
        MoveLogic();
        LimitSpeed();
    }

    void JumpCooldownCountdown()
    {
        if (_currentJumpCooldown > 0)
        {
            _currentJumpCooldown -= Time.deltaTime;
        }
    }

    void MoveLogic()
    {
        Vector3 input = new Vector3(_horizontalInput, 0, _verticalInput);

        rb.AddForce(input * (speed * 12), ForceMode.Force);
    }

    void DetectInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && _currentJumpCooldown <= 0)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _currentJumpCooldown = jumpCooldown;
        }
    }

    void DetectInputMultiplayer()
    {
        if (photonView.IsMine)
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded && _currentJumpCooldown <= 0)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    void LimitSpeed()
    {
        Vector3 velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if (velocity.magnitude > speed)
        {
            Vector3 limitedVelocity = velocity.normalized * speed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    void OnCollisionStay(Collision actor)
    {
        if ((groundLayer.value & (1 << actor.gameObject.layer)) > 0)
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision actor)
    {
        if ((groundLayer.value & (1 << actor.gameObject.layer)) > 0)
        {
            isGrounded = false;
        }
    }
}
