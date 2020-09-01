using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC_movementScript : MonoBehaviour
{
    //Game Manager Singleton

    //GameObject Components
    [Header("GameObject Components")]

    [SerializeField] private Rigidbody m_rb;
    [SerializeField] private Animator m_animator;

    //Movement Members
    [Header("Movement Members")]
    public float m_moveSpeed = 10f;
    private Vector3 m_moveInput;
    [SerializeField]private Vector3 m_charForward;

    //Jump Members
    [Header("Jump Members")]
    public float m_jumpForce = 10;
    public float m_gravityForce = 9.81f;
   /* [HideInInspector]*/ public bool m_isGrounded;
    public Transform m_groundCheck;
    public float m_checkRadius;
    public LayerMask m_whatIsGround;

    public int m_extraJumpValue = 2;
    private int m_extraJumps;

    public float m_jumpTime;
    private float m_jumpTimeCounter = 0;
    private bool m_isJumping;


    //Dash members
    [Header("Dash members")]
    public float m_dashSpeed;
    public float m_startDashTime;
    public float m_dashCost;
    private float m_dashTime;
    private float m_dashDirection;
    private bool m_hasDashed;
    private bool m_isDashing = false;

    [Header("WallSlide Members")]
    public Transform m_frontCheck;
    public float m_wallSlidingSpeed;
    private bool m_isTouchingFront;
    [SerializeField]private bool m_wallSliding;

    [Header("WallJump Members")]
    public float m_horizontalWallForce;
    public float m_verticalWallForce;
    public float m_wallJumpTime;
    private bool m_wallJumping;

    //Particles
    [Header("VFX Effects")]
    public GameObject m_fx_dash;
    // Start is called before the first frame update
    void Start()
    {

        if (m_rb == null) m_rb = this.GetComponent<Rigidbody>();
       // if (m_animator == null) m_animator = this.GetComponent<Animator>();
        m_extraJumps = m_extraJumpValue;
        m_jumpTimeCounter = m_jumpTime;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        m_isGrounded = Physics.CheckSphere(m_groundCheck.position, m_checkRadius, m_whatIsGround);
        //m_animator.SetBool("isJumping", !m_isGrounded);
        if (!m_isGrounded)
            m_rb.velocity += Vector3.down * m_gravityForce * Time.deltaTime;
      
        #region movement
        if (!m_isDashing && !m_wallJumping)
            {
                m_moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f);
                m_moveInput.z = Input.GetAxis("Vertical");
                m_rb.velocity = Vector3.right * m_moveInput.x * m_moveSpeed 
                + Vector3.forward * m_moveInput.z * m_moveSpeed
                + Vector3.up * m_rb.velocity.y;

            if (m_moveInput.magnitude > 0)
            {
                m_charForward = m_moveInput.normalized;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_moveInput).normalized, 0.3f);
                //this.transform.forward = Vector3.Lerp(this.transform.forward, m_charForward, 0.3f) ;
            }
               // m_animator.SetBool("isMoving", m_moveInput.x != 0);
            }
        #endregion
    }

    void Update()
    {
        if (m_isGrounded)
        {
            m_hasDashed = false;
            m_extraJumps = m_extraJumpValue;
        }


        #region Jump
        if (Input.GetKeyDown(KeyCode.Space) && m_wallSliding)
        {
            m_wallJumping = true;
            Invoke("SetWallJumpingToFalse", m_wallJumpTime);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && m_extraJumps > 0)
        {
            m_isJumping = true;
            m_jumpTimeCounter = m_jumpTime;
            m_rb.velocity = Vector2.up * m_jumpForce;
            m_extraJumps--;

            //Instantiate(m_fx_dash, m_groundCheck.position, Quaternion.identity);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && m_extraJumps == 0 && m_isGrounded)
        {
            m_isJumping = true;
            m_jumpTimeCounter = m_jumpTime;
            m_rb.velocity = Vector2.up * m_jumpForce;

           // Instantiate(m_fx_dash, m_groundCheck.position, Quaternion.identity);
        }

        if (m_wallJumping)
        {
            Vector3 l_vector = new Vector3(m_horizontalWallForce * -m_charForward.x, m_verticalWallForce, m_horizontalWallForce * -m_charForward.z);
            m_rb.velocity = l_vector;
            Debug.Log(l_vector + "|" + m_rb.velocity);
        }
        else
        {
            if (Input.GetKey(KeyCode.Space))
            {
                if (m_jumpTimeCounter > 0 && m_isJumping)
                {
                    m_rb.velocity = Vector3.up * m_jumpForce;
                    m_jumpTimeCounter -= Time.deltaTime;
                }
                else
                    m_isJumping = false;
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                m_isJumping = false;
            }
        }
        #endregion

        #region Dash
        if (!m_hasDashed && !m_isDashing)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                m_isDashing = true;
                //m_animator.SetBool("isDashing", m_isDashing);

                m_rb.velocity = m_charForward * m_dashSpeed;
                m_dashTime = m_startDashTime;
              //  Instantiate(m_fx_dash, this.transform.position, Quaternion.identity);
            }
        }

        if (m_dashTime > 0)
        {
            m_dashTime -= Time.deltaTime;
            if (m_dashTime <= 0)
            {
                m_rb.velocity = Vector2.zero;
                m_hasDashed = true;
                m_isDashing = false;
               // m_animator.SetBool("isDashing", m_isDashing);
            }
        }
        #endregion

        #region WallSliding
        m_isTouchingFront = Physics.CheckSphere(m_frontCheck.position, m_checkRadius, m_whatIsGround);
        if (m_isTouchingFront && !m_isGrounded && (m_moveInput.x != 0 | m_moveInput.z != 0))
            m_wallSliding = true;
        else
            m_wallSliding = false;

        if (m_wallSliding)
            m_rb.velocity = new Vector2(m_rb.velocity.x, Mathf.Clamp(m_rb.velocity.y, -m_wallSlidingSpeed, float.MaxValue));
        #endregion

    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_groundCheck.position, m_checkRadius);
        Gizmos.DrawWireSphere(m_frontCheck.position, m_checkRadius);
    }
    private void SetWallJumpingToFalse()
    {
        m_wallJumping = false;
    }
}
