using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private GameObject mPlayerCameraAnchorPoint = null;
    private Rigidbody mRigidbody = null;

    [SerializeField] private LayerMask mGroundLayerMaskCheck = new LayerMask();
    [SerializeField] private float mMaxSlopeAngle = 30f;
    [SerializeField] private float mMaxSlideAngle = 80f;

    [SerializeField] private float mMovementSpeed = 5f;

    [Tooltip("Time taken for character to get to top speed. Smaller values take longer.")]
    [SerializeField] private float mTimeToLerpToMaxSpeed = 0.1f;

    [Tooltip("Time taken for the character to rotate to the movement direction. Smaller values take longer.")]
    [SerializeField] private float mRotationLerpSpeed = 0.1f;

    [SerializeField] private float mMaxAngleBeforeFlip = 130f;

    [SerializeField] private bool mSliding = false;
    private Vector3 mGroundNormal = Vector3.zero;


    [Space][Space][Space]
    /*
     * Camera Variables
     */
    [SerializeField] private GameObject mCameraAnchorPoint = null;

    private bool mShouldAutoAdjustCamera = false;
    [SerializeField] private float mTimeUntilAutomaticCameraAdjustment = 2f;
    private float mAutoAdjustmentTimer = 0f;
    [SerializeField] private float mCameraRotationSensitivity = 50f;
    [SerializeField] private float mControllerLookStickDeadzone = 0.25f;


    void Start()
    {

        // Find the players Rigidbody component
        mRigidbody = this.gameObject.GetComponent<Rigidbody>();

    }

    

    void Update()
    {

        Cursor.lockState = CursorLockMode.Locked;

        /*
        * Get the players target movement direction
        */
        Vector3 targetRotationVector = GetPlayerTargetRotationVector();

        /*
        * Rotate player character towards target movement direction
        */
        if (Vector3.Angle(targetRotationVector, transform.forward) <= mMaxAngleBeforeFlip)
            transform.forward = Vector3.Lerp(transform.forward, targetRotationVector, mRotationLerpSpeed);
        else
            transform.forward = targetRotationVector;

        /*
         * Check if the player is standing on the ground before applying movement
         */
        if (IsPlayerGrounded())
        {
            mRigidbody.useGravity = false;
            /*
             * Make the player stand still if no input is detected
             */
            if (Vector3.zero == targetRotationVector)
            {
                mRigidbody.velocity = Vector3.zero;
            }
            else
            {
                /*
                 * Project the players target movement direction onto the surface the player is standing on
                 */
                Vector3 movementDirection = Vector3.ProjectOnPlane(targetRotationVector, mGroundNormal).normalized * mMovementSpeed;

                mRigidbody.velocity = Vector3.Lerp(mRigidbody.velocity, movementDirection, mTimeToLerpToMaxSpeed);
            }
        }
        else
        {
            /*
             * Player is not grounded so apply gravity
             */
            mRigidbody.useGravity = true;
        }




        /*
         * Camera Management
         */
        mCameraAnchorPoint.transform.position = transform.position;

        /*
         * Get the rotation through the camera input
         */
        Vector2 cameraRotationInput = new Vector2(Input.GetAxisRaw(GameConstants.Instance.HorizontalLookInput), Input.GetAxisRaw(GameConstants.Instance.VerticalLookInput));

        /*
         * Make sure that the input values are greater than the controller deadzone
         */
        if (cameraRotationInput.magnitude < mControllerLookStickDeadzone)
            cameraRotationInput = Vector2.zero;

        cameraRotationInput = cameraRotationInput.normalized;

        /*
         * Apply the camera rotation on the X axis
         */
        mCameraAnchorPoint.transform.Rotate(Vector3.up, cameraRotationInput.x * mCameraRotationSensitivity);

    }


    private bool IsPlayerGrounded()
    {

        RaycastHit hit;

        if (Physics.SphereCast(transform.position, 0.45f, Vector3.down, out hit, 1.0f, mGroundLayerMaskCheck))
        {
            /*
             * Check if the raycast hit anything (Return false if not)
             */
            if (null == hit.transform)
                return false;

            /*
             * Check the angle between -GravityDirection and the slope normal to make sure the player can walk on it
             */
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

    private void OnDrawGizmos()
    {

        Gizmos.DrawWireSphere(transform.position + Vector3.down, 0.45f);

    }

    private Vector3 GetPlayerTargetRotationVector()
    {

        Vector3 targetRotationVector = Vector3.zero;

        targetRotationVector += mPlayerCameraAnchorPoint.transform.forward * Input.GetAxisRaw(GameConstants.Instance.VerticalInput);
        targetRotationVector += mPlayerCameraAnchorPoint.transform.right * Input.GetAxisRaw(GameConstants.Instance.HorizontalInput);

        return targetRotationVector.normalized;

    }

}
