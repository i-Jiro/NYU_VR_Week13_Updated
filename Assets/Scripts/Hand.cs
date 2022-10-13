using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public enum HandType
{
    Left,
    Right
}

public class Hand : MonoBehaviour
{
    public HandType HandType = HandType.Left;
    public bool IsHidden { get; private set; }
    public bool IsCollisionEnabled { get; private set; } = false;

    public InputAction trackedAction = null;
    public InputAction gripAction;
    public InputAction triggerAction;

    public Animator animator;
    private int _gripAmountParameter = 0;
    private int _pointAmountParameter = 0;
    
    private bool _isCurrentlyTracked = false;
    private List<Renderer> _currentRenderers = new List<Renderer>();
    private Collider[] _colliders = null;
    private XRBaseInteractor _interactor = null;

    private void Awake()
    {
        _interactor = GetComponentInParent<XRBaseInteractor>();
    }

    //_interactor.onSelectEnter and the likes is deprecated. Use _interactor.selectEntered instead.
    //Listeners methods however must take an EventArgs parameter now instead of the XRBaseInteractable.
    //Goto line 127 to see the changes there.
    private void OnEnable()
    {
        _interactor.selectEntered.AddListener(OnGrab);
        _interactor.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        _interactor.selectEntered.RemoveListener(OnGrab);
        _interactor.selectExited.RemoveListener(OnRelease);
    }

    // Start is called before the first frame update
    void Start()
    {
        
        _colliders = GetComponentsInChildren<Collider>().Where(childCollider => !childCollider.isTrigger).ToArray();
        trackedAction.Enable();
        gripAction.Enable();
        triggerAction.Enable();
        _gripAmountParameter = Animator.StringToHash("GripAmount");
        _pointAmountParameter = Animator.StringToHash("PointAmount");
        Hide();
    }

    void UpdateAnimation()
    {
        float gripAmount = gripAction.ReadValue<float>();
        float pointAmount = triggerAction.ReadValue<float>();
        animator.SetFloat(_gripAmountParameter, Mathf.Clamp01(gripAmount + pointAmount));
        animator.SetFloat(_pointAmountParameter, pointAmount);
    }

    // Update is called once per frame
    void Update()
    {
        float isTracked = trackedAction.ReadValue<float>();
        if(isTracked == 1.0f && !_isCurrentlyTracked)
        {
            Show();
            _isCurrentlyTracked = true;
        }
        else if (isTracked == 0.0f && _isCurrentlyTracked)
        {
            Hide();
            _isCurrentlyTracked = false;
        }
        UpdateAnimation();
    }

    public void Show()
    {
        foreach (var renderer in _currentRenderers)
        {
            renderer.enabled = true;
        }
        EnableCollisions(true);
        IsHidden = false;
    }

    public void Hide()
    {
        _currentRenderers.Clear();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = false;
            _currentRenderers.Add(renderer);
        }
        EnableCollisions(false);
        IsHidden = true;
    }

    public void EnableCollisions(bool state)
    {
        if (IsCollisionEnabled == state) return;
        IsCollisionEnabled = state;
        foreach(var collider in _colliders)
        {
            collider.enabled = IsCollisionEnabled;
        }
    }

    //Toolkit 2.20 passes EventsArgs to event callbacks instead of XRBaseInteractable.
    //Refer to the API manual for XR Toolkit 2.20 to see which EventArgs to use.
    //To access the gameObject, simply add .transform. Ie: args.interactorObject.transform.gameObject.GetComponent<Rigidbody>()
    private void OnGrab(SelectEnterEventArgs args)
    {
        HandControl ctrl = args.interactableObject.transform.GetComponent<HandControl>();
        if (ctrl != null)
        {
            if (ctrl.HideHand)
            {
                Hide();
            }
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        HandControl ctrl = args.interactableObject.transform.GetComponent<HandControl>();
        if (ctrl != null)
        {
            if (ctrl.HideHand)
            {
                Show();
            }
        }
    }
}
