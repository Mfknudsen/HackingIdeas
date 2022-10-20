using UnityEngine;

namespace Idea_1
{
    public class PathCreator : MonoBehaviour
    {
        [HideInInspector]
        public Path path;
        
        private void CreatePath() => 
            this.path = new Path(this.transform.position);

        private void Reset() => this.CreatePath();
    }
}