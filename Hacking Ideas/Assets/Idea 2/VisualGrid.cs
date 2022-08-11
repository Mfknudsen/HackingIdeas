using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Idea_2
{
    public class VisualGrid : MonoBehaviour
    {
        public Transform[,] gridTransforms;
        public float sizeScale = 1;
        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private List<GridSquare> blocks = new List<GridSquare>();

        private void Start()
        {
            this.gridTransforms = new Transform[gridSize.x, gridSize.y];

            foreach (GridSquare block in blocks)
                this.gridTransforms[block.id.x, block.id.y] = block.transform;
        }

        public void Setup(Vector2Int size, GameObject tilePrefab)
        {
            this.gridSize = size;

            foreach (GameObject o in (from Transform t in transform
                         where t.GetComponent<GridSquare>() != null
                         select t.gameObject).ToList())
                DestroyImmediate(o);

            this.gridTransforms = new Transform[size.x, size.y];
            this.blocks.Clear();

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    GameObject obj = Instantiate(tilePrefab, transform);
                    obj.name = "Square + " + x + "-" + y;
                    obj.transform.localScale *= sizeScale;
                    float localSize = obj.transform.localScale.x * 10;
                    this.gridTransforms[x, y] = obj.transform;
                    obj.transform.position = transform.position;
                    obj.transform.position -= transform.right * x * localSize - transform.up * y * localSize;
                    obj.transform.position += transform.right * (size.x / 2) * localSize -
                                              transform.up * (size.y / 2) * localSize;
                    obj.transform.LookAt(obj.transform.position - transform.up);

                    GridSquare square = obj.GetComponent<GridSquare>();
                    square.id = new Vector2Int(x, y);

                    this.blocks.Add(square);
                }
            }
        }
    }
}