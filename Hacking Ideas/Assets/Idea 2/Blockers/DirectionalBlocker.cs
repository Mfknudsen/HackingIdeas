using System.Collections;
using UnityEngine;

namespace Idea_2.Blockers
{
    public class DirectionalBlocker : Blocker
    {
        public override IEnumerator Trigger(GridKey key, float timePerBlock, VisualGrid visualGrid,
            InputBoard inputBoard)
        {
            if (key.previousDirection == placeDirection)
            {
                key.Reset();
                yield break;
            }

            Vector2Int keyDir = key.previousDirection switch
            {
                PlaceDirection.MinusX => new Vector2Int(-1, 0),
                PlaceDirection.MinusY => new Vector2Int(0, -1),
                PlaceDirection.PlusX => new Vector2Int(1, 0),
                _ => new Vector2Int(0, 1)
            };

            float t = 0;
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

            inputBoard.Trigger(key, idToMoveTo);
        }
    }
}