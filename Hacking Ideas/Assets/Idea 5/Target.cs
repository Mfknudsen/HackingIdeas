using UnityEngine;

namespace Idea_5
{
    public class Target : MonoBehaviour
    {
        public Color color;
        public HordeShooterHackSetup setup;
        public float life;
        private float current;

        private void Update()
        {
            Transform t = transform;
            t.position += t.forward * (.25f * Time.deltaTime);

            current += Time.deltaTime;

            if (current < life) return;

            Destroy(gameObject);
        }

        public void Hit(Color c)
        {
            if (!this.color.Equals(c)) return;

            this.setup.hits++;

            Destroy(gameObject);
        }
    }
}