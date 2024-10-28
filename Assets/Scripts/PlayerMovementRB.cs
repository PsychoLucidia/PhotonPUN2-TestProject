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
    public int speed = 10;

    [Header("Player Settings (Private)")]
    [SerializeField] float _pressNHoldThreshold = 0.6f;

    // Non-Serialized Variables
    float _horizontalInput;
    float _verticalInput;

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
    }

    void FixedUpdate()
    {
        MoveLogic();
        LimitSpeed();
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
    }

    void DetectInputMultiplayer()
    {
        if (photonView.IsMine)
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
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
}
