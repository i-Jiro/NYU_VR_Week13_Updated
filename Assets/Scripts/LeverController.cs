using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MonoBehaviour
{
    public Transform StartRotation;
    public Transform EndRotation;

    private MeshRenderer _meshRenderer;

    private void Start()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void OnDragStart()
    {
        _meshRenderer.material.SetColor("_BaseColor", Color.red);
    }

    public void OnDragStop()
    {
        _meshRenderer.material.SetColor("_BaseColor", Color.white);
    }

    public void UpdateDrag(float percent)
    {
        transform.rotation = Quaternion.Slerp(StartRotation.rotation, EndRotation.rotation, percent);
    }
}
