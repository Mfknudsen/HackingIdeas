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
            Transform t = transform;
            t.position += t.forward * (speed * Time.deltaTime);

            currentLife += Time.deltaTime;

            if (currentLife > 10)
                Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            ParticleSystem system = Instantiate(this.particlePrefab).GetComponent<ParticleSystem>();
            system.transform.position = transform.position;
            ParticleSystem.MainModule main = system.main;
            main.startColor = this.color;

            if (collision.gameObject.GetComponent<Target>() is { } target)
                target.Hit(this.color);

            Destroy(gameObject);
        }
    }
}