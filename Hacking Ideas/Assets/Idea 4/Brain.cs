using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Idea_4
{
    public class Brain : VrGrabObject
    {
        public BrainSurgeryHackSetup setup;
        public readonly List<CutLine> cutLines = new List<CutLine>();
        public float maxDistanceFromLine = .1f;

        public float currentDamage, maxDamage = 20;

        [SerializeField] private Material lineMat;
        [SerializeField] private GameObject indicator;
        [SerializeField] private TextMeshPro textMeshPro;

        private int knifePartsTouching;
        private Camera cam;

        private Transform grabbedBy;
        private Vector3 prePosition;

        protected override void Start()
        {
            this.cam = Camera.main;
        }

        private void Update()
        {
            if (this.cam != null)
            {
                Vector3 position = this.textMeshPro.transform.position;
                Vector3 dir = this.cam.transform.position - position;
                dir = new Vector3(dir.x, 0, dir.z);
                this.textMeshPro.transform.LookAt(position - dir, Vector3.up);
            }
            else
                this.cam = Camera.main;

            this.textMeshPro.text = Mathf.FloorToInt((this.maxDamage - this.currentDamage) * 10) / 10f +
                                    " / " + this.maxDamage;

            if (this.cutLines.All(c => c.done))
                this.textMeshPro.text = "Victory!";
            else if (this.currentDamage >= this.maxDamage) this.textMeshPro.text = "Dead!";

            if (this.grabbedBy == null || this.cam == null) return;
            {
                if (this.grabbedBy.position == this.prePosition) return;

                Vector3 center = transform.position;
                Vector3 curPos = this.grabbedBy.position;

                Vector2 camPos = this.cam.WorldToScreenPoint(curPos),
                    preCamPos = this.cam.WorldToScreenPoint(this.prePosition);

                Vector2 dir = camPos - preCamPos;

                transform.RotateAround(center, this.cam.transform.up, -dir.x * .3f);
                transform.RotateAround(center, this.cam.transform.right, dir.y * .3f);

                this.prePosition = curPos;
            }
        }

        protected override void OnGrab()
        {
            Transform t = transform;
            this.grabbedBy = t.parent;
            this.prePosition = this.grabbedBy.position;
            t.parent = this.setup.transform;
        }

        protected override void OnRelease()
        {
            this.grabbedBy = null;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (!(other.GetComponentInParent<Knife>() is { } knife)) return;

            this.knifePartsTouching++;
            knife.touchingBrain = true;
        }

        private void OnTriggerExit(Collider other)
        {
            Knife knife = other.GetComponentInParent<Knife>();

            if (knife == null) return;

            this.knifePartsTouching--;

            if (this.knifePartsTouching == 0)
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

                this.cutLines.Add(new GameObject("Cutline").AddComponent<CutLine>().Setup(startPoint, endPoint,
                    between.ToArray(),
                    Instantiate(this.indicator), this));
            }

            foreach (CutLine cutLine in this.cutLines)
            {
                GameObject obj = cutLine.gameObject;
                obj.transform.parent = t;
                obj.transform.position = center;

                obj.transform.LookAt(center + (cutLine.startPoint - center) + (cutLine.endPoint - center));

                LineRenderer lineRenderer = obj.AddComponent<LineRenderer>();
                cutLine.lineRenderer = lineRenderer;

                lineRenderer.alignment = LineAlignment.TransformZ;
                lineRenderer.positionCount = cutLine.between.Length + 2;
                lineRenderer.startColor = Color.cyan;
                lineRenderer.endColor = Color.cyan;
                lineRenderer.startWidth = .003f;
                lineRenderer.endWidth = .003f;
                lineRenderer.material = this.lineMat;

                List<Vector3> positions = new List<Vector3> { cutLine.startPoint };
                positions.AddRange(cutLine.between);
                positions.Add(cutLine.endPoint);

                List<Transform> transforms = new List<Transform>();

                for (int i = 0; i < positions.Count; i++)
                {
                    GameObject o = new GameObject("T")
                    {
                        transform =
                        {
                            parent = cutLine.transform,
                            position = positions[i]
                        }
                    };

                    transforms.Add(o.transform);

                    lineRenderer.SetPosition(i, positions[i] + (positions[i] - center).normalized * .002f);
                }

                cutLine.pointTransforms = transforms;
                cutLine.points = positions;
                cutLine.indicator.transform.position = cutLine.startPoint;
                cutLine.indicator.transform.LookAt(cutLine.startPoint + (center - cutLine.startPoint));
            }
        }

        private Vector3 RandomPointOnBrain(Vector3 center)
        {
            Vector3 checkPoint = center + 5 * new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f));

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
}