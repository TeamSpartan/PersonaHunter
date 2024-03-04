using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unnko
    : MonoBehaviour
    , IInitializableComponent
{
    public void InitializeThisComponent()
    {
        // Debug.Log($"{nameof(Unnko)} : Is Init");
    }

    public void FinalizeThisComp()
    {
        // Debug.Log($"{nameof(Unnko)} : Is Finalized");
    }
}
