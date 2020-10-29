using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy: MonoBehaviour
{
    [SerializeField] public float mLookArea = 10f;

    Transform mPlayer;
    NavMeshAgent mAgent;


    private void Start ()
    {
        mPlayer = PlayerTarget.instance.mPlayer.transform;
        mAgent = GetComponent<NavMeshAgent>();
        
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
    
    }

    void FacePlayer ()
    {
        Vector3 direction = (mPlayer.position - transform.position).normalized;
        Quaternion lookRoatation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRoatation, Time.deltaTime * 5f);

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, mLookArea);
    }


}
