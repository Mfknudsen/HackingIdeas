using System.Collections;
using System.Linq;
using UnityEngine;

namespace Idea_2
{
    /// <summary>
    /// Enum to determine witch way it will move the key on the grid board. 
    /// </summary>
    public enum PlaceDirection
    {
        PlusX,
        MinusX,
        PlusY,
        MinusY
    }

    /// <summary>
    /// This is the blocks the player will be placing on the grid board.
    /// The way the keys move is determined in inherited classes.
    /// </summary>
    public abstract class InputBlock : VrGrabObject
    {
        /// <summary>
        /// What way the block is placed on the grid board and the way it will move the key.
        /// </summary>
        public PlaceDirection placeDirection;
        
        /// <summary>
        /// The input board for getting possibly placement positions and for signaling the key is at a new id.
        /// </summary>
        public InputBoard inputBoard;
        
        /// <summary>
        /// The block is spawned at the table and the block will signal when it's grabbed.
        /// </summary>
        public Table table;
        
        /// <summary>
        /// If the block has been placed in the input board.
        /// </summary>
        private bool inserted;
        
        /// <summary>
        /// The blocks id in the input board.
        /// </summary>
        public Vector2Int id;
        
        /// <summary>
        /// For visual effect.
        /// When the block is still on the table the block will rotate at this speed.
        /// </summary>
        private float speed;
        
        /// <summary>
        /// To set the color to show when the block can be placed onto the input board.
        /// </summary>
        private static readonly int ColorID = Shader.PropertyToID("_Color");

        /// <summary>
        /// How the block moves the key.
        /// </summary>
        /// <param name="key">To move</param>
        /// <param name="timePerBlock">Duration of the move</param>
        /// <returns>Coroutine</returns>
        public abstract IEnumerator TriggerInput(GridKey key, float timePerBlock);

        /// <summary>
        /// Start for the grab object and setting speed.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            this.speed = Random.Range(5, 10);
            this.speed *= Random.Range(0f, 1f) < .5f ? -1 : 1;
        }

        /// <summary>
        /// If block is grabbed then it will check if it can be placed and show it through a color change.
        /// If not grabbed and still at the table then it will rotate around itself.
        /// </summary>
        private void Update()
        {
            if (this.grabbed)
            {
                Transform t = null;
                Vector3 pos = this.transform.position;
                float dist = 10;
                for (int x = 0; x < this.inputBoard.gridTransforms.Count; x++)
                {
                    for (int y = 0; y < this.inputBoard.gridTransforms[0].Count; y++)
                    {
                        if (this.inputBoard.blockerTypes[x][y] != BlockerType.None) continue;

                        float d = Vector3.Distance(pos, this.inputBoard.gridTransforms[x][y].position);

                        if (d >= dist || d > .125f) continue;

                        dist = d;
                        t = this.inputBoard.gridTransforms[x][y];
                    }
                }

                foreach (Renderer r in this.GetComponentsInChildren<Renderer>()
                             .Where(r => r.material.name.Contains("Arrow")))
                {
                    r.material.EnableKeyword("_Color");
                    r.material.SetColor(ColorID, t != null ? Color.green : Color.white);
                }
            }

            if (!this.inserted && !this.grabbed) 
                this.transform.Rotate(this.transform.up, this.speed * Time.deltaTime);
        }

        /// <summary>
        /// If already inserted into the input board then remove it.
        /// Else it would be in the table and then tell the table to spawn a new block.
        /// </summary>
        protected override void OnGrab()
        {
            if (this.inserted)
                this.inputBoard.RemoveFromBoard(this);
            else
                this.table.spawning = true;
        }

        /// <summary>
        /// Try to place the block in the input board and if failed then destroy the block.
        /// </summary>
        protected override void OnRelease()
        {
            this.inserted = this.inputBoard.TryPlaceBlock(this);
            
            foreach (Renderer r in this.GetComponentsInChildren<Renderer>()
                         .Where(r => r.material.name.Contains("Arrow")))
            {
                r.material.EnableKeyword("_Color");
                r.material.SetColor(ColorID, Color.white);
            }

            if (!this.inserted)
                Destroy(this.gameObject);
        }

        public virtual void Reset()
        {
        }

        /// <summary>
        /// Id move direction from the place direction enum.
        /// </summary>
        public Vector2Int idDir => this.placeDirection switch
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
    }
}