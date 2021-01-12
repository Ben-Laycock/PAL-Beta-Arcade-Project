using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitParticleEmitter : MonoBehaviour
{

    public GameObject mSpitParticleObject = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        print("hit");
        GameObject temp = PoolSystem.Instance.GetObjectFromPool(mSpitParticleObject, argActivateObject: true, argShouldExpandPool: true, argShouldCreateNonExistingPool: true);
        temp.transform.position = transform.position;
        DeactivateObjectAfterTime deac = temp.GetComponent<DeactivateObjectAfterTime>();
        deac.ResetTimer();
        deac.ActivateTimer();
        Destroy(this.gameObject);
    }
}
