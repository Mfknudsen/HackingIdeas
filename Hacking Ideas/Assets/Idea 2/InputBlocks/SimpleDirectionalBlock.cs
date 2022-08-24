using System.Collections;
using UnityEngine;

namespace Idea_2.InputBlocks
{
    public class SimpleDirectionalBlock : InputBlock
    {
        public override IEnumerator TriggerInput(GridKey key, float timePerBlock, VisualGrid visualGrid)
        {
            key.transform.position = visualGrid.gridTransforms[id.x][id.y].position;
         
            float t = 0;
            Vector2Int idToMoveTo = new Vector2Int(this.id.x + this.idDir.x, this.id.y + this.idDir.y);

            Vector3 dir = UpDir(visualGrid) * this.idDir.y + RightDir(visualGrid) * this.idDir.x;

            while (t < timePerBlock)
            {
                t += Time.deltaTime;

                key.transform.position += dir * (1 / timePerBlock * Time.deltaTime);

                yield return null;
            }

            key.previousDirection = placeDirection;

            this.inputBoard.Trigger(key, idToMoveTo);
        }
    }
}