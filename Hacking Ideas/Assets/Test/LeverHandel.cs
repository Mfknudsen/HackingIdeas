#region Systems

using UnityEngine;

#endregion

namespace Test
{
    public class LeverHandel : VrGrabObject 
    {

        public Transform toReturnTo; //Where the handel returns when not being held.

        public Transform parentOfObject; //The original parent of this object.


        protected override void Start()
        {
            base.Start();
            
            parentOfObject = transform.parent; //Get the parent transform.
        }

        protected override void OnRelease() //When no longer being held.
        {
            transform.position = toReturnTo.position; //Return to the top of the lever.

            transform.SetParent(parentOfObject); //Return af a child to the original parent.
        }

        protected override void OnGrab()
        {
        }
    }
}