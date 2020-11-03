using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy: MonoBehaviour
{
    [SerializeField] public float mLookArea = 10f;
    [SerializeField] public Transform[] mPatrolPoints;
    [SerializeField] public GameObject mThePlayer;
    
    private int mDesPoint = 0;
   
    Transform mPlayer;
    NavMeshAgent mAgent;


    private void Start ()
    {
        mPlayer = PlayerTarget.instance.mPlayer.transform;
        mAgent = GetComponent<NavMeshAgent>();
        mAgent.autoBraking = false;

        GoToNextPoint();
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, mLookArea);
    }


}
