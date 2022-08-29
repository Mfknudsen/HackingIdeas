using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Idea_3
{
    public class Mouth : MonoBehaviour
    {
        [HideInInspector] public FeedingHackSetup setup;

        public TextMeshPro textMesh;

        public Color color;

        public bool done;
        public int min, max, current;
        private static readonly int ColorID = Shader.PropertyToID("_Color");

        private void Start()
        {
            if (Camera.main != null)
                transform.LookAt(Camera.main.transform.position);
        }

        public void Eat(Food food)
        {
            if (this.done) return;

            if (this.color == food.color)
            {
                this.current++;

                List<Color> l = this.setup.colors.ToList();

                l.Remove(this.color);

                SetColor(l[Random.Range(0, this.setup.colors.Count - 1)]);
            }
            else
                this.current--;

            if (this.current == this.max)
            {
                this.textMesh.text = "Full";
                this.done = true;
            }
            else if (this.current == this.min)
            {
                this.textMesh.text = "Dead";
                this.done = true;
            }
            else
            {
                if (this.current >= this.max / 2)
                    this.textMesh.text = "Little Hungry";
                else if (this.current < this.min / 2)
                    this.textMesh.text = "Very Hungry";
            }
        }

        public void SetColor(Color c)
        {
            this.color = c;
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                Material mat = r.material;
                mat.EnableKeyword("_Color");
                mat.SetColor(ColorID, c);
            }
        }
    }
}