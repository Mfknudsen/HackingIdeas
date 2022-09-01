using System.Collections;
using UnityEngine;

namespace Idea_2.Blockers
{
    public class DirectionalBlocker : Blocker
    {
        public override IEnumerator Trigger(GridKey key, float timePerBlock,
            InputBoard inputBoard)
        {
            key.transform.position = this.inputBoard.gridTransforms[this.id.x][this.id.y].position;

            int keyLength = key.previousDirection.ToString().Length,
                placeLength = this.placeDirection.ToString().Length;

            string cKeyDirCoord = key.previousDirection.ToString().Substring(
                    keyLength - 1, 1),
                cDirCoord = this.placeDirection.ToString().Substring(
                    placeLength - 1, 1);

            string cKeyDir = key.previousDirection.ToString().Substring(0, 
                    keyLength - 1),
                cDir = this.placeDirection.ToString().Substring(0, 
                    placeLength - 1);

            if (cKeyDirCoord == cDirCoord && cKeyDir != cDir)
            {
                this.inputBoard.Reset(key);
                yield break;
            }

            Vector2Int keyDir = key.previousDirection switch
            {
                PlaceDirection.MinusX => new Vector2Int(-1, 0),
                PlaceDirection.MinusY => new Vector2Int(0, -1),
                PlaceDirection.PlusX => new Vector2Int(1, 0),
                _ => new Vector2Int(0, 1)
            };

            float t = 0;
            Vector2Int idToMoveTo = new Vector2Int(this.id.x + keyDir.x, this.id.y + keyDir.y);
            Vector3 dir = UpDir() * keyDir.y + RightDir() * keyDir.x;


            while (t < timePerBlock)
            {
                t += Time.deltaTime;

                key.transform.position += dir * (1 / timePerBlock * Time.deltaTime);

                yield return null;
            }

            key.previousDirection = placeDirection;

            inputBoard.Trigger(key, idToMoveTo);
        }
    }
}