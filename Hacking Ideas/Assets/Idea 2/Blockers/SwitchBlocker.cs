using System.Collections;
using UnityEngine;

namespace Idea_2.Blockers
{
    /// <summary>
    /// Moves the key a direction by one then switch direction to the opposite.
    /// Repeats everytime a key triggers it. 
    /// </summary>
    public class SwitchBlocker : Blocker
    {
        private bool switched;

        public override IEnumerator Trigger(GridKey key, float timePerBlock,
            InputBoard board)
        {
            key.transform.position = this.inputBoard.gridTransforms[this.id.x][this.id.y].position;
            
            PlaceDirection switchedDir = this.placeDirection switch
            {
                PlaceDirection.PlusX => this.switched ? PlaceDirection.MinusX : PlaceDirection.PlusX,
                PlaceDirection.MinusX => this.switched ? PlaceDirection.PlusX : PlaceDirection.MinusX,
                PlaceDirection.PlusY => this.switched ? PlaceDirection.MinusY : PlaceDirection.PlusY,
                _ => this.switched ? PlaceDirection.PlusY : PlaceDirection.MinusY,
            };

            Vector2Int keyDir = switchedDir switch
            {
                PlaceDirection.MinusX => new Vector2Int(-1, 0),
                PlaceDirection.MinusY => new Vector2Int(0, -1),
                PlaceDirection.PlusX => new Vector2Int(1, 0),
                _ => new Vector2Int(0, 1)
            };

            this.switched = !this.switched;

            float t = 0;
            Vector2Int idToMoveTo = new Vector2Int(this.id.x + keyDir.x, this.id.y + keyDir.y);
            Vector3 dir = this.UpDir() * keyDir.y + this.RightDir() * keyDir.x;


            while (t < timePerBlock)
            {
                t += Time.deltaTime;

                key.transform.position += dir * (1 / timePerBlock * Time.deltaTime);

                yield return null;
            }

            key.previousDirection = this.placeDirection;
            Transform trans = this.transform;
            this.transform.LookAt(trans.position - trans.forward, trans.up);

            board.Trigger(key, idToMoveTo);
        }

        public override void Reset()
        {
            if (!this.switched)
                return;

            this.switched = false;

            Transform trans = this.transform;
            trans.LookAt(trans.position - trans.forward, trans.up);
        }
    }
}