using Sirenix.OdinInspector;
using UnityEngine;

namespace Idea_2
{
    public class GridHackSetup : MonoBehaviour
    {
        public Vector2Int gridSize = new Vector2Int(3, 3);
        public InputBoard inputBoard;
        public VisualGrid visualGrid;
        public GridKey gridKey;
        public Transform endPoint;

        private Vector2Int endPointID;

        public float timePerBlock = 1;

        [Space, Header("Visual")] public GameObject tilePrefab;

        [Button]
        private void Setup()
        {
            //Setup Grid
            this.inputBoard.Setup(this.gridSize, this.tilePrefab);
            this.visualGrid.Setup(this.gridSize, this.tilePrefab);
            this.gridKey.ShuffleStartPosition(this.visualGrid);

            this.endPointID = this.gridKey.id;
            while (this.endPointID == this.gridKey.id)
            {
                int xSize = this.visualGrid.gridTransforms.GetLength(0),
                    ySize = this.visualGrid.gridTransforms.GetLength(1);
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

                this.endPointID = endID;
                this.endPoint.GetComponent<GridKey>().id = endID;

                Vector2Int checkFrom = new Vector2Int(
                    Mathf.Clamp(endID.x, 0, xSize - 1),
                    Mathf.Clamp(endID.y, 0, ySize - 1));

                Vector3 pos = this.visualGrid.gridTransforms[checkFrom.x, checkFrom.y].position;
                Vector3 upDir = this.visualGrid.gridTransforms[0, 1].position -
                                this.visualGrid.gridTransforms[0, 0].position,
                    rightDir = this.visualGrid.gridTransforms[1, 0].position -
                               this.visualGrid.gridTransforms[0, 0].position;

                this.endPoint.position = pos + upDir * extraDir.y + rightDir * extraDir.x;
            }
        }

        [Button]
        private void ShuffleKeyStartPosition()
        {
            this.gridKey.ShuffleStartPosition(this.visualGrid);
        }
    }
}