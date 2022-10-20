using System.Collections;
using TMPro;
using UnityEngine;

namespace Idea_5
{
    public class PointDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMeshPro;
        [SerializeField] private float time;

        private IEnumerator Start()
        {
            float t = 0;
            while (t < this.time)
            {
                this.textMeshPro.color *= new Color(1, 1, 1, 1 - t);

                t += 1 / this.time * Time.deltaTime;
                yield return null;
            }

            Destroy(this.gameObject);
        }
    }
}