using System.Collections;
using UnityEngine;

namespace Idea_2.InputBlocks
{
    /// <summary>
    /// Moves the key in the place direction a distance of one.
    /// </summary>
    public class SimpleDirectionalBlock : InputBlock
    {
        public override IEnumerator TriggerInput(GridKey key, float timePerBlock)
        {
            key.transform.position = this.inputBoard.gridTransforms[this.id.x][this.id.y].position;
         
            float t = 0;
            Vector2Int idToMoveTo = new Vector2Int(this.id.x + this.idDir.x, this.id.y + this.idDir.y);

            Vector3 dir = this.UpDir() * this.idDir.y + this.RightDir() * this.idDir.x;

            while (t < timePerBlock)
            {
                t += Time.deltaTime;

                key.transform.position += dir * (1 / timePerBlock * Time.deltaTime);

                yield return null;
            }

            key.previousDirection = this.placeDirection;

            this.inputBoard.Trigger(key, idToMoveTo);
        }
    }
}