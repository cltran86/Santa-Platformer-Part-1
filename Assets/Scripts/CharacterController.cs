using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private Rigidbody2D _rb;

    [Header("Movement")]
    [SerializeField]
    private float maxSpeed = 5;
    [SerializeField]
    private float acceleration = 0.5f;
    private float horizontalTilt;
    private float currentSpeed;

    [Header("Jumping")]
    [SerializeField]
    private float jumpHeight = 3;

    [Header("Ground Detection")]
    [SerializeField]
    private float groundDistance;
    [SerializeField]
    private Vector3 colliderOffset;
    [SerializeField]
    private LayerMask groundLayer;
    private bool onGround = false;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalTilt = Input.GetAxisRaw("Horizontal");

        // input for various actions
        if (Input.GetKeyDown(KeyCode.Space)) StartJump();
        if (Input.GetKeyUp(KeyCode.Space)) CancelJump();
    }

    private void FixedUpdate()
    {
        UpdateMovement();
        CheckGround();
    }

    private void UpdateMovement()
    {
        // if the horizontal tilt is not zero, add to the speed
        if (horizontalTilt != 0)
        {
            // calculate the current speed based on tilt and acceleration
            // clamp the speed so that the speed never exceeds the maximum
            currentSpeed = Mathf.Clamp(currentSpeed + horizontalTilt * acceleration, -maxSpeed, maxSpeed);
        }
        // if the horizontal tilt is zero, slowly decrease to zero
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, acceleration);
        }

        // apply that speed the the rigid body as a velocity
        _rb.velocity = new Vector2(currentSpeed, _rb.velocity.y);
    }

    private void CheckGround()
    {
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector3.down, groundDistance, groundLayer)
            || Physics2D.Raycast(transform.position - colliderOffset, Vector3.down, groundDistance, groundLayer);
    }

    private void StartJump()
    {
        if (!onGround) return;

        // calculate the force required
        float jumpForce = Mathf.Sqrt(jumpHeight * -2 * Physics2D.gravity.y * _rb.gravityScale) * _rb.mass;

        // apply the force onto the rigid body
        _rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    private void CancelJump()
    {
        if (_rb.velocity.y > 0)
        {
            Vector3 v = _rb.velocity;
            v.y = 0;
            _rb.velocity = v;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundDistance);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundDistance);
    }
}
