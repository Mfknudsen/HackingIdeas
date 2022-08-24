using System.Collections;
using UnityEngine;

namespace Idea_2
{
    public enum PlaceDirection
    {
        PlusX,
        MinusX,
        PlusY,
        MinusY
    }

    public abstract class InputBlock : VrGrabObject
    {
        public PlaceDirection placeDirection;
        public InputBoard inputBoard;
        public Table table;
        private bool inserted;
        public Vector2Int id;
        private float speed;
        
        public abstract IEnumerator TriggerInput(GridKey key, float timePerBlock, VisualGrid visualGrid);

        private void Start()
        {
            speed = Random.Range(5, 10);
            speed *= Random.Range(0f, 1f) < .5f ? -1 : 1;
        }

        private void Update()
        {
            if (!this.inserted)
                transform.Rotate(transform.up, speed * Time.deltaTime);
        }

        protected override void OnGrab()
        {
            if (this.inserted)
                this.inputBoard.RemoveFromBoard(this);
            else
                this.table.spawning = true;
        }

        protected override void OnRelease()
        {
            this.inserted = this.inputBoard.TryPlaceBlock(this);

            if (!this.inserted)
                Destroy(gameObject);
        }

        public virtual void Reset()
        {
        }

        public Vector2Int idDir => this.placeDirection switch
        {
            PlaceDirection.MinusX => new Vector2Int(-1, 0),
            PlaceDirection.MinusY => new Vector2Int(0, -1),
            PlaceDirection.PlusX => new Vector2Int(1, 0),
            _ => new Vector2Int(0, 1)
        };

        protected static Vector3 RightDir(VisualGrid grid) =>
            grid.gridTransforms[1][0].position - grid.gridTransforms[0][0].position;

        protected static Vector3 UpDir(VisualGrid grid) =>
            grid.gridTransforms[0][1].position - grid.gridTransforms[0][0].position;
    }
}