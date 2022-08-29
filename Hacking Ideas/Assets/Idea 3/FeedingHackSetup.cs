using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Idea_3
{
    public enum State
    {
        Running,
        Lost,
        Won
    }

    public class FeedingHackSetup : MonoBehaviour
    {
        public List<Color> colors;

        [Header("Mouth"), SerializeField] private int mouthCount;
        [SerializeField] private float distanceBetween;
        [SerializeField] private GameObject mouthPrefab, foodPrefab;
        [SerializeField] private Transform mouthSpawnOrigin;

        [Header("Food"), Tooltip("Time in minutes"), SerializeField]
        private float timeToMax;

        [SerializeField] private AnimationCurve spawnCurve;

        [SerializeField] private Vector3 spawnPos;
        [SerializeField] private Vector2 spawnSize;

        [SerializeField] private GameObject particle;

        private float currentTime;
        private Coroutine spawnCoroutine;

        private State state = State.Running;

        private void OnDrawGizmos()
        {
            Vector3 size = new Vector3(this.spawnSize.x, 0, this.spawnSize.y);
            Vector3 oppositeSpawn = new Vector3(-this.spawnPos.x - this.spawnSize.x, this.spawnPos.y, this.spawnPos.z);

            Gizmos.DrawWireCube(this.spawnPos + size / 2, size);
            Gizmos.DrawWireCube(oppositeSpawn + size / 2, size);
        }

        private void Start()
        {
            Vector3 originPos = this.mouthSpawnOrigin.position, right = this.mouthSpawnOrigin.right;

            for (int i = 0; i < this.mouthCount; i++)
            {
                GameObject obj = Instantiate(this.mouthPrefab, transform);

                Vector3 pos = originPos + right * (this.distanceBetween * this.mouthCount) / 2;
                pos += -right * this.distanceBetween * i + Vector3.up * Random.Range(.2f, .7f);
                obj.transform.position = pos;

                Mouth m = obj.GetComponent<Mouth>();
                m.SetColor(this.colors[i]);
                m.setup = this;
            }
        }

        private void Update()
        {
            if (this.state != State.Running) return;

            CheckState();
            SpawnFood();
        }

        private void CheckState()
        {
            int done = 0, dead = 0;
            foreach (Mouth m in GetComponentsInChildren<Mouth>())
            {
                if (m.current == m.max)
                    done++;
                else if (m.current == m.min)
                {
                    dead++;
                    break;
                }
            }

            if (done == this.mouthCount)
                this.state = State.Won;
            else if (dead > 0)
                this.state = State.Lost;

            if (this.state == State.Running) return;

            foreach (Transform t in transform)
            {
                if (t.GetComponent<Mouth>() is { } mouth)
                    mouth.done = true;

                foreach (TextMeshPro text in t.GetComponentsInChildren<TextMeshPro>())
                    text.text = this.state.ToString();
            }

            Food[] foods = GetComponentsInChildren<Food>();

            for (int i = foods.Length - 1; i >= 0; i--)
            {
                Food f = foods[i];

                Instantiate(this.particle).transform.position = f.transform.position;

                Destroy(f.gameObject);
            }

            if (this.spawnCoroutine != null)
                StopCoroutine(this.spawnCoroutine);
        }

        private void SpawnFood()
        {
            //Current Time in second Time To Max in minutes
            this.currentTime += Time.deltaTime;

            if (this.spawnCoroutine != null) return;

            float timeBetween = this.spawnCurve.Evaluate(this.currentTime / (60 * this.timeToMax));

            this.spawnCoroutine = StartCoroutine(SpawnFoodTimer(timeBetween));
        }

        private IEnumerator SpawnFoodTimer(float time)
        {
            yield return new WaitForSeconds(time);

            GameObject obj = Instantiate(this.foodPrefab, transform);
            Vector3 oppositeSpawn = new Vector3(-this.spawnPos.x - this.spawnSize.x, this.spawnPos.y, this.spawnPos.z);

            obj.transform.position = (Random.Range(1, 11) <= 5 ? this.spawnPos : oppositeSpawn) +
                                     new Vector3(
                                         Random.Range(0f, this.spawnSize.x),
                                         0,
                                         Random.Range(0f, this.spawnSize.y)
                                     );

            obj.transform.LookAt(obj.transform.position +
                                 new Vector3(
                                     Random.Range(-.2f, .2f),
                                     -1,
                                     Random.Range(-.2f, .2f)
                                 ));

            Food f = obj.GetComponent<Food>();
            f.SetColor(this.colors[Random.Range(0, this.colors.Count)]);

            this.spawnCoroutine = null;
        }
    }
}