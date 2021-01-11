using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLlamaHat : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player")
        {
            GameObject hat = GameObject.Find("SM_GliderHat_MD");
            print("hit");
            if (hat)
            {
                hat.SetActive(true);
                Destroy(gameObject.transform.parent.gameObject);
            }
            else
                print("null");

        }
        else
        {
            print("other");
        }
    }

}