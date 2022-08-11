using System.Collections.Generic;
using UnityEngine;

namespace Idea_1
{
    public enum Controls { V1, V2 }

    public class HackLinesSetup : MonoBehaviour
    {
        #region Values

        [Header("Lines")]
        [SerializeField] private int lineCount = 5;
        [SerializeField] private float maxAngle = 5;
        //- Minimum distance between two points in a line
        [SerializeField] private float minPointDist = 1;
        [Range(.2f, 1f), SerializeField] private float bezierDist = .2f;
        [SerializeField, HideInInspector] private List<Line> allLines = new List<Line>();

        [Space, Header("Connections")]
        [Range(0f, 1f), SerializeField] private float chanceOfSelfConnect = .1f;
        [SerializeField] private int maxSelfConnects = 2;
        private int currentSelfConnects = 0;

        [Space, Header("Visual")]
        [SerializeField] private GameObject gatePrefab;
        [SerializeField] private Material lineMat;
        [SerializeField] private float startSize = .05f, endSize = .05f;
        [SerializeField] private Color[] colors = { Color.white, Color.red, Color.blue, Color.green, Color.grey, Color.yellow, Color.cyan, Color.magenta };
        [SerializeField] private Transform generationReferencePoint;

        [Space, Header("Goal")]
        [SerializeField] private GameObject temp;
        [SerializeField] private int sequenceLength = 5;
        [SerializeField] private Transform goalSequenceTransform, currentSequenceTransform;
        [SerializeField] private List<Color> goalSequence = new List<Color>(), currentSequence = new List<Color>();
        [SerializeField] private Material sequenceMat;

        [Space, Header("Player")]
        public Transform playerArrow;
        public Controls playerControls;

#if UNITY_EDITOR
        [Space, Header("Debug")]
        public bool displayBounds;
        public bool displayLineGizmos;
        public Color frontColor = Color.green, backColor = Color.blue;
#endif

        #endregion

        #region MonoBehaviour

        private void OnValidate()
        {
            //Update allready spawned lines
            foreach (LineRenderer r in GetComponentsInChildren<LineRenderer>())
            {
                r.startWidth = this.startSize;
                r.endWidth = this.endSize;
                r.material = this.lineMat;

                Line l = r.GetComponent<Line>();

                if (l == null) continue;

                l.DisplayGizmo = this.displayLineGizmos;
                l.frontColor = this.frontColor;
                l.backColor = this.backColor;

                r.startColor = l.color;
                r.endColor = l.color;
            }

#if UNITY_EDITOR
            foreach (Line l in GetComponentsInChildren<Line>())
            {
                l.DisplayGizmo = this.displayLineGizmos;
                l.frontColor = this.frontColor;
                l.backColor = this.backColor;
            }
#endif
        }

        private void Start()
        {
            List<GameObject> toDelete = new List<GameObject>();
            foreach (Transform t in this.goalSequenceTransform)
                toDelete.Add(t.gameObject);

            foreach (GameObject o in toDelete)
                DestroyImmediate(o);

            for (int i = 0; i < this.goalSequence.Count; i++)
            {
                Color c = this.goalSequence[i];
                GameObject obj = Instantiate(this.temp);
                obj.transform.parent = this.goalSequenceTransform;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.position += this.goalSequenceTransform.forward * (i * .1f);
                obj.transform.rotation = this.goalSequenceTransform.rotation;
                Material mat = obj.GetComponent<Renderer>().material = new Material(sequenceMat);
                mat.EnableKeyword("_Color");
                mat.SetColor("_Color", c);
            }

            ResetCurrentSequence();
        }

        #endregion

        #region Getters

        public Line GetRootLine()
        {
            return this.allLines[0];
        }

        #endregion

        #region In

        public void TryAddToCurrentSequence(Key key, Color toAdd)
        {
            if (this.goalSequence[this.currentSequence.Count] != toAdd)
            {
                key.ResetStartPosition();
                ResetCurrentSequence();
                return;
            }

            this.currentSequence.Add(toAdd);

            GameObject obj = Instantiate(this.temp);
            obj.transform.parent = this.currentSequenceTransform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.position += this.currentSequenceTransform.forward * ((this.currentSequence.Count - 1) * .1f);
            obj.transform.rotation = this.currentSequenceTransform.rotation;
            Material mat = obj.GetComponent<Renderer>().material = new Material(sequenceMat);
            mat.EnableKeyword("_Color");
            mat.SetColor("_Color", toAdd);

            if (this.currentSequence.Count == this.goalSequence.Count)
                key.SetCurrentLine(GetComponentInChildren<EndLine>(), TransferTo.FRONT, 1);
        }

        #endregion

        #region Internal

        [Sirenix.OdinInspector.Button]
        private void CreateNewSetup()
        {
            //Delete previous lines
            foreach (Line l in transform.GetComponentsInChildren<Line>())
            {
                if (l.GetType() != typeof(EndLine))
                    DestroyImmediate(l.gameObject);
            }

            List<Vector3> usedAngles = new List<Vector3>();
            allLines.Clear();
            //Setup new lines
            for (int i = 0; i < lineCount; i++)
            {
                GameObject obj = new GameObject("Line");
                obj.transform.parent = transform;
                obj.transform.localPosition = Vector3.zero;

                Line l = obj.AddComponent<Line>();
                l.setup = this;
                this.allLines.Add(l);

                GameObject lineRend = new GameObject("Rendere");
                lineRend.transform.parent = obj.transform;
                lineRend.AddComponent<LineRenderer>();
                lineRend.AddComponent<PathCreator>();
                lineRend = new GameObject("Rendere");
                lineRend.transform.parent = obj.transform;
                lineRend.AddComponent<LineRenderer>();
                lineRend.AddComponent<PathCreator>();

                foreach (LineRenderer r in obj.GetComponentsInChildren<LineRenderer>())
                {
                    r.material = this.lineMat;
                    r.startWidth = .1f;
                    r.endWidth = .1f;

                    if (i < this.colors.Length)
                        l.color = colors[i];

                    r.startColor = l.color;
                    r.endColor = l.color;
                }

                Vector3 front = Vector3.zero,
                    middle = Vector3.zero,
                    back = Vector3.zero,
                    fNormal = Vector3.zero,
                    mNormal = Vector3.zero,
                    bNormal = Vector3.zero;

                Vector3 angle = new Vector3(
                    -1,
                    Random.Range(-this.maxAngle, this.maxAngle),
                    Random.Range(-this.maxAngle * 2, this.maxAngle * 2));

                RaycastHit hit;

                while (!IsValidAngle(angle, usedAngles))
                    angle = new Vector3(
                        -1,
                        Random.Range(-this.maxAngle, this.maxAngle),
                        Random.Range(-this.maxAngle * 2, this.maxAngle * 2));

                if (Physics.Raycast(transform.position, angle.normalized, out hit, 100))
                {
                    front = hit.point;
                    fNormal = hit.normal;
                }

                usedAngles.Add(angle);

                while (!IsValidAngle(angle, usedAngles))
                    angle = new Vector3(
                        -1,
                        Random.Range(-this.maxAngle, this.maxAngle),
                        Random.Range(-this.maxAngle * 2, this.maxAngle * 2));

                if (Physics.Raycast(transform.position, angle.normalized, out hit, 100))
                {
                    back = hit.point;
                    bNormal = hit.normal;
                }

                usedAngles.Add(angle);

                while (!IsValidAngle(angle, usedAngles))
                    angle = new Vector3(
                        -1,
                        Random.Range(-this.maxAngle, this.maxAngle),
                        Random.Range(-this.maxAngle * 2, this.maxAngle * 2));

                if (Physics.Raycast(transform.position, angle.normalized, out hit, 100))
                {
                    middle = hit.point;
                    mNormal = hit.normal;
                }

                usedAngles.Add(angle);

                l.front = front;
                l.middle = middle;
                l.back = back;
                l.fNormal = fNormal;
                l.mNormal = mNormal;
                l.bNormal = bNormal;

                GameObject frontGate = Instantiate(this.gatePrefab, l.transform),
                    middleGate = Instantiate(this.gatePrefab, l.transform),
                    backGate = Instantiate(this.gatePrefab, l.transform);

                frontGate.transform.position = front;
                middleGate.transform.position = middle;
                backGate.transform.position = back;

                frontGate.transform.LookAt(front + fNormal);
                middleGate.transform.LookAt(middle + mNormal);
                backGate.transform.LookAt(back + bNormal);
                frontGate.transform.position += frontGate.transform.forward * 0.2f;
                middleGate.transform.position += middleGate.transform.forward * 0.2f;
                backGate.transform.position += backGate.transform.forward * 0.2f;

                l.frontGate = frontGate;
                l.middleGate = middleGate;
                l.backGate = backGate;

                l.SetupExtraPoints(this.generationReferencePoint, this.bezierDist);
            }

            //Setup connections between points
            EndLine end = transform.GetComponentInChildren<EndLine>();
            this.currentSelfConnects = 0;
            List<Line> lines = new List<Line>();
            lines.AddRange(transform.GetComponentsInChildren<Line>());
            //- Dont want to connect the end
            lines.Remove(end);

            //- Each line connect independenly without caring of others connections
            foreach (Line l in lines)
            {
                List<Line> others = new List<Line>();
                others.AddRange(lines);
                others.Remove(l);

                l.frontTransfer = others[Random.Range(0, others.Count)];
                l.frontTransferTo = Random.Range(0, 3);
                others.Remove(l.frontTransfer);
                l.middleTransfer = others[Random.Range(0, others.Count)];
                l.middleTransferTo = Random.Range(0, 3);
                others.Remove(l.middleTransfer);
                l.backTransfer = others[Random.Range(0, others.Count)];
                l.backTransferTo = Random.Range(0, 3);

                l.frontGate.GetComponentInChildren<Light>().color = l.frontTransfer.color;
                l.middleGate.GetComponentInChildren<Light>().color = l.middleTransfer.color;
                l.backGate.GetComponentInChildren<Light>().color = l.backTransfer.color;
            }

            //Validate connections by insuring all can be reached from the first connected line.
            int runs = 0;
            //- Always returns true when validating but use limited runs to insure while loop is not infinit. 
            while (runs < 100)
            {
                if (ValidateConnections(lines))
                    break;

                runs++;
            }

            ShuffleGoalSequence();
        }

        [Sirenix.OdinInspector.Button]
        private void ShuffleGoalSequence()
        {
            //Set the first line to the ball.
            GetComponentInChildren<Key>().currentLine = allLines[0];

            //Setup goal sequence       
            this.goalSequence.Clear();
            this.currentSequence.Clear();

            Line check = allLines[0];
            for (int i = 0; i < this.sequenceLength; i++)
            {
                this.goalSequence.Add(check.color);

                Line checkFrom = check;

                float r = Random.Range(0f, 1f);
                if (r < .35f)
                    check = checkFrom.frontTransfer;
                else if (r < .7f)
                    check = checkFrom.middleTransfer;
                else
                    check = checkFrom.backTransfer;

                if (this.goalSequence.Count < 2)
                    continue;

                if (check.color == this.goalSequence[this.goalSequence.Count - 2])
                {
                    if (r < .35f)
                        check = checkFrom.middleTransfer;
                    else if (r < .7f)
                        check = checkFrom.backTransfer;
                    else
                        check = checkFrom.frontTransfer;
                }
            }

            this.currentSequence.Add(this.goalSequence[0]);
        }

        private bool IsValidAngle(Vector3 angle, List<Vector3> preAngles)
        {
            foreach (Vector3 p in preAngles)
            {
                if (Vector3.Angle(angle, p) < 10)
                    return false;
            }

            return true;
        }

        private Vector3 GetValidPointPosition(Vector3 otherPos, List<Vector3> usedPos)
        {
            //Valid point position is a position with a distance from the other point position higher then the minimum distance.
            Vector3 result = Vector3.zero;
            Vector3 angle = new Vector3(-1, Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            if (Physics.Raycast(transform.position, angle.normalized, out RaycastHit hit, 100))
                result = hit.point;

            float lowestDist = 10;

            for (int i = 0; i < usedPos.Count; i++)
            {
                float dist = Vector3.Distance(result, usedPos[i]);
                if (dist < lowestDist)
                    lowestDist = dist;

                if (dist <= this.minPointDist)
                    break;
            }

            return Vector3.Distance(result, otherPos) > this.minPointDist && result != Vector3.zero ? result : GetValidPointPosition(otherPos, usedPos);
        }

        private bool ValidateConnections(List<Line> lines)
        {
            //From the first line where the ball start, it will check if all other lines can be reached.
            List<Line> toCheck = new List<Line>() { lines[0] }, checkedLines = new List<Line>();
            while (toCheck.Count > 0)
            {
                Line l = toCheck[0];
                toCheck.RemoveAt(0);

                if (l == null)
                    continue;

                if (checkedLines.Contains(l))
                    continue;

                checkedLines.Add(l);

                if (!checkedLines.Contains(l.frontTransfer))
                    toCheck.Add(l.frontTransfer);

                if (!checkedLines.Contains(l.backTransfer))
                    toCheck.Add(l.backTransfer);
            }

            //If the reached number of lines does not match the actual number of lines then take a line not connected and connect it to a line that is connected.
            if (checkedLines.Count != lines.Count)
            {
                //l is not connected to key
                Line l = null;
                while (l == null)
                {
                    Line temp = lines[Random.Range(0, lines.Count)];

                    if (checkedLines.Contains(temp))
                        continue;

                    l = temp;
                }

                //cL is connected to key
                Line cL = checkedLines[Random.Range(0, checkedLines.Count)];

                if (Random.Range(0, 2) == 1)
                    cL.frontTransfer = l;
                else
                    cL.backTransfer = l;

                return false;
            }

            return true;
        }

        private void ResetCurrentSequence()
        {
            List<GameObject> toDelete = new List<GameObject>();
            foreach (Transform t in this.currentSequenceTransform)
                toDelete.Add(t.gameObject);

            foreach (GameObject o in toDelete)
                DestroyImmediate(o);

            this.currentSequence.Clear();
            this.currentSequence.Add(this.goalSequence[0]);

            Color c = this.currentSequence[0];
            GameObject obj = Instantiate(this.temp);
            obj.transform.parent = this.currentSequenceTransform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.rotation = this.currentSequenceTransform.rotation;

            Material mat = obj.GetComponent<Renderer>().material = new Material(sequenceMat);
            mat.EnableKeyword("_Color");
            mat.SetColor("_Color", c);
        }

        #endregion
    }
}