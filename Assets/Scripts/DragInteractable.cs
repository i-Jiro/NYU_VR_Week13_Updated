using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[System.Serializable]
public class DragEvent : UnityEvent<float> { }

public class DragInteractable : XRBaseInteractable
{
    public Transform StartDragPosition = null;
    public Transform EndDragPosition = null;
    //Instead of storing XRInteractable, store the interface IXRInteractor.
    //XRInteractable isn't directly accessible through the their new event callback in XR.toolkit 2.20.
    //See Line 80.
    private IXRInteractor _interactor = null;
    
    [SerializeField]
    private float _dragPercent = 0.0f;
    private Coroutine _drag = null;

    public UnityEvent OnDragStart = new UnityEvent();
    public UnityEvent OnDragEnd = new UnityEvent();
    public DragEvent OnDragUpdate = new DragEvent();
    
    void StartDrag()
    {
        if (_drag != null)
        {
            StopCoroutine(_drag);
        }
        _drag = StartCoroutine(CalculateDrag());
        OnDragStart?.Invoke();
    }

    void EndDrag()
    {
        if (_drag != null)
        {
            StopCoroutine(_drag);
            _drag = null;
        }
        OnDragEnd?.Invoke();
    }

    private float InverseLerp(Vector3 start, Vector3 end, Vector3 currentPosition)
    {
        Vector3 AB = end - start;
        Vector3 AV = currentPosition - start;
        return Mathf.Clamp01(Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB));
    }

    IEnumerator CalculateDrag()
    {
        while(_interactor != null)
        {
            
            //Line vector from A to B.
            Vector3 line = StartDragPosition.localPosition - EndDragPosition.localPosition;
            //Get the interactor's position in local space of the parent transform of the component.
            Vector3 interactorLocalPosition = StartDragPosition.parent.InverseTransformPoint(_interactor.transform.position);
            //Get the positional vector of where the hand is relative to the line vector direction.
            Vector3 projectedPoint = Vector3.Project(interactorLocalPosition, line.normalized);
            //Return value from 0 to 1f depending on where the hand moved while grabbing along the vector line.
            _dragPercent = InverseLerp(StartDragPosition.localPosition, EndDragPosition.localPosition, projectedPoint);
            
            OnDragUpdate?.Invoke(_dragPercent);
            
            yield return null;
        }
        yield return null;
    }
    
    //OnSelectEntered(XRBaseInteractor interactor) or any method that took an XRBaseInteractable/Interactor is deprecated.
    //Instead override methods that take EventArgs parameters.
    //Interactables can be accessed with args.interactableObject and Interactor with args.interactorObject.
    //To access the gameObject, simply add .transform. Ie: args.interactorObject.transform.gameObject.GetComponent<Rigidbody>()
    
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        _interactor = args.interactorObject;
        StartDrag();
        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        EndDrag();
        _interactor = null;
        base.OnSelectExited(args);
    }
}
