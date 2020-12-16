using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy: MonoBehaviour
{
    [SerializeField] public float mLookArea = 10f;
    [SerializeField] public Transform[] mPatrolPoints;
    [SerializeField] public GameObject mThePlayer;
    [SerializeField] public Rigidbody mProjectile;
    [SerializeField] public float mBulletForce = 20f;
    [SerializeField] float mRange = 50f;
    
    private int mDesPoint = 0;
    private bool mInRange = false;
   
    Transform mPlayer;
    NavMeshAgent mAgent;


    private void Start ()
    {
       
        mPlayer = PlayerTarget.instance.mPlayer.transform;
        mAgent = GetComponent<NavMeshAgent>();
        mAgent.autoBraking = false;

        GoToNextPoint();

        float rand = Random.Range(1f, 2f);
        InvokeRepeating("Shoot", 2, rand);
    }


    private void Update()
    {
        float distance = Vector3.Distance(mPlayer.position, transform.position);

        if (distance <= mLookArea)
        {
            mAgent.SetDestination(mPlayer.position);

           
           if (distance <= mAgent.stoppingDistance)
            {
                FacePlayer();

            } 

        }

        if (!mAgent.pathPending && mAgent.remainingDistance < 0.5f)
            GoToNextPoint();
        else
        {
            

        }


        mInRange = Vector3.Distance(transform.position, mPlayer.position) < mRange;

        if (mInRange)
            transform.LookAt(mPlayer);
    
    
    }

    void FacePlayer ()
    {
        Vector3 direction = (mPlayer.position - transform.position).normalized;
        Quaternion lookRoatation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRoatation, Time.deltaTime * 5f);

    }


    void GoToNextPoint()
    {

        if (mPatrolPoints.Length == 0)
            return;

        mAgent.destination = mPatrolPoints[mDesPoint].position;
        mDesPoint = (mDesPoint + 1) % mPatrolPoints.Length;

     }


    void Shoot()
    {

        if (mInRange)
        {
            Rigidbody bullet = (Rigidbody)Instantiate(mProjectile, transform.position + transform.forward, transform.rotation);
            bullet.AddForce(transform.forward * mBulletForce, ForceMode.Impulse);

            Destroy(bullet.gameObject, 2);

        }

    }
    
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, mLookArea);
    }


}
