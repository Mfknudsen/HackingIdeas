using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Idea_3
{
    public class Food : VrGrabObject
    {
        public Color color;
        public float speed;
        [SerializeField] private GameObject particles;
        private readonly List<Mouth> closeMouths = new List<Mouth>();
        private Vector3 spawnPoint;
        private static readonly int ColorID = Shader.PropertyToID("_Color");

        protected override void Start()
        {
            base.Start();

            this.spawnPoint = this.transform.position;
        }

        private void Update()
        {
            if (this.grabbed) return;

            Transform t = this.transform;
            t.position += t.forward * (this.speed * Time.deltaTime);

            if (Mathf.Abs(t.position.y - this.spawnPoint.y) > 1.2f)
                Destroy(this.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!(other.GetComponentInParent<Mouth>() is { } mouth)) return;

            if (!this.closeMouths.Contains(mouth))
                this.closeMouths.Add(mouth);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!(other.GetComponentInParent<Mouth>() is { } mouth)) return;

            this.closeMouths.Remove(mouth);
        }

        protected override void OnGrab()
        {
        }

        protected override void OnRelease()
        {
            this.transform.LookAt(this.transform.position - Vector3.up);

            if (this.closeMouths.Count == 0) return;

            Vector3 pos = this.transform.position;

            Mouth m = this.closeMouths.OrderBy(m => Vector3.Distance(m.transform.position, pos)).First();

            if (m != null)
            {
                ParticleSystem particle = Instantiate(this.particles).GetComponent<ParticleSystem>();
                particle.transform.position = pos;
                ParticleSystem.MainModule settings = particle.main;
                settings.startColor = m.color == this.color ? Color.white : Color.black;

                m.Eat(this);
            }

            Destroy(this.gameObject);
        }

        public void SetColor(Color c)
        {
            this.color = c;
            foreach (Renderer r in this.GetComponentsInChildren<Renderer>())
            {
                Material mat = r.material;
                mat.EnableKeyword("_Color");
                mat.SetColor(ColorID, c);
            }
        }
    }
}