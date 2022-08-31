using UnityEngine;

public abstract class VrGrabObject : MonoBehaviour
{
    protected Transform originParent;
    protected bool grabbed;

    protected virtual void Start()
    {
        this.originParent = transform.parent;
    }

    public void Grab(Transform newParent)
    {
        if (this.grabbed && newParent == transform.parent) return;

        transform.parent = newParent;

        this.grabbed = true;

        OnGrab();
    }

    public void Release()
    {
        if (!this.grabbed) return;

        transform.parent = this.originParent;

        this.grabbed = false;
        
        OnRelease();
    }

    protected abstract void OnGrab();

    protected abstract void OnRelease();
}