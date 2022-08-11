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

    public abstract class InputBlock : VrGrabable
    {
        public PlaceDirection placeDirection;
        public InputBoard inputBoard;
        public Table table;
        private bool inserted;
        public Vector2Int id;

        protected float t = 0;

        public abstract IEnumerator TriggerInput(GridKey key, float timePerBlock, VisualGrid visualGrid);

        private void Update()
        {
            if (!this.inserted)
                transform.Rotate(transform.up, 5 * Time.deltaTime);
        }

        protected override void OnGrab()
        {
            if (this.inserted)
                this.inputBoard.RemoveFromBoard(this);
        }

        protected override void OnRelease()
        {
            this.inserted = this.inputBoard.TryPlaceBlock(this);

            if (!this.inserted)
                this.table.Return(transform);
        }

        public Vector2Int idDir => this.placeDirection switch
        {
            PlaceDirection.MinusX => new Vector2Int(-1, 0),
            PlaceDirection.MinusY => new Vector2Int(0, -1),
            PlaceDirection.PlusX => new Vector2Int(1, 0),
            _ => new Vector2Int(0, 1)
        };
    }
}