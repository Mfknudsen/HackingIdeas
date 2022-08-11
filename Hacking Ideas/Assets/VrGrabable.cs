using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VrGrabable : MonoBehaviour
{
    protected Transform originParent;

    private void Start()
    {
        this.originParent = transform.parent;
    }

    public void Grab(Transform newParent)
    {
        transform.parent = newParent;
        
        OnGrab();
    }

    public void Release()
    {
        transform.parent = originParent;
        
        OnRelease();
    }

    protected abstract void OnGrab();

    protected abstract void OnRelease();
}
