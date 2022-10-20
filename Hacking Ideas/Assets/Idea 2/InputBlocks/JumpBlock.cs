using System.Collections;
using UnityEngine;

namespace Idea_2.InputBlocks
{
    /// <summary>
    /// Input block move the key a distance of two in the place distance.
    /// The move ignore the block in between start and end.
    /// Allows the key to jump over one block.
    /// </summary>
    public class JumpBlock : InputBlock
    {
        public override IEnumerator TriggerInput(GridKey key, float timePerBlock)
        {
            Transform keyTransform = key.transform;
            keyTransform.position = this.inputBoard.gridTransforms[this.id.x][this.id.y].position;

            float t = 0;
            Vector2Int idToMoveTo = this.id + this.idDir * 2;

            Vector3 keySize = keyTransform.localScale;
            Vector3 scaleDir = keySize / (timePerBlock / 2);

            while (t < timePerBlock / 2)
            {
                t += Time.deltaTime;

                keyTransform.localScale -= scaleDir * Time.deltaTime;

                yield return null;
            }

            keyTransform.position = keyTransform.position + this.RightDir() * (this.idDir * 2).x + this.UpDir() * (this.idDir * 2).y;

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