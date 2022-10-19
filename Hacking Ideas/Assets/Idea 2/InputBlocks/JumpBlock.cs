using System.Collections;
using UnityEngine;

namespace Idea_2.InputBlocks
{
    public class JumpBlock : InputBlock
    {
        public override IEnumerator TriggerInput(GridKey key, float timePerBlock)
        {
            Transform keyTransform = key.transform;
            keyTransform.position = this.inputBoard.gridTransforms[this.id.x][this.id.y].position;

            float t = 0;
            Vector2Int idToMoveTo = this.id + idDir * 2;

            Vector3 keySize = keyTransform.localScale;
            Vector3 scaleDir = keySize / (timePerBlock / 2);

            while (t < timePerBlock / 2)
            {
                t += Time.deltaTime;

                keyTransform.localScale -= scaleDir * Time.deltaTime;

                yield return null;
            }

            keyTransform.position = keyTransform.position +
                                    RightDir() * (idDir * 2).x +
                                    UpDir() * (idDir * 2).y;

            while (t < timePerBlock)
            {
                t += Time.deltaTime;

                keyTransform.localScale += scaleDir * Time.deltaTime;

                yield return null;
            }

            keyTransform.localScale = keySize;

            key.previousDirection = this.placeDirection;

            this.inputBoard.Trigger(key, idToMoveTo);
        }
    }
}