using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : MonoBehaviour
{ 

    [SerializeField] private GameObject mDeathParticle;

    private GameObject mPlayerObject;

    private PlayerController mPlayerController;

    private void Start()
    {
        mPlayerObject = GameObject.Find("Player");

        mPlayerController = mPlayerObject.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            if(mPlayerController.GetMovementState() == PlayerController.EMovementState.eCharging)
            {
                Instantiate(mDeathParticle, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }

}