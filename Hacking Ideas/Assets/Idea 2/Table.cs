using UnityEngine;

namespace Idea_2
{
    public class Table : MonoBehaviour
    {
        public GameObject toSpawn;

        private void Start()
        {
            Transform parent = transform.parent;
            GameObject obj = Instantiate(this.toSpawn, parent);
            Return(obj.transform);

            InputBlock block = obj.GetComponent<InputBlock>();
            block.table = this;
            block.inputBoard = parent.GetComponentInChildren<InputBoard>();
            
            obj.transform.Rotate(transform.up, Random.Range(0, 10));
        }

        public void Return(Transform toReturn)
        {
            toReturn.position = transform.position + Vector3.up * .25f;
            toReturn.LookAt(Vector3.forward, Vector3.up);
        }
    }
}
