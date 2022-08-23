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
        public VisualGrid visualGrid;
        public PlaceDirection previousDirection;

        private Vector2Int originID;

        private void Move(Vector2Int newID) => inputBoard.Trigger(this, newID);

        public void TriggerFirst()
        {
            if (!ready)
                return;

            originID = id;
            ready = false;

            StartCoroutine(MoveToFirst());
        }

        public void Reset()
        {
            this.id = this.originID;

            this.ready = true;

            Vector2Int checkFrom = new Vector2Int(
                Mathf.Clamp(id.x, 0, this.visualGrid.gridTransforms.Count - 1),
                Mathf.Clamp(id.y, 0, this.visualGrid.gridTransforms[0].Count() - 1));

            Vector3 pos = this.visualGrid.gridTransforms[checkFrom.x][checkFrom.y].position;

            Vector3 upDir = this.visualGrid.gridTransforms[0][1].position -
                            this.visualGrid.gridTransforms[0][0].position,
                rightDir = this.visualGrid.gridTransforms[1][0].position -
                           this.visualGrid.gridTransforms[0][0].position;

            Vector2Int extraDir = this.id - checkFrom;
            transform.position = pos + upDir * extraDir.y + rightDir * extraDir.x;
        }

        private IEnumerator MoveToFirst()
        {
            float t = 0;
            Vector2Int idToMoveTo = new Vector2Int(
                Mathf.Clamp(this.id.x, 0, this.visualGrid.gridTransforms.Count - 1),
                Mathf.Clamp(this.id.y, 0, this.visualGrid.gridTransforms[0].Count() - 1));
            
            Vector3 dir = this.visualGrid.gridTransforms[idToMoveTo.x][idToMoveTo.y].position - transform.position;

            while (t < this.timePerBlock)
            {
                t += Time.deltaTime;

                transform.position += dir * (1 / this.timePerBlock * Time.deltaTime);

                yield return null;
            }

            Move(idToMoveTo);
        }

        public void ShuffleStartPosition(VisualGrid visual)
        {
            this.visualGrid = visual;

            int xSize = this.visualGrid.gridTransforms.Count,
                ySize = this.visualGrid.gridTransforms[0].Count();
            Vector2Int keyStartID = new Vector2Int(
                Random.Range(0, xSize),
                Random.Range(0, ySize));

            keyStartID = Random.Range(0f, 1f) < .5f
                ? new Vector2Int(
                    keyStartID.x,
                    Random.Range(0f, 1f) < .5f ? -1 : ySize)
                : new Vector2Int(
                    Random.Range(0f, 1f) < .5f ? -1 : xSize,
                    keyStartID.y);

            Vector2Int extraDir = new Vector2Int(
                keyStartID.x == -1 ? -1 :
                keyStartID.x == xSize ? 1 :
                0,
                keyStartID.y == -1 ? -1 :
                keyStartID.y == ySize ? 1 :
                0
            );

            this.id = keyStartID;

            Vector2Int checkFrom = new Vector2Int(
                Mathf.Clamp(keyStartID.x, 0, xSize - 1),
                Mathf.Clamp(keyStartID.y, 0, ySize - 1));

            Vector3 pos = this.visualGrid.gridTransforms[0][0].position;
            Vector3 upDir = this.visualGrid.gridTransforms[0][1].position - pos,
                rightDir = this.visualGrid.gridTransforms[1][0].position - pos;

            transform.position = this.visualGrid.gridTransforms[checkFrom.x][checkFrom.y].position +
                                 upDir * extraDir.y + rightDir * extraDir.x;
        }
    }
}