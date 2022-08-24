using UnityEngine;
using Random = UnityEngine.Random;

namespace Idea_2
{
    public class Table : MonoBehaviour
    {
        public GameObject toSpawn;
        [HideInInspector] public GameObject holding;

        [HideInInspector] public bool spawning;

        private void Start()
        {
            GameObject obj = Instantiate(this.toSpawn);

            Transform trans = transform;
            Transform t = obj.transform;
            t.position += trans.position + Vector3.up * .25f;
            t.LookAt(Vector3.forward, Vector3.up);

            InputBlock block = obj.GetComponent<InputBlock>();
            block.table = this;
            block.inputBoard = trans.parent.GetComponentInChildren<InputBoard>();

            obj.transform.Rotate(trans.up, Random.Range(0, 10));

            this.holding = obj;
            this.spawning = false;
        }

        private void Update()
        {
            SpawnNew();
        }

        private void SpawnNew()
        {
            if (this.holding != null)
            {
                if (!this.spawning)
                    return;

                if (Vector3.Distance(this.holding.transform.position, transform.position + Vector3.up * .25f) < .2f)
                    return;
            }

            GameObject obj = Instantiate(this.toSpawn);

            Transform trans = transform;
            Transform t = obj.transform;
            t.position += trans.position + Vector3.up * .25f;
            t.LookAt(Vector3.forward, Vector3.up);

            InputBlock block = obj.GetComponent<InputBlock>();
            block.table = this;
            block.inputBoard = trans.parent.GetComponentInChildren<InputBoard>();

            obj.transform.Rotate(trans.up, Random.Range(0, 10));

            this.holding = obj;
            spawning = false;
        }
    }
}