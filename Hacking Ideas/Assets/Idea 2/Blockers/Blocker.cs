using System.Collections;
using UnityEngine;

namespace Idea_2.Blockers
{
    public class Blocker : MonoBehaviour
    {
        public PlaceDirection placeDirection;
        public Vector2Int id;

        private Vector2Int idDir => this.placeDirection switch
        {
            PlaceDirection.MinusX => new Vector2Int(-1, 0),
            PlaceDirection.MinusY => new Vector2Int(0, -1),
            PlaceDirection.PlusX => new Vector2Int(1, 0),
            _ => new Vector2Int(0, 1)
        };
        
        public virtual IEnumerator Trigger(GridKey key, float timePerBlock, VisualGrid visualGrid, InputBoard inputBoard)
        {
            float t = 0;
            Vector2Int idToMoveTo = new Vector2Int(this.id.x + idDir.x, this.id.y + idDir.y);

            Vector3 up = visualGrid.gridTransforms[0][1].position - visualGrid.gridTransforms[0][0].position,
                right = visualGrid.gridTransforms[1][0].position - visualGrid.gridTransforms[0][0].position;
            Vector3 dir = up * idDir.y + right * idDir.x;

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