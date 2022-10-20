using System.Collections;
using UnityEngine;

namespace Idea_2.InputBlocks
{
    /// <summary>
    /// Moves the key in the place direction a distance of one.
    /// After starting thew move of one key it will switch direction to that opposite of it's current.
    /// This will happen every time the block moves one key.
    /// </summary>
    public class SwitchBlock : InputBlock
    {
        private bool switched;

        public override IEnumerator TriggerInput(GridKey key, float timePerBlock)
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

            this.inputBoard.Trigger(key, idToMoveTo);
        }

        public override void Reset()
        {
            if (!this.switched)
                return;

            this.switched = false;

            Transform trans = this.transform;
            this.transform.LookAt(trans.position - trans.forward, trans.up);
        }
    }
}