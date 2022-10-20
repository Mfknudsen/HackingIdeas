using System.Collections;
using UnityEngine;

namespace Idea_2.Blockers
{
    /// <summary>
    /// A static blocker to create obstacles for the player to overcome.
    /// By default the blocker forces the key to move a specific direction by one.
    /// </summary>
    public class Blocker : MonoBehaviour
    {
        /// <summary>
        /// The input board the blocker is placed on.
        /// </summary>
        public InputBoard inputBoard;
        
        /// <summary>
        /// The direction the blocker is facing. 
        /// </summary>
        public PlaceDirection placeDirection;
        
        /// <summary>
        /// Its current id on the grid board.
        /// </summary>
        public Vector2Int id;

        /// <summary>
        /// ID direction from the place direction.
        /// </summary>
        private Vector2Int idDir => this.placeDirection switch
        {
            PlaceDirection.MinusX => new Vector2Int(-1, 0),
            PlaceDirection.MinusY => new Vector2Int(0, -1),
            PlaceDirection.PlusX => new Vector2Int(1, 0),
            _ => new Vector2Int(0, 1)
        };

        /// <summary>
        /// Get the right direction from the input board.
        /// </summary>
        /// <returns>Right direction of the input board</returns>
        protected Vector3 RightDir() =>
            this.inputBoard.gridTransforms[1][0].position - this.inputBoard.gridTransforms[0][0].position;

        /// <summary>
        /// Get the up direction from the input board.
        /// </summary>
        /// <returns>Up direction of the input board</returns>
        protected Vector3 UpDir() =>
            this.inputBoard.gridTransforms[0][1].position - this.inputBoard.gridTransforms[0][0].position;

        /// <summary>
        /// How the block moves the key.
        /// </summary>
        /// <param name="key">To move</param>
        /// <param name="timePerBlock">Duration of the move</param>
        /// <param name="board">Board placed on</param>
        /// <returns>Coroutine</returns>
        public virtual IEnumerator Trigger(GridKey key, float timePerBlock, InputBoard board)
        {
            this.inputBoard = board;
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

            board.Trigger(key, idToMoveTo);
        }

        /// <summary>
        /// For when the blocker needs to reset to an original state.
        /// </summary>
        public virtual void Reset()
        {
        }
    }
}