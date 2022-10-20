using UnityEngine;

namespace Idea_5
{
    public class Bullet : MonoBehaviour
    {
        public Color color;
        public GameObject particlePrefab;
        [SerializeField] private float speed;

        private float currentLife;

        private void Update()
        {
            Transform t = this.transform;
            t.position += t.forward * (this.speed * Time.deltaTime);

            this.currentLife += Time.deltaTime;

            if (this.currentLife > 10)
                Destroy(this.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            ParticleSystem system = Instantiate(this.particlePrefab).GetComponent<ParticleSystem>();
            system.transform.position = this.transform.position;
            ParticleSystem.MainModule main = system.main;
            main.startColor = this.color;

            if (collision.gameObject.GetComponent<Target>() is { } target)
                target.Hit(this.color);

            Destroy(this.gameObject);
        }
    }
}