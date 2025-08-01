using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform player;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform dashPoint;
    #endregion
    #region Variables
    [Header("Movement Variables")]
    public bool MovementLock;

    [Header("Running Variables")]
    [SerializeField] public float Speed;
    private float m_horizontalMove;
    private bool m_facingRight = true;

    [Header("Jumping Variables")]
    [SerializeField] private float m_groundOffset;
    [SerializeField] private float m_groundRadius;
    [SerializeField] public float JumpForce;
    private bool m_doubleJump;
    private bool m_isGrounded;

    [Header("Dashing Variables")]
    [SerializeField] private float m_dashDistance;
    [SerializeField] private float m_dashSpeed;
    [SerializeField] private float m_dashRayYOffset;
    [SerializeField] private float m_dashRayXOffset;
    [SerializeField] private float m_dashCooldown;
    private bool m_isDashing;
    private bool m_canDash = true;
#endregion
    void Update()
    {
        if (MovementLock == false)
        {
            #region Flipping
            //gets the horizontal left or right on the X axis
            float moveX = Input.GetAxisRaw("Horizontal");
            //Flip character if horizontal is moving right and not already facing right
            if (moveX > 0 && !m_facingRight)
            {
                Flip();
            } //Flip character if horizontal is moving  and not already facing left
            else if (moveX < 0 && m_facingRight)
            {
                Flip();
            }
            #endregion
            #region Jumping
            //create overlap circle for ground check
            m_isGrounded = Physics2D.OverlapCircle(new Vector2(player.position.x, player.position.y - m_groundOffset), m_groundRadius, groundLayer);
            //when on the ground the jump counter is equal to zero
            if (m_isGrounded && !Input.GetButton("Jump"))
            {
                m_doubleJump = false;
            }
            //if jump is pressed
            if (Input.GetButtonDown("Jump"))
            {
                if (m_isGrounded || m_doubleJump)
                {//call jump function
                    Jump();
                }
            }
            #endregion
        }

        #region Dashing
        //if Q is pressed
        if (Input.GetKeyDown("q") && m_canDash == true)
        {//call dashRay function
            Dash();
        }
        //checks if facing right
        if (m_facingRight)
        {
            //check if playerX is equal or greater than than dashpointX and stop dashing
            if (m_isDashing == true && player.position.x >= dashPoint.position.x)
            {
                m_isDashing = false;
            }
        } else if (!m_facingRight)//check if facing left
        {
            //check if playerX is equal or less than dashpointX and stop dashing
            if (m_isDashing == true && player.position.x <= dashPoint.position.x)
            {
                m_isDashing = false;
            }
        }
        //check if your supposed to dash and start dashing
        if (m_isDashing == true)
        {
            StopMovement();
            player.position = Vector2.MoveTowards(player.position, dashPoint.position, m_dashSpeed * Time.deltaTime);
        }
        if(m_isDashing == false)
        {
            ReturnMovement();
            //set the Y velocity to 0 and return gravity;
            rb.gravityScale = 4f;
        }
        #endregion
        #region RaycastDebug
        if (m_facingRight)
        {
            Debug.DrawRay(player.position, Vector2.right * m_dashDistance, Color.red);
            Debug.DrawRay(new Vector2(player.position.x, player.position.y + m_dashRayYOffset), Vector2.right * m_dashDistance, Color.red);
            Debug.DrawRay(new Vector2(player.position.x, player.position.y - m_dashRayYOffset), Vector2.right * m_dashDistance, Color.red);
        } else if (!m_facingRight) //flip if facing left
        {
            Debug.DrawRay(player.position, Vector2.left * m_dashDistance, Color.red);
            Debug.DrawRay(new Vector2(player.position.x, player.position.y + m_dashRayYOffset), Vector2.left * m_dashDistance, Color.red);
            Debug.DrawRay(new Vector2(player.position.x, player.position.y - m_dashRayYOffset), Vector2.left * m_dashDistance, Color.red);
        }
        #endregion

    }
    void FixedUpdate()
    {    
        if (MovementLock == false)
        {
            #region Movement
            //call movement function
            Move();
            #endregion
        }
    }
    #region Jump()
    void Jump()
    {
        //change velocity to jumping force
        rb.velocity = new Vector2(rb.velocity.x, JumpForce);
        //changes doublejump into its opposite
        m_doubleJump = !m_doubleJump;
    }
    #endregion
    #region Move()
    void Move()
    {   
        //movement speed is equal to the horizontal axis and the speed
        m_horizontalMove = Input.GetAxis("Horizontal") * Speed;
        //velocity is equal to the movement speed and the current y velocity
        rb.velocity = new Vector2(m_horizontalMove, rb.velocity.y);
    }
    #endregion\
    #region Dash()
    void Dash()
    {
        m_canDash = false;

        //refrences rays in local function
        RaycastHit2D dRay = new RaycastHit2D();
        RaycastHit2D dRay1 = new RaycastHit2D();
        RaycastHit2D dRay2 = new RaycastHit2D();

        //Spawn multiple rays to check if you can dash
        if (m_facingRight)
        {
            dRay = Physics2D.Raycast(player.position, Vector2.right, m_dashDistance, groundLayer, -Mathf.Infinity, Mathf.Infinity);
            dRay1 = Physics2D.Raycast(new Vector2(player.position.x, player.position.y + m_dashRayYOffset), Vector2.right, m_dashDistance, groundLayer, -Mathf.Infinity, Mathf.Infinity);
            dRay2 = Physics2D.Raycast(new Vector2(player.position.x, player.position.y - m_dashRayYOffset), Vector2.right, m_dashDistance, groundLayer, -Mathf.Infinity, Mathf.Infinity);
        } else if (!m_facingRight) //flip rays if facing left
        {
            dRay = Physics2D.Raycast(player.position, Vector2.left, m_dashDistance, groundLayer, -Mathf.Infinity, Mathf.Infinity);
            dRay1 = Physics2D.Raycast(new Vector2(player.position.x, player.position.y + m_dashRayYOffset), Vector2.left, m_dashDistance, groundLayer, -Mathf.Infinity, Mathf.Infinity);
            dRay2 = Physics2D.Raycast(new Vector2(player.position.x, player.position.y - m_dashRayYOffset), Vector2.left, m_dashDistance, groundLayer, -Mathf.Infinity, Mathf.Infinity);
        }

        if (m_facingRight)
        {
            //checking if each collider hits something and if it does move dashpoint to corresponding location
            if (dRay.collider != null)
            {
                dashPoint.position = new Vector2(dRay.point.x - m_dashRayXOffset, player.position.y);
            }
            else if (dRay1.collider != null)
            {
                dashPoint.position = new Vector2(dRay1.point.x - m_dashRayXOffset, player.position.y);
            }
            else if (dRay2.collider != null)
            {
                dashPoint.position = new Vector2(dRay2.point.x - m_dashRayXOffset, player.position.y);
            }
        } else if (!m_facingRight)
        {
            //checking if each collider hits something and if it does move dashpoint to corresponding location
            if (dRay.collider != null)
            {
                dashPoint.position = new Vector2(dRay.point.x + m_dashRayXOffset, player.position.y);
            }
            else if (dRay1.collider != null)
            {
                dashPoint.position = new Vector2(dRay1.point.x + m_dashRayXOffset, player.position.y);
            }
            else if (dRay2.collider != null)
            {
                dashPoint.position = new Vector2(dRay2.point.x + m_dashRayXOffset, player.position.y);
            }
        }
        //if all rays return null set dashPoint to specific spot
        if (dRay.collider == null && dRay1.collider == null && dRay2.collider == null)
        {
            if (m_facingRight)
            {
                dashPoint.position = new Vector2(player.position.x + m_dashDistance, player.position.y);
            } else if (!m_facingRight)//switch if not facing right
            {
                dashPoint.position = new Vector2(player.position.x - m_dashDistance, player.position.y);
            }
        }

        //set Dashing to true
        m_isDashing = true;
        //set the X and Y velocity to 0 and remove gravity;
        rb.velocity = new Vector2(0f, 0f);
        rb.gravityScale = 0f;
        //start the Dash Coroutine
        StartCoroutine(Dash(m_dashCooldown));

    }

    IEnumerator Dash(float dashCooldown)
    {
        //set dashing variable to true after certain amount of seconds
        yield return new WaitForSeconds(dashCooldown);
        m_canDash = true;
    }
    #endregion
    #region StopMovement()
    //locks movement
    void StopMovement()
    {
        MovementLock = true;
    }
    #endregion
    #region ReturnMovement()
    //returns movement
    void ReturnMovement()
    {
        MovementLock = false;
    }
    #endregion
    #region OnDrawGizmos()
    //Gizmo for ground check
    void OnDrawGizmos()
    {
        //change gizmo color
        Gizmos.color = Color.red;
        //draw circle gizmo
        Gizmos.DrawWireSphere(new Vector2(player.position.x, player.position.y - m_groundOffset), m_groundRadius);
    }
    #endregion
    #region Flip()
    void Flip()
    {
        //invert the facingRight bool
        m_facingRight = !m_facingRight;
        Vector3 localScale = transform.localScale;
        //flip the sprite on X axis
        localScale.x *= -1;
        transform.localScale = localScale;
    }
    #endregion
}
