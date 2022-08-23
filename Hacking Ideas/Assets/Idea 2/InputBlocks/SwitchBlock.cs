using System.Collections;
using UnityEngine;

namespace Idea_2.InputBlocks
{
    public class SwitchBlock : InputBlock
    {
        private bool switched;

        public override IEnumerator TriggerInput(GridKey key, float timePerBlock, VisualGrid visualGrid)
        {
            PlaceDirection switchedDir = this.placeDirection switch
            {
                PlaceDirection.PlusX => switched ? PlaceDirection.MinusX : PlaceDirection.PlusX,
                PlaceDirection.MinusX => switched ? PlaceDirection.PlusX : PlaceDirection.MinusX,
                PlaceDirection.PlusY => switched ? PlaceDirection.MinusY : PlaceDirection.PlusY,
                _ => switched ? PlaceDirection.PlusY : PlaceDirection.MinusY,
            };

            Vector2Int keyDir = switchedDir switch
            {
                PlaceDirection.MinusX => new Vector2Int(-1, 0),
                PlaceDirection.MinusY => new Vector2Int(0, -1),
                PlaceDirection.PlusX => new Vector2Int(1, 0),
                _ => new Vector2Int(0, 1)
            };

            switched = !switched;

            t = 0;
            Vector2Int idToMoveTo = new Vector2Int(this.id.x + keyDir.x, this.id.y + keyDir.y);

            Vector3 up = visualGrid.gridTransforms[0][1].position - visualGrid.gridTransforms[0][0].position,
                right = visualGrid.gridTransforms[1][0].position - visualGrid.gridTransforms[0][0].position;
            Vector3 dir = up * keyDir.y + right * keyDir.x;

            while (t < timePerBlock)
            {
                t += Time.deltaTime;

                key.transform.position += dir * (1 / timePerBlock * Time.deltaTime);

                yield return null;
            }

            key.previousDirection = placeDirection;
            Transform trans = transform;
            transform.LookAt(trans.position - trans.forward, trans.up);

            inputBoard.Trigger(key, idToMoveTo);
        }
    }
}