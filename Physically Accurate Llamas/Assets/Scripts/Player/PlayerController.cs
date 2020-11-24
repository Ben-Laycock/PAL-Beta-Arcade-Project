﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public enum EMovementState
    {
        eIdle,
        eWalking,
        eFalling,
        eGliding
    }


    private Rigidbody mRigidbody = null;

    [SerializeField] private float mGravityScale = 1f;


    [SerializeField] private float mGlidingGravityScale = 0.25f;


    [SerializeField] private LayerMask mGroundLayerMaskCheck = new LayerMask();
    private bool mGrounded = false;

    [SerializeField] private float mMaxSlopeAngle = 30f;

    [Tooltip("Used to toggle player character control while not grounded.")]
    [SerializeField] private bool mCanControlInAir = true;

    [SerializeField] private float mMovementSpeed = 5f;
    [SerializeField] private float mRunningSpeed = 10f;

    private bool mShouldRun = false;

    [Tooltip("Time taken for character to get to top speed. Smaller values take longer.")]
    [SerializeField] private float mTimeToLerpToMaxSpeed = 0.1f;

    [Tooltip("Time taken for the character to rotate to the movement direction. Smaller values take longer.")]
    [SerializeField] private float mRotationLerpSpeed = 0.1f;

    [SerializeField] private float mMaxAngleBeforeFlip = 130f;

    [SerializeField] private bool mSliding = false;
    private Vector3 mGroundNormal = Vector3.zero;

    private bool mShouldJump = false;
    [SerializeField] private float mJumpForce = 10f;

    private bool mShouldDash = false;
    [SerializeField] private float mTimeBetweenDashAttacks = 3f;
    private float mTimeSincePreviousDashAttack = 0f;
    [Tooltip("How long the dash should remain active after the keypress")]
    [SerializeField] private float mLengthOfDash = 1f;
    private bool mIsDashing = false;

    private Vector3 mTargetMovementVector = Vector3.zero;

    private EMovementState mMovementState = EMovementState.eIdle;


    [Space]
    [Space]
    [Space]


    /*
     * Camera Variables
     */
    [SerializeField] private bool mInvertYRotation = false;
    [SerializeField] private Vector2 mYRotationMinMax = new Vector2(-15.0f, 30.0f);
    [SerializeField] private GameObject mCameraRotationXPivot = null;
    [SerializeField] private GameObject mCameraRotationYPivot = null;

    [SerializeField] private bool mEnableAutomaticCameraAdjustment = false;
    private bool mShouldAutoAdjustCamera = false;
    [SerializeField] private float mTimeUntilAutomaticCameraAdjustment = 2f;
    private float mTimeUntilStartOfAutomaticAdjustment = 0f;
    [SerializeField] private float mCameraAutomaticAdjustmentSpeed = 2f;

    [SerializeField] private float mCameraRotationSensitivity = 50f;

    float mYRotation = 0f;


    void Start()
    {

        // Find the players Rigidbody component
        mRigidbody = this.gameObject.GetComponent<Rigidbody>();
        mRigidbody.useGravity = false;

        Cursor.lockState = CursorLockMode.Locked;


        // Find the camera pivot points in the scene
        mCameraRotationXPivot = GameObject.Find("CameraRotationXPivot");
        mCameraRotationYPivot = GameObject.Find("CameraRotationYPivot");

    }



    void Update()
    {

        // Temporary way to stop player when game is paused
        if (GameConstants.Instance.GamePaused)
        {
            mRigidbody.isKinematic = true;
            return;
        }
        else
        {
            mRigidbody.isKinematic = false;
        }

        // Update dash attack timer
        mTimeSincePreviousDashAttack += Time.deltaTime;
        mTimeSincePreviousDashAttack = Mathf.Clamp(mTimeSincePreviousDashAttack, 0, mTimeBetweenDashAttacks);
        mIsDashing = mTimeSincePreviousDashAttack <= mLengthOfDash;


        //Check if the player is grounded
        mGrounded = IsPlayerGrounded();

        // Get the players target movement direction
        mTargetMovementVector = GetPlayerTargetMovementVector();

        // Account for controller stick deadzones
        if (mTargetMovementVector.magnitude < GameConstants.Instance.LeftStickDeadzone)
            mTargetMovementVector = Vector3.zero;


        // Manage player movement state machine
        if (mGrounded)
        {
            if (Vector3.zero == mTargetMovementVector)
                mMovementState = EMovementState.eIdle;
            else
                mMovementState = EMovementState.eWalking;
        }
        else
        {
            if (Input.GetAxisRaw(GameConstants.Instance.ControllerBInput) > 0)
                mMovementState = EMovementState.eGliding;
            else
                mMovementState = EMovementState.eFalling;
        }



        //mShouldRun = Input.GetAxisRaw(GameConstants.Instance.RunInput) > 0;


        // Rotate player character towards target movement direction / flip player to face target movement direction
        RotatePlayerCharacter();


        // Check if player wants to jump (Make sure player is grounded first)
        if (mGrounded && Input.GetAxisRaw(GameConstants.Instance.JumpInput) > 0)
            mShouldJump = true;


        // Check if the player wants to dash
        if (Input.GetKeyDown(KeyCode.M) && mMovementState == EMovementState.eWalking && mTimeSincePreviousDashAttack >= mTimeBetweenDashAttacks)
            mShouldDash = true;


        // Camera Management
        ManageCamera();

    }


    private void FixedUpdate()
    {

        // Temporary way to stop player when game is paused
        if (GameConstants.Instance.GamePaused)
            return;

        switch(mMovementState)
        {
            case EMovementState.eIdle:
                {
                    mRigidbody.velocity = Vector3.zero;
                }
                break;

            case EMovementState.eWalking:
                {
                    // Project the players target movement direction onto the surface the player is standing on
                    Vector3 movementDirection = Vector3.ProjectOnPlane(mTargetMovementVector, mGroundNormal).normalized * mMovementSpeed;

                    mRigidbody.velocity = Vector3.Lerp(mRigidbody.velocity, movementDirection, mTimeToLerpToMaxSpeed);
                }
                break;

            case EMovementState.eFalling:
                {
                    ApplyGravity();

                    // Apply sliding motion on surface normal
                    if (mSliding)
                    {
                        Vector3 slidingDirection = Vector3.ProjectOnPlane(GameConstants.Instance.GravityDirection, mGroundNormal).normalized * GameConstants.Instance.GlobalGravityScale;

                        mRigidbody.velocity = Vector3.Lerp(mRigidbody.velocity, slidingDirection, 0.1f);
                    }

                    CharacterAirControl();
                }
                break;

            case EMovementState.eGliding:
                {
                    ApplyGliderHatForce();
                    CharacterAirControl();
                }
                break;

            default:
                break;
        }


        if (mShouldJump)
        {
            mShouldJump = false;
            mRigidbody.velocity = new Vector3(mRigidbody.velocity.x, 0, mRigidbody.velocity.z);
            mRigidbody.AddForce(-GameConstants.Instance.GravityDirection * mJumpForce, ForceMode.Impulse);
        }


        if (mShouldDash)
        {
            mShouldDash = false;
            mTimeSincePreviousDashAttack = 0f;
            mRigidbody.AddForce(transform.forward * mJumpForce, ForceMode.Impulse);
        }

    }


    private void ApplyGravity()
    {

        Vector3 gravity = GameConstants.Instance.GlobalGravityScale * mGravityScale * GameConstants.Instance.GravityDirection;
        mRigidbody.AddForce(gravity, ForceMode.Acceleration);

    }


    public void ApplyGliderHatForce()
    {

        Vector3 gliderForce = GameConstants.Instance.GlobalGravityScale * mGlidingGravityScale * GameConstants.Instance.GravityDirection;
        //mRigidbody.AddForce(gliderForce, ForceMode.Acceleration);
        mRigidbody.velocity = new Vector3(mRigidbody.velocity.x, gliderForce.y, mRigidbody.velocity.z); // Temporary fix (Doesnt take gravity direction into acount)

    }


    private void CharacterAirControl()
    {

        // Allow player to control character in the air
        if (mCanControlInAir && !mSliding)
        {
            Vector3 movementDirection = mTargetMovementVector.normalized * mMovementSpeed;
            movementDirection.y = mRigidbody.velocity.y;

            mRigidbody.velocity = Vector3.Lerp(mRigidbody.velocity, movementDirection, mTimeToLerpToMaxSpeed);
        }

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

        targetMovementVector += mCameraRotationXPivot.transform.forward * Input.GetAxisRaw(GameConstants.Instance.VerticalInput);
        targetMovementVector += mCameraRotationXPivot.transform.right * Input.GetAxisRaw(GameConstants.Instance.HorizontalInput);

        return targetMovementVector.normalized;

    }


    public void ManageCamera()
    {

        mTimeUntilStartOfAutomaticAdjustment -= Time.deltaTime;

        // Move camera anchor point to current player position
        mCameraRotationXPivot.transform.position = transform.position;

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
            mCameraRotationXPivot.transform.forward = Vector3.Lerp(mCameraRotationXPivot.transform.forward, transform.forward, mCameraAutomaticAdjustmentSpeed);
        }
        else
        {
            mCameraRotationXPivot.transform.eulerAngles += new Vector3(0, cameraRotationInput.x * mCameraRotationSensitivity, 0);
        }


        // Y Pivot
        mYRotation += cameraRotationInput.y * mCameraRotationSensitivity;
        mYRotation = Mathf.Clamp(mYRotation, mYRotationMinMax.x, mYRotationMinMax.y);

        if (!mInvertYRotation)
            mCameraRotationYPivot.transform.localRotation = Quaternion.Euler(mYRotation, 0, 0);
        else
            mCameraRotationYPivot.transform.localRotation = Quaternion.Euler(-mYRotation, 0, 0);

    }


    public void RotatePlayerCharacter()
    {

        if (Vector3.Angle(mTargetMovementVector, transform.forward) <= mMaxAngleBeforeFlip)
            transform.forward = Vector3.Lerp(transform.forward, mTargetMovementVector, mRotationLerpSpeed);
        else
        {
            transform.forward = mTargetMovementVector;
            mTimeUntilStartOfAutomaticAdjustment = mTimeUntilAutomaticCameraAdjustment;
        }

    }


    private void OnDrawGizmos()
    {

        Gizmos.DrawWireSphere(transform.position + Vector3.down * 0.60f, 0.45f);

    }


    public EMovementState GetMovementState()
    {
        return mMovementState;
    }


    public bool GetIsDashing()
    {
        return mIsDashing;
    }

}
