using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Idea_2
{
    public class InputBoard : MonoBehaviour
    {
        public InputBlock[,] gridBlocks;
        private Transform[,] gridTransforms;
        [SerializeField] private float placeDist, sizeScale = 1;
        public GridHackSetup setup;
        [SerializeField] private List<GridSquare> blocks = new List<GridSquare>();
        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private GridKey endPoint;

        private void Start()
        {
            this.gridTransforms = new Transform[gridSize.x, gridSize.y];
            this.gridBlocks = new InputBlock[gridSize.x, gridSize.y];

            foreach (GridSquare block in blocks)
                this.gridTransforms[block.id.x, block.id.y] = block.transform;
        }

        public void Setup(Vector2Int size, GameObject tilePrefab)
        {
            this.gridSize = size;
            List<GameObject> toDelete = (from Transform d in transform select d.gameObject).ToList();
            foreach (GameObject o in toDelete)
                DestroyImmediate(o);

            this.gridBlocks = new InputBlock[size.x, size.y];
            this.gridTransforms = new Transform[size.x, size.y];

            Transform t = transform;
            Vector3 startPos = t.position + t.up * .55f;
            blocks.Clear();

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    GameObject obj = Instantiate(tilePrefab, transform);
                    obj.name = "Square + " + x + "-" + y;

                    Transform oTransform = obj.transform;
                    oTransform.localScale *= sizeScale;

                    float localSize = oTransform.localScale.x * 10;

                    gridTransforms[x, y] = oTransform;

                    Vector3 objPosition = startPos;
                    Vector3 forward = t.forward, right = t.right;

                    objPosition -=
                        right * x * localSize + forward * y * localSize;
                    objPosition += right * (size.x / 2) * localSize +
                                   forward * (size.y / 2) * localSize;
                    oTransform.position = objPosition;
                    oTransform.LookAt(objPosition + forward);

                    GridSquare square = obj.GetComponent<GridSquare>();
                    square.id = new Vector2Int(x, y);

                    blocks.Add(square);
                }
            }
        }

        public void RemoveFromBoard(InputBlock block)
        {
            for (int x = 0; x < gridBlocks.GetLength(0); x++)
            {
                for (int y = 0; y < gridBlocks.GetLength(1); y++)
                {
                    if (gridBlocks[x, y] != block) continue;

                    gridBlocks[x, y] = null;
                    break;
                }
            }
        }

        public void Trigger(GridKey key, Vector2Int id)
        {
            if (id == this.endPoint.id)
            {
                key.transform.position = transform.parent.GetComponentInChildren<Button>().transform.position;
                return;
            }

            if (id.x < 0 || id.y < 0 ||
                id.x == this.gridSize.x || id.y == this.gridSize.y ||
                this.gridBlocks[id.x, id.y] == null)
            {
                key.Reset();
                return;
            }

            StartCoroutine(
                this.gridBlocks[id.x, id.y].TriggerInput(
                    key,
                    this.setup.timePerBlock,
                    this.setup.visualGrid));
        }

        public bool TryPlaceBlock(InputBlock block)
        {
            Vector2Int closestID = -Vector2Int.one;
            float lowestDist = 0;

            for (int x = 0; x < this.gridTransforms.GetLength(0); x++)
            {
                for (int y = 0; y < this.gridTransforms.GetLength(1); y++)
                {
                    Transform gridTransform = this.gridTransforms[x, y];
                    float dist = Vector3.Distance(gridTransform.position, block.transform.position);
                    if (dist >= this.placeDist)
                        continue;

                    if (closestID == -Vector2Int.one)
                    {
                        closestID = new Vector2Int(x, y);
                        lowestDist = dist;
                        continue;
                    }

                    if (dist >= lowestDist) continue;

                    lowestDist = dist;
                    closestID = new Vector2Int(x, y);
                }
            }

            if (closestID == -Vector2Int.one)
                return false;

            try
            {
                if (this.gridBlocks[closestID.x, closestID.y] != null)
                    return false;

                this.gridBlocks[closestID.x, closestID.y] = block;
            }
            catch
            {
                return false;
            }

            block.id = closestID;
            Transform tile = this.gridTransforms[closestID.x, closestID.y];
            Vector3 blockForward = block.transform.forward;
            float aForward = Vector3.Angle(blockForward, tile.forward),
                aRight = Vector3.Angle(blockForward, tile.right),
                aBackwards = Vector3.Angle(blockForward, -tile.forward),
                aLeft = Vector3.Angle(blockForward, -tile.right);

            if (aForward < aRight && aForward < aBackwards && aForward < aLeft)
                block.placeDirection = PlaceDirection.MinusY;
            else if (aRight < aBackwards && aRight < aLeft)
                block.placeDirection = PlaceDirection.MinusX;
            else if (aBackwards < aLeft)
                block.placeDirection = PlaceDirection.PlusY;
            else
                block.placeDirection = PlaceDirection.PlusX;

            block.transform.position = this.gridTransforms[closestID.x, closestID.y].position;

            Vector3 pos = this.gridTransforms[0, 0].position;
            Vector3 forward = this.gridTransforms[0, 1].position - pos,
                right = this.gridTransforms[1, 0].position - pos;

            block.transform.LookAt(
                block.transform.position
                + forward * block.idDir.y
                + right * block.idDir.x,
                this.gridTransforms[0, 0].up);

            this.gridBlocks[block.id.x, block.id.y] = block;

            return true;
        }
    }
}