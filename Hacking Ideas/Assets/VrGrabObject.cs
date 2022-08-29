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
        transform.parent = newParent;
        
        OnGrab();

        grabbed = true;
    }

    public void Release()
    {
        transform.parent = originParent;
        
        OnRelease();

        grabbed = false;
    }

    protected abstract void OnGrab();

    protected abstract void OnRelease();
}
