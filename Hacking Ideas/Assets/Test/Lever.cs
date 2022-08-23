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
            minRot = Quaternion.Euler(-45, 0, 0);
            maxRot = Quaternion.Euler(-45, 180, 0);

            //Setting the levers rotation based on its start active state.
            if (active == true)
            {
                joint.transform.localRotation = maxRot;
            }
            else
            {
                joint.transform.localRotation = minRot;
            }

            handel.transform.position = top.transform.position; //Placing the handel on the top.
        }

        private void Update()
        {
            //Updating the different parts of the script.
            RotateTowardsHandel();
            CountPercent();
            IsActiveOrNot();
            LockHandelTransform();
        }

        private void RotateTowardsHandel() //Rotating the handel.
        {
            targetTransform.position =
                handel.transform
                    .position; //Due to VR changing the parent of the object being held. Therefor a second transform is used when using localPosition. Matching their global position.

            //Making two temp vector3's out of local position values and removing the x value so the lever wont rotate around that axis later in the code.
            Vector3 tempHandel = targetTransform.localPosition;
            tempHandel.x = 0;
            Vector3 tempJoint = joint.transform.localPosition;
            tempJoint.x = 0;

            lookDirection =
                tempHandel - tempJoint; //The lookDirection is based on the vector between the joint and the top.

            if (!(lookDirection.y >= minRot.eulerAngles.y)) return; //If the new vector3 is not beyond the min rotation.

            targetRotation =
                Quaternion.LookRotation(lookDirection); //The new rotation is made based on the new vector3.

            joint.transform.localRotation = targetRotation; //Moving the rotation of the lever.

            lastHandelPosition = handel.transform.position; //Remembering where the handel were during this rotation.

            if (targetRotation.y == 0 && targetRotation.z == 0)
            {
                if (targetRotation.eulerAngles.x > minRot.eulerAngles.x)
                    joint.transform.localRotation =
                        minRot; //If the new rotation is beyond the min rotation then it is returned to the min rotation.
            }
            else if (targetRotation.x == 0 && targetRotation.w == 0)
            {
                if (targetRotation.eulerAngles.x > maxRot.eulerAngles.x)
                    joint.transform.localRotation =
                        maxRot; //If the new rotation is beyond the max rotation then it is returned to the max rotation.
            }
        }

        private void CountPercent() //Counting the placement of the lever between the min and max rotation.
        {
            float tempPercent;
            Quaternion tempRot = joint.transform.localRotation;

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
            progressInProcent =
                Mathf.Ceil(tempPercent);
        }

        private void IsActiveOrNot() //Check when the lever is active.
        {
            if (progressInProcent <= 50)
                active = false;
            else if (progressInProcent > 50)
                active = true;
        }

        private void LockHandelTransform() //Locking the handel position to the top position.
        {
            if (lockHandel) 
                handel.transform.position = top.transform.position;
        }
    }
}