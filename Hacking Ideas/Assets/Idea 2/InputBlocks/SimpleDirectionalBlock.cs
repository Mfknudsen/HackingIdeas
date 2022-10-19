using System.Collections;
using UnityEngine;

namespace Idea_2.InputBlocks
{
    public class SimpleDirectionalBlock : InputBlock
    {
        public override IEnumerator TriggerInput(GridKey key, float timePerBlock)
        {
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

            this.inputBoard.Trigger(key, idToMoveTo);
        }
    }
}