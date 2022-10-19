using System.Collections;
using UnityEngine;

namespace Idea_2.Blockers
{
    public class Blocker : MonoBehaviour
    {
        public InputBoard inputBoard;
        public PlaceDirection placeDirection;
        public Vector2Int id;

        private Vector2Int idDir => this.placeDirection switch
        {
            PlaceDirection.MinusX => new Vector2Int(-1, 0),
            PlaceDirection.MinusY => new Vector2Int(0, -1),
            PlaceDirection.PlusX => new Vector2Int(1, 0),
            _ => new Vector2Int(0, 1)
        };

        protected Vector3 RightDir() =>
            this.inputBoard.gridTransforms[1][0].position - this.inputBoard.gridTransforms[0][0].position;

        protected Vector3 UpDir() =>
            this.inputBoard.gridTransforms[0][1].position - this.inputBoard.gridTransforms[0][0].position;

        public virtual IEnumerator Trigger(GridKey key, float timePerBlock, InputBoard inputBoard)
        {
            this.inputBoard = inputBoard;
            key.transform.position = this.inputBoard.gridTransforms[this.id.x][this.id.y].position;

            float t = 0;
            Vector2Int idToMoveTo = new Vector2Int(this.id.x + idDir.x, this.id.y + idDir.y);

            Vector3 dir = UpDir() * idDir.y + RightDir() * idDir.x;

            while (t < timePerBlock)
            {
                t += Time.deltaTime;

                key.transform.position += dir * (1 / timePerBlock * Time.deltaTime);

                yield return null;
            }

            key.previousDirection = this.placeDirection;

            inputBoard.Trigger(key, idToMoveTo);
        }

        public virtual void Reset()
        {
        }
    }
}