using System.Collections;
using System.Linq;
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
        private static readonly int ColorID = Shader.PropertyToID("_Color");

        public abstract IEnumerator TriggerInput(GridKey key, float timePerBlock);

        protected override void Start()
        {
            base.Start();

            speed = Random.Range(5, 10);
            speed *= Random.Range(0f, 1f) < .5f ? -1 : 1;
        }

        private void Update()
        {
            if (this.grabbed)
            {
                Transform t = null;
                Vector3 pos = transform.position;
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

                foreach (Renderer r in GetComponentsInChildren<Renderer>()
                             .Where(r => r.material.name.Contains("Arrow")))
                {
                    r.material.EnableKeyword("_Color");
                    r.material.SetColor(ColorID, t != null ? Color.green : Color.white);
                }
            }

            if (!this.inserted && !this.grabbed)
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


            foreach (Renderer r in GetComponentsInChildren<Renderer>()
                         .Where(r => r.material.name.Contains("Arrow")))
            {
                r.material.EnableKeyword("_Color");
                r.material.SetColor(ColorID, Color.white);
            }

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

        protected Vector3 RightDir() =>
            this.inputBoard.gridTransforms[1][0].position - inputBoard.gridTransforms[0][0].position;

        protected Vector3 UpDir() =>
            this.inputBoard.gridTransforms[0][1].position - this.inputBoard.gridTransforms[0][0].position;
    }
}