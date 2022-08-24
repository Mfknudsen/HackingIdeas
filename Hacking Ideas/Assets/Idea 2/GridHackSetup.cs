using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Idea_2
{
    public class GridHackSetup : MonoBehaviour
    {
        public Vector2Int gridSize = new Vector2Int(3, 3);
        [HideInInspector] public Vector2Int currentSize;
        public InputBoard inputBoard;
        public VisualGrid visualGrid;
        public Transform endPoint;
        [HideInInspector] public List<Vector2Int> keyStartPositions = new List<Vector2Int>();

        public float timePerBlock = 1;

        [Space, Header("Visual")] public GameObject tilePrefab;
        public GameObject keyPrefab;

        private void OnValidate()
        {
            for (int i = 0; i < keyStartPositions.Count; i++)
            {
                keyStartPositions[i] = new Vector2Int(
                    Mathf.Clamp(keyStartPositions[i].x, -1, gridSize.x),
                    Mathf.Clamp(keyStartPositions[i].y, -1, gridSize.y)
                );
            }
        }

        private void Start()
        {
            foreach (Vector2Int index in keyStartPositions)
            {
                GameObject obj = Instantiate(this.keyPrefab, transform);
                obj.name = "Key";

                GridKey key = obj.GetComponent<GridKey>();

                key.id = index;
                key.visualGrid = this.visualGrid;
                key.inputBoard = this.inputBoard;

                Vector3 upDir = this.visualGrid.gridTransforms[0][1].position -
                                this.visualGrid.gridTransforms[0][0].position,
                    rightDir = this.visualGrid.gridTransforms[1][0].position -
                               this.visualGrid.gridTransforms[0][0].position;

                key.transform.position = this.visualGrid.gridTransforms[0][0].position +
                                         rightDir * index.x + upDir * index.y;
            }
        }

        public void Setup()
        {
            currentSize = gridSize;
            //Setup Grid
            this.inputBoard.Setup(this.gridSize, this.tilePrefab);
            this.visualGrid.Setup(this.gridSize, this.tilePrefab);

            this.keyStartPositions.Clear();

            int xSize = this.visualGrid.gridTransforms.Count,
                ySize = this.visualGrid.gridTransforms[0].Count;
            Vector2Int endID = new Vector2Int(
                Random.Range(0, xSize),
                Random.Range(0, ySize));

            endID = Random.Range(0f, 1f) < .5f
                ? new Vector2Int(
                    endID.x,
                    Random.Range(0f, 1f) < .5f ? -1 : ySize)
                : new Vector2Int(
                    Random.Range(0f, 1f) < .5f ? -1 : xSize,
                    endID.y);

            Vector2Int extraDir = new Vector2Int(
                endID.x == -1 ? -1 :
                endID.x == xSize ? 1 :
                0,
                endID.y == -1 ? -1 :
                endID.y == ySize ? 1 :
                0
            );

            this.endPoint.GetComponent<GridKey>().id = endID;

            Vector2Int checkFrom = new Vector2Int(
                Mathf.Clamp(endID.x, 0, xSize - 1),
                Mathf.Clamp(endID.y, 0, ySize - 1));

            Vector3 pos = this.visualGrid.gridTransforms[checkFrom.x][checkFrom.y].position;
            Vector3 upDir = this.visualGrid.gridTransforms[0][1].position -
                            this.visualGrid.gridTransforms[0][0].position,
                rightDir = this.visualGrid.gridTransforms[1][0].position -
                           this.visualGrid.gridTransforms[0][0].position;

            this.endPoint.position = pos + upDir * extraDir.y + rightDir * extraDir.x;
        }
    }
}