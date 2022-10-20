using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Idea_1
{
    /// <summary>
    /// Enum used for decide the players controls for the minigame.
    /// How the player decide the key should move.
    /// </summary>
    public enum Controls
    {
        V1,
        V2
    }

    /// <summary>
    /// Setup and control for the Hack Line minigame.
    /// </summary>
    public class HackLinesSetup : MonoBehaviour
    {
        #region Values

        /// <summary>
        /// How many lines should the minigame consist of.
        /// </summary>
        [Header("Lines")] [SerializeField] private int lineCount = 5;

        /// <summary>
        /// Angle used to spawn lines.
        /// </summary>
        [SerializeField] private float maxAngle = 5;

        /// <summary>
        /// When calculating the bezier curves the distance determines the total point count along the lines.
        /// Lower means more points and smoother rounding of the curves.
        /// </summary>
        [Range(.2f, 1f), SerializeField] private float bezierDist = .2f;

        /// <summary>
        /// The total list of generated lines witch the key can move on.
        /// </summary>
        [SerializeField, HideInInspector] private List<Line> allLines = new List<Line>();

        /// <summary>
        /// The prefab for the gates.
        /// Gates are located at the end of each line part.
        /// When the key reaches a gate it will be transported.
        /// </summary>
        [Space, Header("Visual")] [SerializeField]
        private GameObject gatePrefab;

        /// <summary>
        /// The material for the generated lines.
        /// </summary>
        [SerializeField] private Material lineMat;

        /// <summary>
        /// The size for the lines.
        /// Will be used in the line render.
        /// </summary>
        [SerializeField] private float lineSize = .05f;

        /// <summary>
        /// Each line will have a individual color witch will be taken from this array.
        /// </summary>
        [SerializeField] private Color[] colors =
            { Color.white, Color.red, Color.blue, Color.green, Color.grey, Color.yellow, Color.cyan, Color.magenta };

        /// <summary>
        /// Point used to create the middle part of the lines.
        /// </summary>
        [SerializeField] private Transform generationReferencePoint;

        /// <summary>
        /// Prefab used to display the correct sequence and the current sequence.
        /// </summary>
        [Space, Header("Goal")] [SerializeField]
        private GameObject goalDisplayPrefab;

        /// <summary>
        /// The total sequence length for the player to make to win.
        /// </summary>
        [SerializeField] private int sequenceLength = 5;

        /// <summary>
        /// The locations to display the goal and the current sequence.
        /// </summary>
        [SerializeField] private Transform goalSequenceTransform, currentSequenceTransform;

        /// <summary>
        /// The generated goal sequence and the current sequence.
        /// </summary>
        [SerializeField] private List<Color> goalSequence = new List<Color>(), currentSequence = new List<Color>();

        /// <summary>
        /// Material for the sequence display. Allows color to be switched.
        /// </summary>
        [SerializeField] private Material sequenceMat;

        /// <summary>
        /// Arrow used by the player to control the key as it travels along the lines.
        /// </summary>
        [Space, Header("Player")] public Transform playerArrow;

        /// <summary>
        /// Enum for setting the players control.
        /// </summary>
        public Controls playerControls;

        /// <summary>
        /// Shader ID for setting color of the material.
        /// </summary>
        private static readonly int ColorID = Shader.PropertyToID("_Color");

#if UNITY_EDITOR
        //For debugging.
        [Space, Header("Debug")] public bool displayLineGizmos;
        public Color frontColor = Color.green, backColor = Color.blue;
#endif

        #endregion

        #region MonoBehaviour

        /// <summary>
        /// Insure all currently generated lines match the settings.
        /// Settings: Line size, line material, line color.
        /// </summary>
        private void OnValidate()
        {
            foreach (LineRenderer r in this.GetComponentsInChildren<LineRenderer>())
            {
                r.startWidth = this.lineSize;
                r.endWidth = this.lineSize;
                r.material = this.lineMat;

                Line l = r.GetComponent<Line>();

                if (l == null) continue;

                l.displayGizmo = this.displayLineGizmos;
                l.frontColor = this.frontColor;
                l.backColor = this.backColor;

                r.startColor = l.color;
                r.endColor = l.color;
            }

#if UNITY_EDITOR
            foreach (Line l in this.GetComponentsInChildren<Line>())
            {
                l.displayGizmo = this.displayLineGizmos;
                l.frontColor = this.frontColor;
                l.backColor = this.backColor;
            }
#endif
        }

        /// <summary>
        /// Sets the goal sequence and the current sequence.
        /// </summary>
        private void Start()
        {
            //Delete the current goal sequence.
            List<GameObject> toDelete = (from Transform t in this.goalSequenceTransform select t.gameObject).ToList();

            foreach (GameObject o in toDelete)
                DestroyImmediate(o);

            //Setup new goal sequence.
            for (int i = 0; i < this.goalSequence.Count; i++)
            {
                Color c = this.goalSequence[i];
                GameObject obj = Instantiate(this.goalDisplayPrefab, this.goalSequenceTransform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.position += this.goalSequenceTransform.forward * (i * .1f);
                obj.transform.rotation = this.goalSequenceTransform.rotation;
                Material mat = obj.GetComponent<Renderer>().material = new Material(this.sequenceMat);
                mat.EnableKeyword("_Color");
                mat.SetColor(ColorID, c);
            }

            //Reset current sequence.
            this.ResetCurrentSequence();
        }

        #endregion

        #region Getters

        /// <summary>
        /// Get the first line for when the key needs to reset.
        /// </summary>
        /// <returns>The first line where the key first starts</returns>
        public Line GetRootLine() => this.allLines[0];

        #endregion

        #region In

        /// <summary>
        /// When the key enters a gate it will check if it was the right one.
        /// If it is then at to the current sequence.
        /// If not then reset the key.
        /// </summary>
        /// <param name="key">The current key</param>
        /// <param name="toAdd">The color to check an possibly add to current sequence</param>
        public void TryAddToCurrentSequence(Key key, Color toAdd)
        {
            //Check if the new color matches the goal color at the same position.
            if (this.goalSequence[this.currentSequence.Count] != toAdd)
            {
                key.ResetStartPosition();
                this.ResetCurrentSequence();
                return;
            }

            //Add the color to the current sequence.
            this.currentSequence.Add(toAdd);

            GameObject obj = Instantiate(this.goalDisplayPrefab, this.currentSequenceTransform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.position += this.currentSequenceTransform.forward * ((this.currentSequence.Count - 1) * .1f);
            obj.transform.rotation = this.currentSequenceTransform.rotation;
            Material mat = obj.GetComponent<Renderer>().material = new Material(this.sequenceMat);
            mat.EnableKeyword("_Color");
            mat.SetColor(ColorID, toAdd);

            //Check if the player have won.
            if (this.currentSequence.Count == this.goalSequence.Count)
                key.SetCurrentLine(this.GetComponentInChildren<EndLine>(), TransferTo.Front, 1);
        }

        #endregion

        #region Internal

#if UNITY_EDITOR
        /// <summary>
        /// Creates a button in the inspector to create a new setup based on the setting.
        /// </summary>
        [Button]
        private void CreateNewSetup()
        {
            //Delete previous lines
            foreach (Line l in this.transform.GetComponentsInChildren<Line>())
            {
                if (l.GetType() != typeof(EndLine))
                    DestroyImmediate(l.gameObject);
            }

            List<Vector3> usedAngles = new List<Vector3>();
            this.allLines.Clear();

            //Setup new lines
            for (int i = 0; i < this.lineCount; i++)
            {
                GameObject obj = new GameObject("Line")
                {
                    transform =
                    {
                        parent = this.transform,
                        localPosition = Vector3.zero
                    }
                };

                Line l = obj.AddComponent<Line>();
                l.setup = this;
                this.allLines.Add(l);

                GameObject lineRend = new GameObject("Renderer")
                {
                    transform =
                    {
                        parent = obj.transform
                    }
                };
                lineRend.AddComponent<LineRenderer>();
                lineRend.AddComponent<PathCreator>();
                lineRend = new GameObject("Renderer")
                {
                    transform =
                    {
                        parent = obj.transform
                    }
                };
                lineRend.AddComponent<LineRenderer>();
                lineRend.AddComponent<PathCreator>();

                foreach (LineRenderer r in obj.GetComponentsInChildren<LineRenderer>())
                {
                    r.material = this.lineMat;
                    r.startWidth = .1f;
                    r.endWidth = .1f;

                    if (i < this.colors.Length)
                        l.color = this.colors[i];

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

                while (!IsValidAngle(angle, usedAngles))
                    angle = new Vector3(
                        -1,
                        Random.Range(-this.maxAngle, this.maxAngle),
                        Random.Range(-this.maxAngle * 2, this.maxAngle * 2));

                if (Physics.Raycast(this.transform.position, angle.normalized, out RaycastHit hit, 100))
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

                if (Physics.Raycast(this.transform.position, angle.normalized, out hit, 100))
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

                if (Physics.Raycast(this.transform.position, angle.normalized, out hit, 100))
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

                // ReSharper disable once Unity.InefficientPropertyAccess
                frontGate.transform.position += frontGate.transform.forward * 0.2f;
                // ReSharper disable once Unity.InefficientPropertyAccess
                middleGate.transform.position += middleGate.transform.forward * 0.2f;
                // ReSharper disable once Unity.InefficientPropertyAccess
                backGate.transform.position += backGate.transform.forward * 0.2f;

                l.frontGate = frontGate;
                l.middleGate = middleGate;
                l.backGate = backGate;

                //Setup points within the bezier line for the line renderer.
                l.SetupExtraPoints(this.generationReferencePoint, this.bezierDist);
            }

            //Setup connections between points
            EndLine end = this.transform.GetComponentInChildren<EndLine>();
            List<Line> lines = new List<Line>();
            lines.AddRange(this.transform.GetComponentsInChildren<Line>());
            //- Dont want to connect the end
            lines.Remove(end);

            //- Each line connect independently without caring of others connections
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
            //- Always returns true when validating but use limited runs to insure while loop is not infinity. 
            while (runs < 100)
            {
                if (ValidateConnections(lines))
                    break;

                runs++;
            }

            //Create a goal sequence
            this.ShuffleGoalSequence();
        }

        [Button]
        private void ShuffleGoalSequence()
        {
            //Set the first line to the ball.
            this.GetComponentInChildren<Key>().currentLine = this.allLines[0];

            //Clearing sequences.      
            this.goalSequence.Clear();
            this.currentSequence.Clear();

            //Start from root.
            Line check = this.allLines[0];
            for (int i = 0; i < this.sequenceLength; i++)
            {
                //Add current to goal sequence.
                this.goalSequence.Add(check.color);

                Line checkFrom = check;

                //Decide witch is next from current transfer points.
                float r = Random.Range(0f, 1f);
                if (r < .35f)
                    check = checkFrom.frontTransfer;
                else if (r < .7f)
                    check = checkFrom.middleTransfer;
                else
                    check = checkFrom.backTransfer;

                if (this.goalSequence.Count < 2)
                    continue;

                //Insure the previous color is not the same as the current.
                if (check.color != this.goalSequence[this.goalSequence.Count - 2]) continue;

                if (r < .35f)
                    check = checkFrom.middleTransfer;
                else if (r < .7f)
                    check = checkFrom.backTransfer;
                else
                    check = checkFrom.frontTransfer;
            }

            this.currentSequence.Add(this.goalSequence[0]);
        }
#endif

        /// <summary>
        /// When generating a setup the angles used to place points will be checked to insure no other point is already located there.
        /// </summary>
        /// <param name="angle">The angle to check</param>
        /// <param name="preAngles">The previous angles used to create lines</param>
        /// <returns>True if the angle will not result in a new gate being placed on top of another</returns>
        private static bool IsValidAngle(Vector3 angle, IEnumerable<Vector3> preAngles) =>
            preAngles.Any(p => !(Vector3.Angle(angle, p) < 10));

        /// <summary>
        /// Insure all lines are connected.
        /// </summary>
        /// <param name="lines">All current lines</param>
        /// <returns>True if all line transfer points are connected to a line and not line only leads to itself.</returns>
        private static bool ValidateConnections(IReadOnlyList<Line> lines)
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

        /// <summary>
        /// Resets the current sequence.
        /// </summary>
        private void ResetCurrentSequence()
        {
            //Delete and clear the current sequence.
            List<GameObject> toDelete =
                (from Transform t in this.currentSequenceTransform select t.gameObject).ToList();

            foreach (GameObject o in toDelete)
                DestroyImmediate(o);

            this.currentSequence.Clear();

            //Add the root and set it's color.
            this.currentSequence.Add(this.goalSequence[0]);

            GameObject obj = Instantiate(this.goalDisplayPrefab, this.currentSequenceTransform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.rotation = this.currentSequenceTransform.rotation;

            Material mat = obj.GetComponent<Renderer>().material = new Material(this.sequenceMat);
            mat.EnableKeyword("_Color");
            mat.SetColor(ColorID, this.currentSequence[0]);
        }

        #endregion
    }
}