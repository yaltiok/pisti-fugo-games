using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaHighlight : MonoBehaviour
{
    public float alphaValue = 30f;

    private MeshRenderer areaRenderer;
    private MeshRenderer handRenderer;
    private Color highlightedColor;
    private Color defaultColor;




    private void Start()
    {
        areaRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        handRenderer = transform.GetChild(1).GetComponent<MeshRenderer>();

        highlightedColor = new Color(1f,1f,1f,30f/255f);
        defaultColor = new Color(1f,1f,1f,0f);
    }

    public void HighlightArea(int index)
    {
        if (index == 0)
        {
            areaRenderer.material.color = highlightedColor;
            handRenderer.material.color = defaultColor;
            

        }
        else
        {
            areaRenderer.material.color = defaultColor;
            handRenderer.material.color = highlightedColor;
        }
    }

    public void ResetHighlight()
    {
        areaRenderer.material.color = defaultColor;
        handRenderer.material.color = defaultColor;
    }
}
