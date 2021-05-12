using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitCollider : MonoBehaviour
{

    public BoxCollider2D boxCollider;

    public void FitColliders(Vector2 S)
    {
        boxCollider.size = S;
    }
}
