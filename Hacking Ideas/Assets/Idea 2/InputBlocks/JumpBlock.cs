using System.Collections;
using UnityEngine;

namespace Idea_2.InputBlocks
{
    public class JumpBlock : InputBlock
    {
        public override IEnumerator TriggerInput(GridKey key, float timePerBlock, VisualGrid visualGrid)
        {
            this.t = 0;
            Vector2Int idToMoveTo = id + this.idDir;

            Transform keyTransform = key.transform;
            Vector3 keySize = keyTransform.localScale;
            Vector3 scaleDir = keySize / (timePerBlock / 2);

            while (this.t < timePerBlock / 2)
            {
                this.t += Time.deltaTime;

                keyTransform.localScale -= scaleDir * Time.deltaTime;

                yield return null;
            }

            keyTransform.position = visualGrid.gridTransforms[idToMoveTo.x, idToMoveTo.y].position;

            while (this.t < timePerBlock)
            {
                this.t += Time.deltaTime;

                keyTransform.localScale += scaleDir * Time.deltaTime;

                yield return null;
            }

            keyTransform.localScale = keySize;

            this.    inputBoard.Trigger(key, idToMoveTo);
        }
    }
}