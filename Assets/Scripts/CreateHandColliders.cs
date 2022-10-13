using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateHandColliders : MonoBehaviour
{
        private struct FingerBone
    {
        public readonly float Radius;
        public readonly float Height;
        public FingerBone(float radius, float height)
        {
            Radius = radius;
            Height = height;
        }

        public Vector3 GetCenter(bool isLeftHand)
        {
            return new Vector3(((isLeftHand) ? -1 : 1) * Height / 2.0f, 0, 0);
        }
    };

    private readonly FingerBone Phalanges = new FingerBone(0.01f, 0.03f);
    private readonly FingerBone Metacarpals = new FingerBone(0.01f, 0.05f);

    public Transform[] bones;
    public GameObject hand;

    void Awake()
    {
        bones = hand.GetComponentsInChildren<Transform>();
        SetHandCols();
    }

    void SetHandCols()
    {
        foreach (Transform bone in bones)
        {
            if (!bone.name.Contains("ignore"))
            {
                CreateCollider(bone);
            }
        }
    }

    private void CreateCollider(Transform transform)
    {
        if (!transform.gameObject.GetComponent(typeof(CapsuleCollider)) && !transform.gameObject.GetComponent(typeof(SphereCollider)) && transform.name.Contains("hands"))
        {
            if (transform.name.Contains("thumb") || transform.name.Contains("index") || transform.name.Contains("middle") || transform.name.Contains("ring") || transform.name.Contains("pinky"))
            {
                if (!transform.name.EndsWith("0"))
                {
                    CapsuleCollider collider = transform.gameObject.AddComponent<CapsuleCollider>();
                    if (!transform.name.EndsWith("1"))
                    {
                        collider.radius = Phalanges.Radius;
                        collider.height = Phalanges.Height;
                        collider.center = Phalanges.GetCenter(transform.name.Contains("_l_"));
                        collider.direction = 0;
                    }
                    else
                    {
                        collider.radius = Metacarpals.Radius;
                        collider.height = Metacarpals.Height;
                        collider.center = Metacarpals.GetCenter(transform.name.Contains("_l_"));
                        collider.direction = 0;
                    }
                }
            }
            else if (transform.name.Contains("grip"))
            {
                SphereCollider collider = transform.gameObject.AddComponent<SphereCollider>();
                collider.radius = 0.04f;
                collider.center = new Vector3(((transform.name.Contains("_l_")) ? -1 : 1) * 0.01f, 0.01f, 0.02f);
            }
        }
    }
}
