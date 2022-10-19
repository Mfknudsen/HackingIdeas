using UnityEngine;

namespace Idea_1
{
    public class PathCreator : MonoBehaviour
    {
        [HideInInspector]
        public Path path;
        
        private void CreatePath() => 
            this.path = new Path(transform.position);

        private void Reset() => 
            CreatePath();
    }
}