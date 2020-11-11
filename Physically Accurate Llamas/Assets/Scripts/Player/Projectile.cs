using UnityEngine;
using System.Collections;
 
public class Projectile: MonoBehaviour
{
    [SerializeField] private GameObject mSpawnPoint;
    [SerializeField] private GameObject mProjectile;
    [SerializeField] private float mProjectileForce;

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            GameObject BulletHandler;
            BulletHandler = Instantiate(mProjectile, mSpawnPoint.transform.position, mSpawnPoint.transform.rotation) as GameObject;

   
            BulletHandler.transform.Rotate(Vector3.left * 90);

            
            Rigidbody Temporary_RigidBody;
            Temporary_RigidBody = BulletHandler.GetComponent<Rigidbody>();

            Temporary_RigidBody.AddForce(transform.forward * mProjectileForce);

            
            Destroy(BulletHandler, 0.5f);
        }
    }
}