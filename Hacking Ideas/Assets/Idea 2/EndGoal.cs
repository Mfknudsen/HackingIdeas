using UnityEngine;

namespace Idea_2
{
    public class EndGoal : GridKey
    {
        private void OnValidate()
        {
            GridHackSetup setup = transform.parent.GetComponent<GridHackSetup>();
            Vector2Int size = setup.currentSize;
            id = new Vector2Int(
                Mathf.Clamp(id.x, -1, size.x),
                Mathf.Clamp(id.y, -1, size.y)
            );

            Vector3 right = this.inputBoard.gridTransforms[1][0].position - this.inputBoard.gridTransforms[0][0].position,
                up = this.inputBoard.gridTransforms[0][1].position - this.inputBoard.gridTransforms[0][0].position;

            transform.position = this.inputBoard.gridTransforms[0][0].position + right * id.x + up * id.y;
        }
    }
}