using System.Collections;
using TMPro;
using UnityEngine;

namespace Idea_5
{
    public class Target : MonoBehaviour
    {
        public Color color;
        public HordeShooterHackSetup setup;
        [SerializeField] private float speed = .25f, fadeTime;
        public bool danger, bonus;
        public Vector3 playerPos;
        [SerializeField] private GameObject particlePrefab, pointDisplay;
        public Vector3 moveDir;
        public float life;
        private static readonly int ColorID = Shader.PropertyToID("_Color");

        private IEnumerator Start()
        {
            if (this.life == 0)
                yield break;

            yield return new WaitForSeconds(this.life - this.fadeTime);

            float t = 0;
            Renderer r = GetComponent<Renderer>();
            while (t < this.fadeTime)
            {
                r.material.SetColor(ColorID, this.color * new Color(1, 1, 1, 1 - t));

                t += Time.deltaTime;
                yield return null;
            }

            if (!this.danger)
                ChangePoints(-1);
            
            Destroy(gameObject);
        }

        private void Update()
        {
            Transform t = transform;
            t.position += this.moveDir * (this.speed * Time.deltaTime);
            t.LookAt(playerPos);

            if (Vector3.Distance(t.position, playerPos) > 1.35f) return;

            if (!this.danger)
                ChangePoints(-1);

            ParticleSystem particle = Instantiate(this.particlePrefab).GetComponent<ParticleSystem>();
            particle.transform.position = t.position;
            ParticleSystem.MainModule particleSystemMain = particle.main;
            particleSystemMain.startColor = this.color;

            Destroy(gameObject);
        }

        public void Hit(Color c)
        {
            if (this.bonus)
            {
                ChangePoints(5);

                Destroy(gameObject);
                return;
            }

            if (!this.color.Equals(c) && !this.danger) return;

            ChangePoints(this.danger ? -1 : 1);

            Destroy(gameObject);
        }

        private void ChangePoints(int change)
        {
            GameObject obj = Instantiate(this.pointDisplay);
            Transform t = transform;
            obj.transform.position = t.position;
            obj.transform.LookAt(obj.transform.position - t.forward);
            obj.GetComponent<TextMeshPro>().text = (change > 0 ? "+" : "-") +
                                                   (Mathf.Abs(change) == 1 ? "" : Mathf.Abs(change).ToString());

            this.setup.hits += change;
        }
    }
}