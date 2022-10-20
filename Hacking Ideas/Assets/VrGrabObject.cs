using UnityEngine;

public abstract class VrGrabObject : MonoBehaviour
{
    protected Transform originParent;
    protected bool grabbed;

    protected virtual void Start() => 
        this.originParent = this.transform.parent;

    public void Grab(Transform newParent)
    {
        if (this.grabbed && newParent == this.transform.parent) return;

        this.transform.parent = newParent;

        this.grabbed = true;

        this.OnGrab();
    }

    public void Release()
    {
        if (!this.grabbed) return;

        this.transform.parent = this.originParent;

        this.grabbed = false;

        this.OnRelease();
    }

    protected abstract void OnGrab();

    protected abstract void OnRelease();
}