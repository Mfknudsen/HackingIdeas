using System.Collections;
using UnityEngine;

namespace Idea_2
{
    /// <summary>
    /// The keys the player need to direction to the "EndGaol".
    /// When all keys reach the goal the player wins.
    /// </summary>
    public class GridKey : MonoBehaviour
    {
        /// <summary>
        /// The input board will control the keys movement once started through the different types of blocks.
        /// </summary>
        public InputBoard inputBoard;

        /// <summary>
        /// Its current id on the grid.
        /// </summary>
        public Vector2Int id;

        /// <summary>
        /// How many seconds this key uses per block
        /// </summary>
        public float timePerBlock = 1;

        /// <summary>
        /// If this key is ready to start moving.
        /// </summary>
        public bool ready = true;

        /// <summary>
        /// The direction it came from when entering a in grid block. 
        /// </summary>
        public PlaceDirection previousDirection;

        /// <summary>
        /// The current action of an block on the grid used to move the key.
        /// </summary>
        public Coroutine currentCoroutine;

        /// <summary>
        /// The start id used to set the start position of the key.
        /// </summary>
        private Vector2Int originID;

        /// <summary>
        /// Start the keys movement if it's ready to start moving.
        /// </summary>
        public void TriggerFirst()
        {
            if (!this.ready)
                return;

            this.originID = this.id;
            this.ready = false;

            this.currentCoroutine = this.StartCoroutine(this.MoveToFirst());
        }

        /// <summary>
        /// Reset the key to it's start position and but it to rest so that it can be triggered again.
        /// </summary>
        public void Reset()
        {
            this.id = this.originID;

            this.gameObject.SetActive(true);

            this.ready = true;

            Vector2Int checkFrom = new Vector2Int(
                Mathf.Clamp(this.id.x, 0, this.inputBoard.gridTransforms.Count - 1),
                Mathf.Clamp(this.id.y, 0, this.inputBoard.gridTransforms[0].Count - 1));

            Vector3 pos = this.inputBoard.gridTransforms[checkFrom.x][checkFrom.y].position;

            Vector3 upDir = this.inputBoard.gridTransforms[0][1].position -
                            this.inputBoard.gridTransforms[0][0].position,
                rightDir = this.inputBoard.gridTransforms[1][0].position -
                           this.inputBoard.gridTransforms[0][0].position;

            Vector2Int extraDir = this.id - checkFrom;
            this.transform.position = pos + upDir * extraDir.y + rightDir * extraDir.x;
        }

        /// <summary>
        /// When the key first starts it will be outside the grid.
        /// This functions will move the key to the closest grid point.
        /// </summary>
        /// <returns>Coroutine</returns>
        private IEnumerator MoveToFirst()
        {
            float t = 0;
            Vector2Int idToMoveTo = new Vector2Int(
                Mathf.Clamp(this.id.x, 0, this.inputBoard.gridTransforms.Count - 1),
                Mathf.Clamp(this.id.y, 0, this.inputBoard.gridTransforms[0].Count - 1));

            Vector3 dir = this.inputBoard.gridTransforms[idToMoveTo.x][idToMoveTo.y].position - this.transform.position;

            while (t < this.timePerBlock)
            {
                t += Time.deltaTime;

                this.transform.position += dir * (1 / this.timePerBlock * Time.deltaTime);

                yield return null;
            }

            this.inputBoard.Trigger(this, idToMoveTo);
        }
    }
}