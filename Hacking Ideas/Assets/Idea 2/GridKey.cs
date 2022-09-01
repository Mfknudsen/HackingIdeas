using System.Collections;
using UnityEngine;

namespace Idea_2
{
    public class GridKey : MonoBehaviour
    {
        public InputBoard inputBoard;
        public Vector2Int id;
        public float timePerBlock = 1;
        public bool ready = true;
        public PlaceDirection previousDirection;
        public Coroutine currentCoroutine;

        private Vector2Int originID;

        private void Move(Vector2Int newID) => inputBoard.Trigger(this, newID);

        public void TriggerFirst()
        {
            if (!this.ready)
                return;

            this.originID = this.id;
            this.ready = false;

            this.currentCoroutine = StartCoroutine(MoveToFirst());
        }

        public void Reset()
        {
            this.id = this.originID;

            gameObject.SetActive(true);

            this.ready = true;

            Vector2Int checkFrom = new Vector2Int(
                Mathf.Clamp(this.id.x, 0, this.inputBoard.gridTransforms.Count - 1),
                Mathf.Clamp(this.id.y, 0, this.inputBoard.gridTransforms[0].Count - 1));

            Vector3 pos = this.inputBoard.gridTransforms[checkFrom.x][checkFrom.y].position;

            Vector3 upDir = this.inputBoard.gridTransforms[0][1].position -
                            this.inputBoard.gridTransforms[0][0].position,
                rightDir = this.inputBoard.gridTransforms[1][0].position -
                           this.inputBoard.gridTransforms[0][0].position;

            Vector2Int extraDir = this.id - checkFrom;
            transform.position = pos + upDir * extraDir.y + rightDir * extraDir.x;
        }

        private IEnumerator MoveToFirst()
        {
            float t = 0;
            Vector2Int idToMoveTo = new Vector2Int(
                Mathf.Clamp(this.id.x, 0, this.inputBoard.gridTransforms.Count - 1),
                Mathf.Clamp(this.id.y, 0, this.inputBoard.gridTransforms[0].Count - 1));

            Vector3 dir = this.inputBoard.gridTransforms[idToMoveTo.x][idToMoveTo.y].position - transform.position;

            while (t < this.timePerBlock)
            {
                t += Time.deltaTime;

                transform.position += dir * (1 / this.timePerBlock * Time.deltaTime);

                yield return null;
            }

            Move(idToMoveTo);
        }
    }
}