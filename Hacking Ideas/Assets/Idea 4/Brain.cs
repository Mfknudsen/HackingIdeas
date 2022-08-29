using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Idea_4
{
    public class Brain : MonoBehaviour
    {
        public readonly List<CutLine> cutLines = new List<CutLine>();
        public float maxDistanceFromLine = .1f;

        public float currentDamage, maxDamage = 20;

        [SerializeField] private Material lineMat;
        [SerializeField] private GameObject indicator;

        private void Update()
        {
            if (this.currentDamage >= this.maxDamage)
                Debug.Log("Dead");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<Knife>() is { } knife)
                knife.touchingBrain = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponentInParent<Knife>() is { } knife)
                knife.touchingBrain = false;
        }

        public void Setup(int lineCount)
        {
            Transform t = transform;
            Vector3 center = t.position;
            for (int i = 0; i < lineCount; i++)
            {
                Vector3 startPoint = RandomPointOnBrain(center), endPoint = RandomPointOnBrain(center);
                Vector3 startDir = startPoint - center, endDir = endPoint - center;
                startDir *= 5;
                endDir *= 5;

                List<Vector3> between = new List<Vector3>();
                int count = Mathf.FloorToInt(Vector3.Angle(startPoint - center, endPoint - center) / 5);

                for (int j = 1; j < count; j++)
                    between.Add(
                        PointOnObject(Vector3.Lerp(
                                startPoint + startDir,
                                endPoint + endDir,
                                1f / count * j),
                            center));

                cutLines.Add(new CutLine(startPoint, endPoint, between.ToArray(), Instantiate(indicator)));
            }

            foreach (SphereCollider s in GetComponents<SphereCollider>())
            {
                if (!s.isTrigger)
                    Destroy(s);
            }

            foreach (CutLine cutLine in cutLines)
            {
                GameObject obj = new GameObject("Cut Line")
                {
                    transform =
                    {
                        parent = t.parent,
                        position = t.position
                    }
                };

                Vector3 position = obj.transform.position;
                obj.transform.LookAt(position + (cutLine.startPoint - center) + (cutLine.endPoint - center));

                LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
                cutLine.lineRenderer = lineRenderer;

                lineRenderer.alignment = LineAlignment.TransformZ;
                lineRenderer.positionCount = cutLine.between.Length + 2;
                lineRenderer.startColor = Color.cyan;
                lineRenderer.endColor = Color.cyan;
                lineRenderer.startWidth = .003f;
                lineRenderer.endWidth = .003f;
                lineRenderer.material = lineMat;

                List<Vector3> positions = new List<Vector3> { cutLine.startPoint };
                positions.AddRange(cutLine.between);
                positions.Add(cutLine.endPoint);

                for (int i = 0; i < positions.Count; i++)
                    lineRenderer.SetPosition(i, positions[i] + (positions[i] - center).normalized * .001f);

                cutLine.indicator.transform.position = cutLine.startPoint;
                cutLine.indicator.transform.LookAt(cutLine.startPoint + (center - cutLine.startPoint));
            }
        }

        private Vector3 RandomPointOnBrain(Vector3 center)
        {
            Vector3 checkPoint = center + new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f)) * 2;

            Vector3 dir = center - checkPoint;

            // ReSharper disable once Unity.PreferNonAllocApi
            RaycastHit[] hits = Physics.RaycastAll(checkPoint, dir, Mathf.Infinity);
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == gameObject)
                    return hit.point;
            }

            return Vector3.zero;
        }

        private Vector3 PointOnObject(Vector3 checkPoint, Vector3 center)
        {
            Vector3 dir = center - checkPoint;

            // ReSharper disable once Unity.PreferNonAllocApi
            RaycastHit[] hits = Physics.RaycastAll(checkPoint, dir, Mathf.Infinity);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == gameObject)
                    return hit.point;
            }

            return Vector3.zero;
        }
    }

    public class CutLine
    {
        public readonly Vector3 startPoint, endPoint;

        public readonly Vector3[] between;

        public readonly GameObject indicator;

        public LineRenderer lineRenderer;

        public bool done;

        private int currentIndex, nextIndex;

        private List<Vector3> points = new List<Vector3>();

        public CutLine(Vector3 startPoint, Vector3 endPoint, Vector3[] between, GameObject indicator)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.between = between;
            this.indicator = indicator;

            this.indicator.transform.position = this.startPoint;
        }

        public void Selected()
        {
            this.points = new List<Vector3> { this.startPoint };
            this.points.AddRange(this.between);
            this.points.Add(this.endPoint);

            this.currentIndex = 0;
            this.nextIndex = 1;

            this.done = false;

            this.indicator.SetActive(false);
        }

        public void Deselect()
        {
            if (this.done) return;

            this.indicator.SetActive(true);

            this.points = new List<Vector3> { this.startPoint };
            this.points.AddRange(this.between);
            this.points.Add(this.endPoint);

            this.lineRenderer.positionCount = this.points.Count;

            for (int i = 0; i < this.points.Count; i++)
                this.lineRenderer.SetPosition(i, this.points[i]);
        }

        public void Update(Knife knife)
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

            if (Vector3.Distance(knifePos, this.points[this.nextIndex]) > .05f) return;

            this.points.RemoveAt(0);

            this.currentIndex++;
            this.nextIndex++;

            this.lineRenderer.positionCount = this.points.Count;
            for (int i = 0; i < this.points.Count; i++)
                this.lineRenderer.SetPosition(i, this.points[i]);

            if (this.currentIndex == this.points.Count)
                this.done = true;
        }

        private static float DistanceToLineFromPoint(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 d = (lineEnd - lineStart) / Vector3.Distance(lineEnd, lineStart);
            Vector3 v = point - lineStart;
            float dot = Vector3.Distance(v, d);
            Vector3 p = lineStart + dot * d;
            return Vector3.Distance(p, point);
        }
    }
}