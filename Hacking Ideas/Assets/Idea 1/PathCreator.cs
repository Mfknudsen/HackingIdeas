using UnityEngine;

namespace Idea_1
{
    public class PathCreator : MonoBehaviour
    {
        [HideInInspector]
        public Path path;

        public Color anchorCol = Color.red;
        public Color controlCol = Color.white;
        public Color segmentCol = Color.green;
        public Color selectedSegmentCol = Color.yellow;
        public float anchorDiameter = .1f;
        public float controlDiameter = .075f;
        public bool displayControlPoints = true;

        public Path CreatePath()
        {
            this.path = new Path(transform.position);
            return this.path;
        }

        private void Reset()
        {
            CreatePath();
        }
    }
}