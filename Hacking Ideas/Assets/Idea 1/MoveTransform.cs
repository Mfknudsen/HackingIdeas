using UnityEngine;

namespace Idea_1
{
    public class MoveTransform : MonoBehaviour
    {
        #region Values

        [SerializeField] private Transform rightHand;

        #endregion

        private void Update()
        {
            transform.position = this.rightHand.position + this.rightHand.forward * .25f;

            transform.LookAt(transform.position + this.rightHand.forward, this.rightHand.up);
        }
    }
}
