using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaHighlight : MonoBehaviour
{
    public float alphaValue = 30f;
    private const float HAND_RATIO = .2f;


    private Camera cam;
    private Vector3 centerUp;
    private Vector3 centerDown;

    private MeshRenderer middleRenderer;
    private MeshRenderer handRenderer;
    private GameObject middleObject;
    private GameObject handObject;
    private Color highlightedColor;
    private Color defaultColor;
    [HideInInspector]
    public float handAreaHeight;
    private float screenHeightInWorld;
    private float halfHeight;
    




    private void Start()
    {
        
        SetChildren();
        SetRenderers();
        SetColors();

        halfHeight = middleRenderer.bounds.size.y / 2;

        SetHandAreaHight();
        SetPanelPositions();
    }

    private void SetColors()
    {
        highlightedColor = new Color(1f, 1f, 1f, 30f / 255f);
        defaultColor = new Color(1f, 1f, 1f, 0f);
    }

    private void SetRenderers()
    {
        middleRenderer = middleObject.GetComponent<MeshRenderer>();
        handRenderer = handObject.GetComponent<MeshRenderer>();
    }

    private void SetChildren()
    {
        middleObject =  transform.GetChild(0).gameObject;
        handObject = transform.GetChild(1).gameObject;
    }

    private void SetPanelPositions()
    {
        middleObject.transform.position += new Vector3(0, halfHeight + handAreaHeight, 0);
        handObject.transform.position += new Vector3(0, -halfHeight + handAreaHeight, 0);
    }

    private void SetHandAreaHight()
    {
        cam = Camera.main;
        centerUp = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth / 2, cam.pixelHeight, 0));
        centerDown = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth / 2, 0, 0));
        screenHeightInWorld = (centerUp - centerDown).y;
        handAreaHeight = centerDown.y + screenHeightInWorld * HAND_RATIO;
    }

    public void HighlightArea(int index)
    {
        if (index == 0)
        {
            middleRenderer.material.color = highlightedColor;
            handRenderer.material.color = defaultColor;
            

        }
        else
        {
            middleRenderer.material.color = defaultColor;
            handRenderer.material.color = highlightedColor;
        }
    }

    public void ResetHighlight()
    {
        middleRenderer.material.color = defaultColor;
        handRenderer.material.color = defaultColor;
    }
}
