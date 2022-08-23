using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Idea_2
{
    public class VisualGrid : MonoBehaviour
    {
        [SerializeField] public List<Storage<Transform>> gridTransforms;
        public float sizeScale = 1;
        private Vector2Int gridSize;

        public void Setup(Vector2Int size, GameObject tilePrefab)
        {
            this.gridSize = size;

            foreach (GameObject o in (from Transform t in transform
                         select t.gameObject).ToList())
                DestroyImmediate(o);

            this.gridTransforms = new List<Storage<Transform>>();
            for (int x = 0; x < gridSize.x; x++)
                this.gridTransforms.Add(new Storage<Transform>(gridSize.y));


            Transform temp = transform;

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    GameObject obj = Instantiate(tilePrefab, temp);
                    obj.name = "Square + " + x + "-" + y;
                    Vector3 tempScale = obj.transform.localScale * sizeScale;
                    float localSize = tempScale.x * 10;
                    obj.transform.localScale = tempScale;
                    this.gridTransforms[x].Set(y, obj.transform);
                    Vector3 tempPosition = temp.position,
                        up = temp.up,
                        right = temp.right;
                    tempPosition -= right * (x * localSize) - up * (y * localSize);
                    tempPosition += right * (size.x / 2f * localSize) - up * (size.y / 2f * localSize);
                    tempPosition -= right * (tempScale.x * 5f);
                    obj.transform.position = tempPosition;
                    obj.transform.LookAt(tempPosition - up);
                }
            }
        }
    }
}