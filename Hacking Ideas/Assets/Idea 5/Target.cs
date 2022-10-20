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
            Renderer r = this.GetComponent<Renderer>();
            while (t < this.fadeTime)
            {
                r.material.SetColor(ColorID, this.color * new Color(1, 1, 1, 1 - t));

                t += Time.deltaTime;
                yield return null;
            }

            if (!this.danger) this.ChangePoints(-1);
            
            Destroy(this.gameObject);
        }

        private void Update()
        {
            Transform t = this.transform;
            t.position += this.moveDir * (this.speed * Time.deltaTime);
            t.LookAt(this.playerPos);

            if (Vector3.Distance(t.position, this.playerPos) > 1.35f) return;

            if (!this.danger) this.ChangePoints(-1);

            ParticleSystem particle = Instantiate(this.particlePrefab).GetComponent<ParticleSystem>();
            particle.transform.position = t.position;
            ParticleSystem.MainModule particleSystemMain = particle.main;
            particleSystemMain.startColor = this.color;

            Destroy(this.gameObject);
        }

        public void Hit(Color c)
        {
            if (this.bonus)
            {
                this.ChangePoints(5);

                Destroy(this.gameObject);
                return;
            }

            if (!this.color.Equals(c) && !this.danger) return;

            this.ChangePoints(this.danger ? -1 : 1);

            Destroy(this.gameObject);
        }

        private void ChangePoints(int change)
        {
            GameObject obj = Instantiate(this.pointDisplay);
            Transform t = this.transform;
            obj.transform.position = t.position;
            obj.transform.LookAt(obj.transform.position - t.forward);
            obj.GetComponent<TextMeshPro>().text = (change > 0 ? "+" : "-") +
                                                   (Mathf.Abs(change) == 1 ? "" : Mathf.Abs(change).ToString());

            this.setup.hits += change;
        }
    }
}