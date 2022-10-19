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
        public Transform endPoint;
        [HideInInspector] public List<Vector2Int> keyStartPositions = new List<Vector2Int>();

        public float timePerBlock = 1;

        [Space, Header("Visual")] public GameObject tilePrefab;
        public GameObject keyPrefab;

        private void OnValidate()
        {
            for (int i = 0; i < this.keyStartPositions.Count; i++)
            {
                this.keyStartPositions[i] = new Vector2Int(
                    Mathf.Clamp(this.keyStartPositions[i].x, -1, this.gridSize.x),
                    Mathf.Clamp(this.keyStartPositions[i].y, -1, this.gridSize.y)
                );
            }
        }

        private void Start()
        {
            foreach (Vector2Int index in this.keyStartPositions)
            {
                GameObject obj = Instantiate(this.keyPrefab, transform);
                obj.name = "Key";

                GridKey key = obj.GetComponent<GridKey>();

                key.id = index;
                key.inputBoard = this.inputBoard;

                Vector3 upDir = this.inputBoard.gridTransforms[0][1].position -
                                this.inputBoard.gridTransforms[0][0].position,
                    rightDir = this.inputBoard.gridTransforms[1][0].position -
                               this.inputBoard.gridTransforms[0][0].position;

                Transform keyT = key.transform;
                keyT.position = this.inputBoard.gridTransforms[0][0].position +
                                      rightDir * index.x + upDir * index.y;
                keyT.localScale = Vector3.one * .1f;
            }
        }

        public void Setup()
        {
            this.currentSize = this.gridSize;
            //Setup Grid
            this.inputBoard.Setup(this.gridSize, this.tilePrefab);

            this.keyStartPositions.Clear();

            int xSize = this.inputBoard.gridTransforms.Count,
                ySize = this.inputBoard.gridTransforms[0].Count;
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

            Vector3 pos = this.inputBoard.gridTransforms[checkFrom.x][checkFrom.y].position;
            Vector3 upDir = this.inputBoard.gridTransforms[0][1].position -
                            this.inputBoard.gridTransforms[0][0].position,
                rightDir = this.inputBoard.gridTransforms[1][0].position -
                           this.inputBoard.gridTransforms[0][0].position;

            this.endPoint.position = pos + upDir * extraDir.y + rightDir * extraDir.x;
            this.endPoint.localScale = Vector3.one * .1f;
        }
    }
}