using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VRGrab : MonoBehaviour
{
    #region Values

    private PlayerInput playerInput;
    [SerializeField] private float threshold = .2f;
    [SerializeField] private bool rightHand;

    private bool grabbing;

    private VrGrabObject holdingObj;
    private readonly List<GameObject> objectsInRange = new List<GameObject>();
    
    #endregion

    #region MonoBehaviour

    private void Start()
    {
        this.playerInput = new PlayerInput();
        this.playerInput.Enable();
    }

    private void Update()
    {
        for (int i = objectsInRange.Count - 1; i >= 0; i--)
        {
            if (objectsInRange[i] == null)
                objectsInRange.RemoveAt(i);
        }

        float grib = (this.rightHand
                ? this.playerInput.Player.GrabRight
                : this.playerInput.Player.GrabLeft)
            .ReadValue<float>();

        grib = Mathf.Floor(grib * 10);
        
        if (grib > this.threshold && !this.grabbing)
        {
            this.grabbing = true;
            Grab();
        }
        else if (grib < this.threshold && this.grabbing)
        {
            this.grabbing = false;
            Release();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<VrGrabObject>() != null && !this.objectsInRange.Contains(other.gameObject))
            this.objectsInRange.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (this.objectsInRange.Contains(other.gameObject))
            this.objectsInRange.Remove(other.gameObject);
    }

    #endregion

    private void Grab()
    {
        if (this.objectsInRange.Count == 0)
            return;

        Vector3 pos = transform.position;

        this.holdingObj = this.objectsInRange
            .Where(o => o != null)
            .OrderBy(o => Vector3.Distance(o.transform.position, pos))
            .FirstOrDefault()
            ?.GetComponent<VrGrabObject>();

        if (this.holdingObj == null) return;

        if (this.holdingObj.GetComponentInParent<VRGrab>() is { } g && g != this) g.holdingObj = null;

        this.holdingObj.Grab(transform);
    }

    private void Release()
    {
        if (this.holdingObj == null)
            return;

        this.holdingObj.Release();

        this.holdingObj = null;
    }
}