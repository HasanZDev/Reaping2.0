using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform player;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform dashPoint;

    [Header("Running Variables")]
    [SerializeField] float speed;
    float horizontalMove;
    bool facingRight = true;

    [Header("Jumping Variables")]
    [SerializeField] float groundOffset;
    [SerializeField] float groundRadius;
    [SerializeField] float jumpForce;
    bool doubleJump;
    bool isGrounded;

    [Header("Dashing Variables")]
    [SerializeField] float dashDistance;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashRayYOffset;

   // Update is called once per frame
    void Update()
    {
        //flipping
        //gets the horizontal left or right on the X axis
        float moveX = Input.GetAxisRaw("Horizontal");
         //Flip character if horizontal is moving right and not already facing right
        if(moveX > 0 && !facingRight)
        {
            Flip();
        } //Flip character if horizontal is moving  and not already facing left
        else if (moveX < 0 && facingRight)
        {
            Flip();
        }

        //jumping
        //create overlap circle for ground check
        isGrounded = Physics2D.OverlapCircle(new Vector2(player.position.x, player.position.y - groundOffset), groundRadius, groundLayer);
        //when on the ground the jump counter is equal to zero
        if (isGrounded && !Input.GetButton("Jump"))
        {
            doubleJump = false;
        }
        //if jump is pressed
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded || doubleJump)
            {//call jump function
                Jump();
            }
        }

        //dashing
        //if Q is pressed
        if (Input.GetKeyDown("q"))
        {//call dash function
            Dash();
        }
        //raycast Debug
        if (facingRight)
        {
            Debug.DrawRay(player.position, Vector2.right * dashDistance, Color.red);
            Debug.DrawRay(new Vector2(player.position.x, player.position.y + dashRayYOffset), Vector2.right * dashDistance, Color.red);
            Debug.DrawRay(new Vector2(player.position.x, player.position.y - dashRayYOffset), Vector2.right * dashDistance, Color.red);
        } else if (!facingRight)
        {
            Debug.DrawRay(player.position, Vector2.left * dashDistance, Color.red);
            Debug.DrawRay(new Vector2(player.position.x, player.position.y + dashRayYOffset), Vector2.left * dashDistance, Color.red);
            Debug.DrawRay(new Vector2(player.position.x, player.position.y - dashRayYOffset), Vector2.left * dashDistance, Color.red);
        }
    }
    void FixedUpdate()
    {
        //call movement function
        Move();
    }
    void Jump()
    {
        //change velocity to jumping force
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        //changes doublejump into its opposite
        doubleJump = !doubleJump;
    }
    void Move()
    {   
        //movement speed is equal to the horizontal axis and the speed
        horizontalMove = Input.GetAxis("Horizontal") * speed;
        //velocity is equal to the movement speed and the current y velocity
        rb.velocity = new Vector2(horizontalMove, rb.velocity.y);
    }

    void Dash()
    {
        //Spawn multiple rays to check if you can dash
        if (facingRight)
        {
            RaycastHit2D dRay = Physics2D.Raycast(player.position, Vector2.right, dashDistance, groundLayer, -Mathf.Infinity, Mathf.Infinity);
            RaycastHit2D dRay1 = Physics2D.Raycast(new Vector2(player.position.x, player.position.y + dashRayYOffset), Vector2.right, dashDistance, groundLayer, -Mathf.Infinity, Mathf.Infinity);
            RaycastHit2D dRay2 = Physics2D.Raycast(new Vector2(player.position.x, player.position.y - dashRayYOffset), Vector2.right, dashDistance, groundLayer, -Mathf.Infinity, Mathf.Infinity);
        } else if (!facingRight)
        {
            RaycastHit2D dRay = Physics2D.Raycast(player.position, Vector2.left, dashDistance, groundLayer, -Mathf.Infinity, Mathf.Infinity);
            RaycastHit2D dRay1 = Physics2D.Raycast(new Vector2(player.position.x, player.position.y + dashRayYOffset), Vector2.left, dashDistance, groundLayer, -Mathf.Infinity, Mathf.Infinity);
            RaycastHit2D dRay2 = Physics2D.Raycast(new Vector2(player.position.x, player.position.y - dashRayYOffset), Vector2.left, dashDistance, groundLayer, -Mathf.Infinity, Mathf.Infinity);
        }
    }

    //Gizmo for ground check
    void OnDrawGizmos()
    {
        //change gizmo color
        Gizmos.color = Color.red;
        //draw circle gizmo
        Gizmos.DrawWireSphere(new Vector2(player.position.x, player.position.y - groundOffset), groundRadius);
    }

    void Flip()
    {
        //invert the facingRight bool
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        //flip the sprite on X axis
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
