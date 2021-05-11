using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitCollider : MonoBehaviour
{

    public void FitColliders(Vector2 S)
    {
        GetComponent<BoxCollider2D>().size = S;
    }
}
