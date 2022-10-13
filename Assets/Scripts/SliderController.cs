using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderController : MonoBehaviour
{
   public Transform StartPosition;
   public Transform EndPosition;

   private MeshRenderer _meshRenderer;

   private void Start()
   {
      _meshRenderer = GetComponent<MeshRenderer>();
   }

   public void OnSlideStart()
   {
         //Use the name '_BaseColor' when using URP. '_Color' is for the standard RP.
      _meshRenderer.material.SetColor("_BaseColor", Color.red);
   }

   public void OnSlideStop()
   {
      _meshRenderer.material.SetColor("_BaseColor", Color.white);
   }

   public void UpdateSlider(float percent)
   {
      transform.position = Vector3.Lerp(StartPosition.position, EndPosition.position, percent);
   }
}
