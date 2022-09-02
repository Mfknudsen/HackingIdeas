using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Idea_5
{
    public class HordeShooterHackSetup : MonoBehaviour
    {
        [SerializeField] private float timeBetweenSpawn,
            targetLife;

        [SerializeField] private GameObject targetPrefab, particlePrefab;

        //x = min  y = max
        [SerializeField] private Vector2 spawnDistance;

        [SerializeField] private Color rightColor, leftColor;

        [SerializeField] private int goal;
        [HideInInspector] public int hits;

        [SerializeField] private TextMeshPro textMeshPro;

        [SerializeField] private Transform playerTransform;


        private Coroutine spawner;

        private static readonly int ColorID = Shader.PropertyToID("_Color");

        private void Start()
        {
            this.spawner = StartCoroutine(SpawnTargets());

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
            this.textMeshPro.text = hits + " / " + goal;

            if (this.hits < this.goal) return;

            foreach (Target c in GetComponentsInChildren<Target>())
            {
                ParticleSystem particle = Instantiate(particlePrefab).GetComponent<ParticleSystem>();
                particle.transform.position = c.transform.position;
                ParticleSystem.MainModule particleSystemMain = particle.main;
                particleSystemMain.startColor = c.color;

                Destroy(c.gameObject);
            }

            StopCoroutine(this.spawner);

            this.textMeshPro.text = "Victory";
        }

        private void OnDestroy() => StopCoroutine(this.spawner);

        private IEnumerator SpawnTargets()
        {
            while (true)
            {
                yield return new WaitForSeconds(this.timeBetweenSpawn);

                GameObject obj = Instantiate(this.targetPrefab, transform);

                Vector3 dir = Vector3.right * Random.Range(-.75f, .75f) +
                              Vector3.up * Random.Range(0, .75f) +
                              Vector3.forward * Random.Range(-.75f, 0f);
                dir.Normalize();

                Vector3 playerPos = playerTransform.position;
                obj.transform.position =
                    playerPos + dir * Random.Range(this.spawnDistance.x, this.spawnDistance.y);
                obj.transform.LookAt(playerPos);

                Target target = obj.GetComponent<Target>();
                target.color = Random.Range(0f, 1f) < .5f ? this.rightColor : this.leftColor;
                target.life = this.targetLife;
                target.setup = this;

                Renderer r = target.GetComponent<Renderer>();
                r.material.EnableKeyword("_Color");
                r.material.SetColor(ColorID, target.color);
            }

            // ReSharper disable once IteratorNeverReturns
        }
    }
}