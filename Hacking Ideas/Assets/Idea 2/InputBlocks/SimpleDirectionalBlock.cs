using System.Collections;
using UnityEngine;

namespace Idea_2.InputBlocks
{
    public class SimpleDirectionalBlock : InputBlock
    {
        public override IEnumerator TriggerInput(GridKey key, float timePerBlock, VisualGrid visualGrid)
        {
            this.t = 0;
            Vector2Int idToMoveTo = new Vector2Int(this.id.x +this. idDir.x, this.id.y +this. idDir.y);

            Vector3 up = visualGrid.gridTransforms[0, 1].position - visualGrid.gridTransforms[0, 0].position,
                right = visualGrid.gridTransforms[1, 0].position - visualGrid.gridTransforms[0, 0].position;
            Vector3 dir = up * this.idDir.y + right * this.idDir.x;
            
            while (this.t < timePerBlock)
            {
                this.t += Time.deltaTime;

                key.transform.position += dir * (1 / timePerBlock * Time.deltaTime);

                yield return null;
            }

            this.inputBoard.Trigger(key, idToMoveTo);
        }
    }
}