using UnityEngine;

namespace Idea_2
{
    public class EndGoal : GridKey
    {
        private void OnValidate()
        {
            GridHackSetup setup = this.transform.parent.GetComponent<GridHackSetup>();
            Vector2Int size = setup.currentSize;
            this.id = new Vector2Int(
                Mathf.Clamp(this.id.x, -1, size.x),
                Mathf.Clamp(this.id.y, -1, size.y)
            );

            Vector3 right = this.inputBoard.gridTransforms[1][0].position - this.inputBoard.gridTransforms[0][0].position,
                up = this.inputBoard.gridTransforms[0][1].position - this.inputBoard.gridTransforms[0][0].position;

            this.transform.position = this.inputBoard.gridTransforms[0][0].position + right * this.id.x + up * this.id.y;
        }
    }
}