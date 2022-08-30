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
        if (this.grabbed) return;

        transform.parent = newParent;

        OnGrab();

        this.grabbed = true;
    }

    public void Release()
    {
        if (!this.grabbed) return;

        transform.parent = this.originParent;

        OnRelease();

        this.grabbed = false;
    }

    protected abstract void OnGrab();

    protected abstract void OnRelease();
}