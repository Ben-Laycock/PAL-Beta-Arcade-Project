using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public enum EMovementState
    {
        eIdle,
        eWalking,
        eRunning,
        eFalling
    }


    private Rigidbody mRigidbody = null;

    [SerializeField] private float mGravityScale = 1f;

    [SerializeField] private LayerMask mGroundLayerMaskCheck = new LayerMask();
    private bool mGrounded = false;

    [SerializeField] private float mMaxSlopeAngle = 30f;

    [Tooltip("Used to toggle player character control while not grounded.")]
    [SerializeField] private bool mCanControlInAir = true;

    [SerializeField] private float mMovementSpeed = 5f;

    [Tooltip("Time taken for character to get to top speed. Smaller values take longer.")]
    [SerializeField] private float mTimeToLerpToMaxSpeed = 0.1f;

    [Tooltip("Time taken for the character to rotate to the movement direction. Smaller values take longer.")]
    [SerializeField] private float mRotationLerpSpeed = 0.1f;

    [SerializeField] private float mMaxAngleBeforeFlip = 130f;

    [SerializeField] private bool mSliding = false;
    private Vector3 mGroundNormal = Vector3.zero;

    private bool mShouldJump = false;
    [SerializeField] private float mJumpForce = 10f;

    private Vector3 mTargetMovementVector = Vector3.zero;

    private EMovementState mMovementState = EMovementState.eIdle;


    [Space][Space][Space]


    /*
     * Camera Variables
     */
    [SerializeField] private GameObject mCameraAnchorPoint = null;

    [SerializeField] private bool mEnableAutomaticCameraAdjustment = false;
    private bool mShouldAutoAdjustCamera = false;
    [SerializeField] private float mTimeUntilAutomaticCameraAdjustment = 2f;
    private float mTimeUntilStartOfAutomaticAdjustment = 0f;
    [SerializeField] private float mCameraAutomaticAdjustmentSpeed = 2f;

    [SerializeField] private float mCameraRotationSensitivity = 50f;


    void Start()
    {

        // Find the players Rigidbody component
        mRigidbody = this.gameObject.GetComponent<Rigidbody>();
        mRigidbody.useGravity = false;

        Cursor.lockState = CursorLockMode.Locked;

        mCameraAnchorPoint = GameObject.Find("CameraAnchorPoint");

    }

    

    void Update()
    {

        //Check if the player is grounded (Store value in mGrounded for use in FixedUpdate)
        mGrounded = IsPlayerGrounded();

        // Get the players target movement direction
        mTargetMovementVector = GetPlayerTargetMovementVector();

        // Set the target movement vector to none if it is less than the specified deadzone value
        if (mTargetMovementVector.magnitude < GameConstants.Instance.LeftStickDeadzone)
            mTargetMovementVector = Vector3.zero;

        // Rotate player character towards target movement direction / flip player to face target movement direction
        if (Vector3.Angle(mTargetMovementVector, transform.forward) <= mMaxAngleBeforeFlip)
            transform.forward = Vector3.Lerp(transform.forward, mTargetMovementVector, mRotationLerpSpeed);
        else
        {
            transform.forward = mTargetMovementVector;
            mTimeUntilStartOfAutomaticAdjustment = mTimeUntilAutomaticCameraAdjustment;
        }

        // Check if player wants to jump (Make sure player is grounded first)
        if (mGrounded && Input.GetAxisRaw(GameConstants.Instance.JumpInput) > 0)
            mShouldJump = true;




        /*
         * Camera Management
         */
        mTimeUntilStartOfAutomaticAdjustment -= Time.deltaTime;

        // Move camera anchor point to current player position
        mCameraAnchorPoint.transform.position = transform.position;

        // Get the camera rotation values from player input
        Vector2 cameraRotationInput = new Vector2(Input.GetAxisRaw(GameConstants.Instance.HorizontalLookInput), Input.GetAxisRaw(GameConstants.Instance.VerticalLookInput));

        // Set input to none if it is less that the specified deadzone value
        if (cameraRotationInput.magnitude < GameConstants.Instance.RightStickDeadzone)
        {
            cameraRotationInput = Vector2.zero;
        }
        else
        {
            // Camera input detected
            mTimeUntilStartOfAutomaticAdjustment = mTimeUntilAutomaticCameraAdjustment;
        }

        mShouldAutoAdjustCamera = (mTimeUntilStartOfAutomaticAdjustment <= 0.0f);

        // Apply the camera rotation on the X axis
        if (mEnableAutomaticCameraAdjustment && mShouldAutoAdjustCamera)
        {
            // Automatically adjust camera to player direction
            mCameraAnchorPoint.transform.forward = Vector3.Lerp(mCameraAnchorPoint.transform.forward, transform.forward, mCameraAutomaticAdjustmentSpeed);
        }
        else
        {
            mCameraAnchorPoint.transform.eulerAngles += new Vector3(0, cameraRotationInput.x * mCameraRotationSensitivity, 0);
        }

    }


    private void FixedUpdate()
    {

        // Check if the player is grounded before applying movement
        if (mGrounded)
        {
            // Player should stand still since there is no movement input
            if (Vector3.zero == mTargetMovementVector)
            {
                mMovementState = EMovementState.eIdle;
                mRigidbody.velocity = Vector3.zero;
            }
            else
            {
                mMovementState = EMovementState.eWalking;

                // Project the players target movement direction onto the surface the player is standing on
                Vector3 movementDirection = Vector3.ProjectOnPlane(mTargetMovementVector, mGroundNormal).normalized * mMovementSpeed;

                mRigidbody.velocity = Vector3.Lerp(mRigidbody.velocity, movementDirection, mTimeToLerpToMaxSpeed);
            }
        }
        else
        {

            ApplyGravity();
            
            // Apply sliding motion on surface normal
            if (mSliding)
            {
                Vector3 slidingDirection = Vector3.ProjectOnPlane(GameConstants.Instance.GravityDirection, mGroundNormal).normalized * GameConstants.Instance.GlobalGravityScale;

                mRigidbody.velocity = Vector3.Lerp(mRigidbody.velocity, slidingDirection, 0.1f);
            }

            // Allow player to control character in the air
            if (mCanControlInAir && !mSliding)
            {
                Vector3 movementDirection = mTargetMovementVector.normalized * mMovementSpeed;
                movementDirection.y = mRigidbody.velocity.y;

                mRigidbody.velocity = Vector3.Lerp(mRigidbody.velocity, movementDirection, mTimeToLerpToMaxSpeed);
            }
        }


        if (mShouldJump)
        {
            mShouldJump = false;
            mRigidbody.velocity = new Vector3(mRigidbody.velocity.x, 0, mRigidbody.velocity.z);
            mRigidbody.AddForce(-GameConstants.Instance.GravityDirection * mJumpForce, ForceMode.Impulse);
        }

    }


    private void ApplyGravity()
    {

        Vector3 gravity = GameConstants.Instance.GlobalGravityScale * mGravityScale * GameConstants.Instance.GravityDirection;
        mRigidbody.AddForce(gravity, ForceMode.Acceleration);

    }


    private bool IsPlayerGrounded()
    {

        RaycastHit hit;

        if (Physics.SphereCast(transform.position, 0.45f, Vector3.down, out hit, 0.60f, mGroundLayerMaskCheck))
        {
            // Check if the raycast hit anything (Return false if not)
            if (null == hit.transform)
                return false;

            // Check the angle between -GravityDirection and the slope normal to make sure the player can walk on it
            mGroundNormal = hit.normal;
            if (Vector3.Angle(mGroundNormal, -GameConstants.Instance.GravityDirection) > mMaxSlopeAngle)
            {
                mSliding = true;
                return false;
            }
            else
            {
                mSliding = false;
                return true;
            }
        }

        return false;

    }


    private Vector3 GetPlayerTargetMovementVector()
    {

        Vector3 targetMovementVector = Vector3.zero;

        targetMovementVector += mCameraAnchorPoint.transform.forward * Input.GetAxisRaw(GameConstants.Instance.VerticalInput);
        targetMovementVector += mCameraAnchorPoint.transform.right * Input.GetAxisRaw(GameConstants.Instance.HorizontalInput);

        return targetMovementVector.normalized;

    }


    private void OnDrawGizmos()
    {

        Gizmos.DrawWireSphere(transform.position + Vector3.down * 0.60f, 0.45f);

    }

}
