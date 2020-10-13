using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    private int one;
    private int one1;
    private int one2;
    private int one3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (one == one1)
        {
            one1 = one2;
        }
    }
}
