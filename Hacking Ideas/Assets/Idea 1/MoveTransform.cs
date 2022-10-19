using UnityEngine;

namespace Idea_1
{
    /// <summary>
    /// Used for controlling the direction of the key on the line.
    /// In game it is an arrow.
    /// </summary>
    public class MoveTransform : MonoBehaviour
    {
        #region Values

        [SerializeField] private Transform rightHand;

        #endregion

        /// <summary>
        /// Updates the arrows position and rotation.
        /// </summary>
        private void Update()
        {
            Transform moveTransform = transform;
            Vector3 handForward = this.rightHand.forward;
            moveTransform.position = this.rightHand.position + handForward * .25f;

            transform.LookAt(moveTransform.position + handForward, this.rightHand.up);
        }
    }
}