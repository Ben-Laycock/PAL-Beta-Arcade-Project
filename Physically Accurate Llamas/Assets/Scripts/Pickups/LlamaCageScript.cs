using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LlamaCageScript : MonoBehaviour
{
    [Tooltip("Mini llama game object goes here, shouldn't need to be changed.")]
    [SerializeField] private GameObject mLlamaObject = null;
    public GameObject llamaObject
    {
        get { return mLlamaObject; }
    }

    [Tooltip("How many llamas come from the cage? This also determines how many points it awards for the llama cage collectable.")]
    [SerializeField] private int mLlamaAmount = 1;
    public int llamaAmount
    {
        get { return mLlamaAmount; }
    }

    [Tooltip("The pickup name for the llama cage, shouldn't need to be changed.")]
    [SerializeField] private string mPickupName = "CagedLlama";
    public string pickupName
    {
        get { return mPickupName; }
    }

    private bool mHasBeenDestroyed = false;
    public bool hasBeenDestroyed
    {
        set { mHasBeenDestroyed = value; }
        get { return mHasBeenDestroyed; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
