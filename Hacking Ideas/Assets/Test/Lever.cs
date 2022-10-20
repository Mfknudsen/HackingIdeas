#region Systems

using UnityEngine;

#endregion

namespace Test
{
    public class Lever : MonoBehaviour
    {
        #region public DATA

        [Header("Required Input")] public GameObject joint; //The point to rotate around.
        public GameObject handel; //The handel that the player can hold onto.
        public Transform top; //The top of the lever.
        [HideInInspector] public bool active; //If the lever is active or not.
        [HideInInspector] public float progressInProcent; //The levers active state based in procent.
        public Transform targetTransform; //The target point to rotate towards.

        #endregion

        #region private DATA

        private Quaternion maxRot, minRot; //Boundaries of the lever.
        private Vector3 lookDirection; //Vector 3 used for rotation.
        private Quaternion targetRotation; //The new rotation based on the lookDirection.
        private Vector3 lastHandelPosition; //Where the handel were last frame.
        private bool lockHandel = false; //If the handel is locked to the position of the top.

        #endregion

        private void Start()
        {
            //Setting the min and max rotations based on vector3's.
            this.minRot = Quaternion.Euler(-45, 0, 0);
            this.maxRot = Quaternion.Euler(-45, 180, 0);

            //Setting the levers rotation based on its start active state.
            if (this.active == true)
            {
                this.joint.transform.localRotation = this.maxRot;
            }
            else
            {
                this.joint.transform.localRotation = this.minRot;
            }

            this.handel.transform.position = this.top.transform.position; //Placing the handel on the top.
        }

        private void Update()
        {
            //Updating the different parts of the script.
            this.RotateTowardsHandel();
            this.CountPercent();
            this.IsActiveOrNot();
            this.LockHandelTransform();
        }

        private void RotateTowardsHandel() //Rotating the handel.
        {
            this.targetTransform.position = this.handel.transform
                    .position; //Due to VR changing the parent of the object being held. Therefor a second transform is used when using localPosition. Matching their global position.

            //Making two temp vector3's out of local position values and removing the x value so the lever wont rotate around that axis later in the code.
            Vector3 tempHandel = this.targetTransform.localPosition;
            tempHandel.x = 0;
            Vector3 tempJoint = this.joint.transform.localPosition;
            tempJoint.x = 0;

            this.lookDirection =
                tempHandel - tempJoint; //The lookDirection is based on the vector between the joint and the top.

            if (!(this.lookDirection.y >= this.minRot.eulerAngles.y)) return; //If the new vector3 is not beyond the min rotation.

            this.targetRotation =
                Quaternion.LookRotation(this.lookDirection); //The new rotation is made based on the new vector3.

            this.joint.transform.localRotation = this.targetRotation; //Moving the rotation of the lever.

            this.lastHandelPosition = this.handel.transform.position; //Remembering where the handel were during this rotation.

            if (this.targetRotation.y == 0 && this.targetRotation.z == 0)
            {
                if (this.targetRotation.eulerAngles.x > this.minRot.eulerAngles.x)
                    this.joint.transform.localRotation = this.minRot; //If the new rotation is beyond the min rotation then it is returned to the min rotation.
            }
            else if (this.targetRotation.x == 0 && this.targetRotation.w == 0)
            {
                if (this.targetRotation.eulerAngles.x > this.maxRot.eulerAngles.x)
                    this.joint.transform.localRotation = this.maxRot; //If the new rotation is beyond the max rotation then it is returned to the max rotation.
            }
        }

        private void CountPercent() //Counting the placement of the lever between the min and max rotation.
        {
            float tempPercent;
            Quaternion tempRot = this.joint.transform.localRotation;

            //Getting the angel of the lever.
            if (tempRot.eulerAngles.y != 0)
                tempPercent = 540 - (360 - tempRot.eulerAngles.x) * 4;
            else
                tempPercent = -(180 - (360 - tempRot.eulerAngles.x) * 4);

            //Converting the angel to procent. 360 degress = 100 procent.
            tempPercent /= 3.6f;

            //Making sure the current procent is not less then 0 or higher then 100.
            if (tempPercent < 0)
                tempPercent = 0;
            else if (tempPercent > 100)
                tempPercent = 100;

            //Getting the closest highest int number to the current procent. 100 -> 100, 23,4 -> 24.
            this.progressInProcent =
                Mathf.Ceil(tempPercent);
        }

        private void IsActiveOrNot() //Check when the lever is active.
        {
            if (this.progressInProcent <= 50)
                this.active = false;
            else if (this.progressInProcent > 50) this.active = true;
        }

        private void LockHandelTransform() //Locking the handel position to the top position.
        {
            if (this.lockHandel) this.handel.transform.position = this.top.transform.position;
        }
    }
}