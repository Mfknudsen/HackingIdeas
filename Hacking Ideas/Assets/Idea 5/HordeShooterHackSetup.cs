using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Idea_5
{
    public class HordeShooterHackSetup : MonoBehaviour
    {
        [SerializeField] private float timeBetweenSpawn;

        [SerializeField] private GameObject targetPrefab, particlePrefab;

        [Tooltip("X is minimum distance. Y is maximum distance")] [SerializeField]
        private Vector2 spawnDistance;

        [SerializeField] private Color rightColor, leftColor, dangerColor, bonusColor;

        [SerializeField] private int goal;
        public int hits = 1;

        [SerializeField] private TextMeshPro textMeshPro;

        [SerializeField] private Transform playerTransform;

        [SerializeField] private bool spawnAgainstPlayer;

        private Coroutine spawner;

        private static readonly int ColorID = Shader.PropertyToID("_Color");

        private void Start()
        {
            this.spawner = this.StartCoroutine(this.SpawnTargets());

            foreach (Shooter shooter in FindObjectsOfType<Shooter>())
            {
                shooter.color = shooter.rightHand ? this.rightColor : this.leftColor;

                foreach (Renderer c in shooter.GetComponentsInChildren<Renderer>())
                {
                    c.material.EnableKeyword("_Color");
                    c.material.SetColor(ColorID, shooter.color);
                }
            }
        }

        private void Update()
        {
            this.textMeshPro.text = this.hits + " / " + this.goal;

            if (this.hits < this.goal && this.hits > 0) return;

            foreach (Target c in this.GetComponentsInChildren<Target>())
            {
                ParticleSystem particle = Instantiate(this.particlePrefab).GetComponent<ParticleSystem>();
                particle.transform.position = c.transform.position;
                ParticleSystem.MainModule particleSystemMain = particle.main;
                particleSystemMain.startColor = c.color;

                Destroy(c.gameObject);
            }

            this.StopCoroutine(this.spawner);

            this.textMeshPro.text = this.hits >= this.goal ? "Victory" : "Defeat";
        }

        private void OnDestroy() => this.StopCoroutine(this.spawner);

        private IEnumerator SpawnTargets()
        {
            int index = 0;

            while (true)
            {
                yield return new WaitForSeconds(this.timeBetweenSpawn);

                if (this.spawnAgainstPlayer)
                {
                    GameObject obj = Instantiate(this.targetPrefab, this.transform);

                    Vector3 dir = Vector3.right * Random.Range(-.75f, .75f) +
                                  Vector3.up * Random.Range(0, .55f) +
                                  Vector3.forward * Random.Range(-.75f, -.2f);
                    dir.Normalize();

                    Vector3 playerPos = this.playerTransform.position;
                    obj.transform.position =
                        playerPos + dir * Random.Range(this.spawnDistance.x, this.spawnDistance.y);
                    obj.transform.LookAt(playerPos + Vector3.up);

                    Target target = obj.GetComponent<Target>();
                    target.color = Random.Range(0f, 1f) < .95f
                        ? new[] { this.leftColor, this.rightColor, this.dangerColor }[index.UniqueIndex(3)]
                        : this.bonusColor;
                    target.playerPos = playerPos;
                    target.setup = this;
                    target.moveDir = target.transform.forward;

                    target.danger = target.color == this.dangerColor;
                    target.bonus = target.color == this.bonusColor;

                    Renderer r = target.GetComponent<Renderer>();
                    r.material.EnableKeyword("_Color");
                    r.material.SetColor(ColorID, target.color);
                }
                else
                {
                    GameObject obj = Instantiate(this.targetPrefab, this.transform);

                    Vector3 dir = Vector3.right * Random.Range(-.55f, .55f) +
                                  Vector3.up * Random.Range(0, .35f) +
                                  Vector3.forward * Random.Range(-.75f, -.2f);
                    dir.Normalize();

                    Vector3 playerPos = this.playerTransform.position;
                    obj.transform.position =
                        playerPos + dir * Random.Range(this.spawnDistance.x, this.spawnDistance.y);
                    obj.transform.LookAt(obj.transform.position + obj.transform.forward);

                    Target target = obj.GetComponent<Target>();
                    target.color = Random.Range(0f, 1f) < .95f
                        ? new[] { this.leftColor, this.rightColor, this.dangerColor }[index.UniqueIndex(3)]
                        : this.bonusColor;
                    target.playerPos = playerPos;
                    target.setup = this;
                    target.moveDir = new Vector3(-dir.x, Random.Range(-.1f, .1f), 0).normalized;
                    target.life = Random.Range(10, 20);

                    target.danger = target.color == this.dangerColor;
                    target.bonus = target.color == this.bonusColor;

                    Renderer r = target.GetComponent<Renderer>();
                    r.material.EnableKeyword("_Color");
                    r.material.SetColor(ColorID, target.color);
                }
            }

            // ReSharper disable once IteratorNeverReturns
        }
    }
}