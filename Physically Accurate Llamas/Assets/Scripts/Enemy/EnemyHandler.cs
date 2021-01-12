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

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.name == "Player")
        {
            if(mPlayerController.GetIsCharging())
            {
                Instantiate(mDeathParticle, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }

}