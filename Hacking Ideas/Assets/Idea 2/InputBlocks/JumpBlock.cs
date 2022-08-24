using System.Collections;
using UnityEngine;

namespace Idea_2.InputBlocks
{
    public class JumpBlock : InputBlock
    {
        public override IEnumerator TriggerInput(GridKey key, float timePerBlock, VisualGrid visualGrid)
        {
            Transform keyTransform = key.transform;
            keyTransform.position = visualGrid.gridTransforms[id.x][id.y].position;

            float t = 0;
            Vector2Int idToMoveTo = id + this.idDir * 2;

            Vector3 keySize = keyTransform.localScale;
            Vector3 scaleDir = keySize / (timePerBlock / 2);

            while (t < timePerBlock / 2)
            {
                t += Time.deltaTime;

                keyTransform.localScale -= scaleDir * Time.deltaTime;

                yield return null;
            }

            keyTransform.position = keyTransform.position +
                                    RightDir(visualGrid) * (this.idDir * 2).x +
                                    UpDir(visualGrid) * (this.idDir * 2).y;

            while (t < timePerBlock)
            {
                t += Time.deltaTime;

                keyTransform.localScale += scaleDir * Time.deltaTime;

                yield return null;
            }

            keyTransform.localScale = keySize;

            key.previousDirection = placeDirection;

            this.inputBoard.Trigger(key, idToMoveTo);
        }
    }
}