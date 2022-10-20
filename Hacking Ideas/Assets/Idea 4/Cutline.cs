#region Packages

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Idea_4
{
    public class CutLine : MonoBehaviour
    {
        public Vector3 startPoint;
        public Vector3 endPoint;
        public Vector3[] between;
        public GameObject indicator;
        public LineRenderer lineRenderer;
        public bool done;
        public List<Transform> pointTransforms = new List<Transform>();
        public List<Vector3> points = new List<Vector3>();

        private Brain brain;
        private int currentIndex, nextIndex;

        private void Update()
        {
            if (this.done) return;

            if (this.pointTransforms[0].position.Equals(this.points[0])) return;

            int diffIndex = this.pointTransforms.Count - this.points.Count;
            for (int i = 0; i < this.points.Count; i++) this.points[i] = this.pointTransforms[i + diffIndex].position;

            for (int i = 0; i < this.points.Count; i++) this.lineRenderer.SetPosition(i, this.points[i] + (this.points[i] - this.transform.position).normalized * .002f);
            this.indicator.transform.LookAt(this.startPoint + (this.transform.position - this.startPoint));

            this.indicator.transform.position = this.points[this.currentIndex];
            this.indicator.transform.LookAt(this.points[this.currentIndex] +
                                            (this.brain.transform.position - this.points[this.currentIndex]));

            this.startPoint = this.pointTransforms[0].position;
        }

        public CutLine Setup(Vector3 startPoint, Vector3 endPoint, Vector3[] between, GameObject indicator, Brain brain)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.between = between;
            this.indicator = indicator;
            this.brain = brain;

            this.indicator.transform.parent = this.transform;
            this.indicator.transform.position = this.startPoint;

            return this;
        }

        public void Selected()
        {
            this.points.Clear();
            foreach (Transform t in this.pointTransforms)
                this.points.Add(t.position);

            this.currentIndex = 0;
            this.nextIndex = 1;

            this.done = false;
        }

        public void Deselect()
        {
            if (this.done) return;

            this.points.Clear();
            foreach (Transform t in this.pointTransforms)
                this.points.Add(t.position);

            this.lineRenderer.positionCount = this.points.Count;

            for (int i = 0; i < this.points.Count; i++)
                this.lineRenderer.SetPosition(i, this.points[i]);

            this.indicator.transform.position = this.startPoint;
            this.indicator.transform.LookAt(this.startPoint + (this.brain.transform.position - this.startPoint));

            this.currentIndex = 0;
            this.nextIndex = 1;
        }

        public void UpdateCut(Knife knife)
        {
            Vector3 knifePos = knife.cutPoint.position;

            if (this.done)
            {
                if (Vector3.Distance(knifePos, this.endPoint) > knife.setup.brain.maxDistanceFromLine)
                    knife.setup.brain.currentDamage += knife.setup.damagePerSecond * Time.deltaTime;

                return;
            }

            if (DistanceToLineFromPoint(knifePos,
                    this.points[this.currentIndex],
                    this.points[this.nextIndex]) >
                knife.setup.brain.maxDistanceFromLine)
                knife.setup.brain.currentDamage += knife.setup.damagePerSecond * Time.deltaTime;

            if (Vector3.Distance(knifePos, this.points[this.currentIndex]) > .02f) return;

            this.points.RemoveAt(0);

            this.currentIndex++;
            this.nextIndex++;

            if (this.nextIndex >= this.points.Count)
            {
                this.indicator.SetActive(false);
                this.lineRenderer.positionCount = 0;
                this.done = true;
                return;
            }

            this.indicator.transform.position = this.points[this.currentIndex];
            this.indicator.transform.LookAt(this.points[this.currentIndex] +
                                            (this.brain.transform.position - this.points[this.currentIndex]));

            this.lineRenderer.positionCount = this.points.Count;
            for (int i = 0; i < this.points.Count; i++)
                this.lineRenderer.SetPosition(i, this.points[i]);
        }

        private static float DistanceToLineFromPoint(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            float s1 = -lineEnd.y + lineStart.y;
            float s2 = lineEnd.x - lineStart.x;
            return Mathf.Abs((point.x - lineStart.x) * s1 + (point.y - lineStart.y) * s2) /
                   Mathf.Sqrt(s1 * s1 + s2 * s2);
        }
    }
}