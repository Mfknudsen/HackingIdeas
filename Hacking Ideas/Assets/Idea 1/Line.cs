using System.Collections.Generic;
using UnityEngine;

namespace Idea_1
{
    public enum TransferTo { FRONT, MIDDLE, BACK }

    public class Line : MonoBehaviour
    {
        #region Values

        public HackLinesSetup setup;
        [HideInInspector] public Vector3 front, back, middle, fNormal, mNormal, bNormal;
        public GameObject frontGate, middleGate, backGate;
        public Color color;

        public List<Vector3> totalPoints = new List<Vector3>(), splitTotalPoints = new List<Vector3>();
        private Path bezierPath, splitPath;
        public Vector3[] calculatedPath, splitCalculatedPath;
        public bool onSplitedPath = false;

        [Space]
        public Line frontTransfer;
        public int frontTransferTo;
        public Line middleTransfer;
        public int middleTransferTo;
        public Line backTransfer;
        public int backTransferTo;

        private LineRenderer[] lineRenderes;

        public int index, splitIndex, crossIndex;

        //V1
        bool disableCrossDirectionUpdate;

        //V2
        public Vector3 playerDir = Vector3.zero;
        public int indexDir;
        public bool readyNextUpdate;

#if UNITY_EDITOR
        public bool DisplayGizmo = true;
        public Color frontColor = Color.green, backColor = Color.blue, middleColor = Color.yellow;
#endif

        #endregion

        #region MonoBehaviour

        private void Start()
        {
            this.lineRenderes = GetComponentsInChildren<LineRenderer>();

            for (int i = 0; i < lineRenderes.Length; i++)
            {
                this.lineRenderes[i].startColor = this.color;
                this.lineRenderes[i].endColor = this.color;
            }

            if (this.frontGate != null && this.frontTransfer != null)
                this.frontGate.GetComponentInChildren<Light>().color = this.frontTransfer.color;
            if (this.backGate != null && this.backTransfer != null)
                this.backGate.GetComponentInChildren<Light>().color = this.backTransfer.color;
            if (this.middleGate != null && this.middleTransfer != null)
                this.middleGate.GetComponentInChildren<Light>().color = this.middleTransfer.color;

            index = Mathf.FloorToInt(totalPoints.Count / 2f);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!this.DisplayGizmo) return;

            if (frontTransfer != null)
            {
                Gizmos.color = this.frontColor;
                Vector3 dir = (this.frontTransferTo == 0 ? this.frontTransfer.front :
                    this.frontTransferTo == 1 ? this.frontTransfer.middle :
                    this.frontTransfer.back) - this.front;
                Gizmos.DrawLine(this.front, this.front + dir / 2);
                Gizmos.DrawWireSphere(this.front, .2f);

                Gizmos.color = Color.white;
                Gizmos.DrawLine(this.front + dir / 2, this.front + dir);
            }

            if (middleTransfer != null)
            {
                Gizmos.color = this.middleColor;
                Vector3 dir = (this.middleTransferTo == 0 ? this.middleTransfer.front :
                    this.middleTransferTo == 1 ? this.middleTransfer.middle :
                    this.middleTransfer.back) - this.middle;
                Gizmos.DrawLine(this.middle, this.middle + dir / 2);
                Gizmos.DrawWireSphere(this.middle, .2f);

                Gizmos.color = Color.white;
                Gizmos.DrawLine(this.middle + dir / 2, this.middle + dir);
            }

            if (backTransfer != null)
            {
                Gizmos.color = this.backColor;
                Vector3 dir = (this.backTransferTo == 0 ? this.backTransfer.front :
                    this.backTransferTo == 1 ? this.backTransfer.middle :
                    this.backTransfer.back) - this.back;
                Gizmos.DrawLine(this.back, this.back + dir / 2);
                Gizmos.DrawWireSphere(this.back, .2f);

                Gizmos.color = Color.white;
                Gizmos.DrawLine(this.back + dir / 2, this.back + dir);
            }
        }
#endif

        #endregion

        #region Setter

        public void SetFrontPosition(Vector3 set)
        {
            this.front = set;
        }

        public void SetBackPosition(Vector3 set)
        {
            this.back = set;
        }
        #endregion

        #region In

        public virtual void UpdateDist(Key key, float dir)
        {
            switch (this.setup.playerControls)
            {
                case Controls.V1:
                    V1Controls(key, dir);
                    break;

                case Controls.V2:
                    V2Controls(key, dir);
                    break;
            };
        }

        public void SetupExtraPoints(Transform refPoint, float dist)
        {
            if (this is EndLine)
                return;

            float distFromGate = 2.5f;

            this.totalPoints.Clear();
            this.totalPoints.Add(this.back);
            this.totalPoints.Add(this.back + this.bNormal * distFromGate);

            int totalPointCount = 4;

            Vector3 between = this.front + this.fNormal * distFromGate - this.totalPoints[1];

            for (int i = 0; i < totalPointCount - 1; i++)
            {
                Vector3 newPos = totalPoints[1] +
                                 between / totalPointCount * (i + 1) +
                                 Vector3.Cross((this.fNormal + this.bNormal) / 2, between.normalized) * Random.Range(0f, 1f) +
                                 Vector3.Lerp(this.fNormal, this.bNormal, Random.Range(0f, .25f));

                this.totalPoints.Add(newPos);
            }

            this.totalPoints.Add(this.front + this.fNormal * distFromGate);
            this.totalPoints.Add(this.front);

            this.bezierPath = GetComponentsInChildren<PathCreator>()[0].path;

            this.bezierPath.MovePoint(0, this.totalPoints[0]);

            //Move points includes control points between position points.
            //- Between two position points there will be two controll points
            this.bezierPath.MovePoint(3, this.totalPoints[1]);

            for (int i = 2; i < this.totalPoints.Count; i++)
                this.bezierPath.AddSegment(this.totalPoints[i]);

            this.calculatedPath = this.bezierPath.CalculateEvenlySpacedPoints(dist, 1);

            this.lineRenderes = GetComponentsInChildren<LineRenderer>();

            this.lineRenderes[0].positionCount = this.calculatedPath.Length;

            this.lineRenderes[0].SetPositions(this.calculatedPath);

            //Split Path
            this.splitTotalPoints = new List<Vector3>();
            int splitIndex = Mathf.FloorToInt(totalPointCount / 2) + 2;
            this.splitTotalPoints.Add(totalPoints[splitIndex]);

            refPoint.position += Vector3.up * (this.totalPoints[splitIndex].y - refPoint.position.y);
            this.splitTotalPoints.Add(this.splitTotalPoints[0] + Vector3.Cross(this.totalPoints[splitIndex + 1] - this.totalPoints[splitIndex], refPoint.position - this.totalPoints[splitIndex]).normalized);

            this.splitTotalPoints.Add(this.splitTotalPoints[1] + (this.middle + this.mNormal * distFromGate - this.splitTotalPoints[1]) / 2);

            this.splitPath = GetComponentsInChildren<PathCreator>()[1].path;

            this.splitPath.MovePoint(0, this.splitTotalPoints[0]);
            this.splitPath.MovePoint(3, this.splitTotalPoints[1]);

            for (int i = 2; i < this.splitTotalPoints.Count; i++)
                this.splitPath.AddSegment(this.splitTotalPoints[i]);

            this.splitPath.AddSegment(this.middle + this.mNormal * distFromGate);
            this.splitPath.AddSegment(this.middle);

            this.splitCalculatedPath = this.splitPath.CalculateEvenlySpacedPoints(dist, 1);

            this.lineRenderes[1].positionCount = this.splitCalculatedPath.Length;
            this.lineRenderes[1].SetPositions(this.splitCalculatedPath);

            for (int i = 0; i < this.calculatedPath.Length; i++)
            {
                if (Vector3.Distance(this.calculatedPath[i], this.splitCalculatedPath[0]) >= Vector3.Distance(this.calculatedPath[this.crossIndex], this.splitCalculatedPath[0]))
                    continue;

                this.crossIndex = i;
            }
        }

        private void V1Controls(Key key, float dir)
        {
            Vector3 keyPos = key.transform.position;

            if (this.index == 0 || this.index == this.calculatedPath.Length - 1 || this.splitIndex == this.splitCalculatedPath.Length - 1)
            {
                key.transform.position = Vector3.Lerp(keyPos, this.calculatedPath[this.crossIndex], Time.deltaTime);

                Color c;

                float fDist = Vector3.Distance(keyPos, this.front), mDist = Vector3.Distance(keyPos, this.middle), bDist = Vector3.Distance(keyPos, this.back);

                if (fDist < bDist && fDist < mDist)
                {
                    key.SetCurrentLine(this.frontTransfer, TransferTo.FRONT, dir);
                    c = this.frontTransfer.color;
                }
                else if (mDist < bDist)
                {
                    key.SetCurrentLine(this.middleTransfer, TransferTo.MIDDLE, dir);
                    c = this.middleTransfer.color;
                }
                else
                {
                    key.SetCurrentLine(this.backTransfer, TransferTo.BACK, dir);
                    c = this.backTransfer.color;
                }

                this.setup.TryAddToCurrentSequence(key, c);

                return;
            }

            if (this.index == this.crossIndex && this.splitIndex == 0)
            {
                float curDist = Vector3.Distance(keyPos, this.splitCalculatedPath[0]),
                    backDist = Vector3.Distance(keyPos, this.calculatedPath[this.index - 1]),
                    frontDist = Vector3.Distance(keyPos, this.calculatedPath[this.index + 1]),
                    middleDist = Vector3.Distance(keyPos, this.splitCalculatedPath[1]);

                if (this.disableCrossDirectionUpdate)
                {

                    if (frontDist < curDist && frontDist < middleDist && frontDist < backDist)
                    {
                        this.index++;
                        this.onSplitedPath = false;
                    }
                    else if (middleDist < curDist && middleDist < frontDist && middleDist < backDist)
                    {
                        this.splitIndex++;
                        this.onSplitedPath = true;
                    }
                    else if (backDist < curDist && backDist < frontDist && backDist < middleDist)
                    {
                        this.index--;
                        this.onSplitedPath = false;
                    }

                    return;
                }

                this.disableCrossDirectionUpdate = true;

                Vector3 playerDir = this.setup.playerArrow.forward,
                    frontDir = (this.calculatedPath[this.index + 1] - keyPos).normalized,
                    middleDir = (this.splitCalculatedPath[1] - keyPos).normalized,
                    backDir = (this.calculatedPath[this.index - 1] - keyPos).normalized;

                bool movingHigher = (key.facingHigherIndexPoint ? 1 : -1) * dir > 0;

                List<Vector3> toConsider = new List<Vector3>() { frontDir, middleDir, backDir };
                List<Vector3> points = new List<Vector3>() { this.front, this.middle, this.back };

                if (frontDist < middleDist && frontDist < backDist)
                {
                    toConsider.RemoveAt(0);
                    points.RemoveAt(0);
                }
                else if (middleDist < backDist)
                {
                    toConsider.RemoveAt(1);
                    points.RemoveAt(1);
                }
                else
                {
                    toConsider.RemoveAt(2);
                    points.RemoveAt(2);
                }

                int selectedIndex = Vector3.Angle(playerDir, toConsider[0]) < Vector3.Angle(playerDir, toConsider[1]) ? 0 : 1;

                if (points[selectedIndex] == front)
                {
                    if (points[Mathf.Abs(selectedIndex - 1)] == this.back)
                        key.facingHigherIndexPoint = !key.facingHigherIndexPoint;

                    key.transform.LookAt(this.calculatedPath[this.index + 1]);
                    key.transform.LookAt(key.transform.position + key.transform.forward * (key.facingHigherIndexPoint ? 1 : -1));
                }
                else if (points[selectedIndex] == this.middle)
                {
                    if (points[Mathf.Abs(selectedIndex - 1)] == this.back)
                        key.facingHigherIndexPoint = !key.facingHigherIndexPoint;

                    key.transform.LookAt(this.splitCalculatedPath[this.splitIndex + 1]);
                    key.transform.LookAt(key.transform.position + key.transform.forward * (key.facingHigherIndexPoint ? 1 : -1));
                }
                else
                {
                    key.transform.LookAt(this.calculatedPath[this.index - 1]);
                    key.transform.LookAt(key.transform.position + key.transform.forward * (key.facingHigherIndexPoint ? -1 : 1));
                }
            }
            else
            {
                this.disableCrossDirectionUpdate = false;

                int currentIndex = this.onSplitedPath ? this.splitIndex : this.index;
                Vector3[] list = this.onSplitedPath ? this.splitCalculatedPath : this.calculatedPath;

                Vector3 curPoint = list[currentIndex],
                    lowerPoint = list[currentIndex - 1],
                    higherPoint = list[currentIndex + 1];
                float lowDist = Vector3.Distance(lowerPoint, keyPos),
                    highDist = Vector3.Distance(higherPoint, keyPos),
                    curDist = Vector3.Distance(curPoint, keyPos);

                if (lowDist < highDist)
                {
                    key.transform.LookAt(lowerPoint);
                    key.transform.LookAt(key.transform.position + key.transform.forward * (key.facingHigherIndexPoint ? -1 : 1));
                }
                else
                {
                    key.transform.LookAt(higherPoint);
                    key.transform.LookAt(key.transform.position + key.transform.forward * (key.facingHigherIndexPoint ? 1 : -1));
                }

                if (lowDist < curDist && lowDist < highDist)
                {
                    if (this.onSplitedPath)
                        this.splitIndex--;
                    else
                        this.index--;
                }
                else if (highDist < curDist && highDist < lowDist)
                {
                    if (this.onSplitedPath)
                        this.splitIndex++;
                    else
                        this.index++;
                }
            }
        }

        private void V2Controls(Key key, float dir)
        {
            Vector3 keyPos = key.transform.position;

            if (this.index == 0 || this.index == this.calculatedPath.Length - 1 || this.splitIndex == this.splitCalculatedPath.Length - 1)
            {
                Color c;
                float fDist = Vector3.Distance(keyPos, this.front), mDist = Vector3.Distance(keyPos, this.middle), bDist = Vector3.Distance(keyPos, this.back);

                if (fDist < mDist && fDist < bDist)
                {
                    key.SetCurrentLine(this.frontTransfer, TransferTo.FRONT, dir);
                    c = this.frontTransfer.color;
                }
                else if (mDist < bDist)
                {
                    key.SetCurrentLine(this.middleTransfer, TransferTo.MIDDLE, dir);
                    c = this.middleTransfer.color;
                }
                else
                {
                    key.SetCurrentLine(this.backTransfer, TransferTo.BACK, dir);
                    c = this.backTransfer.color;
                }

                this.setup.TryAddToCurrentSequence(key, c);
            }
            else
            {
                if (this.index == this.crossIndex && this.splitIndex == 0)
                {
                    if (this.indexDir > 0)
                    {
                        this.onSplitedPath = Vector3.Angle(this.playerDir, this.splitCalculatedPath[1] - keyPos) < Vector3.Angle(this.playerDir, this.calculatedPath[this.index + 1] - keyPos);

                        if (this.onSplitedPath)
                        {
                            key.transform.LookAt(this.splitCalculatedPath[1]);

                            if (Vector3.Distance(keyPos, this.splitCalculatedPath[1]) < Vector3.Distance(keyPos, this.splitCalculatedPath[0]))
                                this.splitIndex++;
                        }
                        else
                        {
                            key.transform.LookAt(this.calculatedPath[this.index + 1]);
                            if (Vector3.Distance(keyPos, this.calculatedPath[this.index + 1]) < Vector3.Distance(keyPos, this.calculatedPath[this.index]))
                                this.index++;
                        }

                        key.transform.position += key.transform.forward * key.speed * Time.deltaTime;
                    }
                    else
                    {
                        this.onSplitedPath = false;
                        key.transform.LookAt(this.calculatedPath[this.index - 1]);
                        key.transform.position += key.transform.forward * key.speed * Time.deltaTime;

                        if (Vector3.Distance(keyPos, this.calculatedPath[this.index - 1]) < Vector3.Distance(keyPos, this.calculatedPath[this.index]))
                            this.index--;
                    }
                }
                else
                {
                    int selectedIndex = this.onSplitedPath ? this.splitIndex : this.index;
                    Vector3[] selectedList = this.onSplitedPath ? this.splitCalculatedPath : this.calculatedPath;

                    if (this.indexDir != 0)
                    {
                        Vector3 curPoint = selectedList[selectedIndex], nextPoint = selectedList[selectedIndex + this.indexDir];

                        key.transform.LookAt(selectedList[selectedIndex + this.indexDir]);
                        key.transform.position += key.transform.forward * key.speed * Time.deltaTime;

                        if (Vector3.Distance(keyPos, nextPoint) < Vector3.Distance(keyPos, curPoint))
                        {
                            if (this.onSplitedPath)
                                this.splitIndex += this.indexDir;
                            else
                                this.index += this.indexDir;
                        }
                    }

                    int dirNorm = dir > 0.25f ? 1 : 0;

                    if (dirNorm == 0 && !this.readyNextUpdate)
                    {
                        this.readyNextUpdate = true;
                        this.indexDir = 0;
                    }
                    else if (dirNorm != 0 && this.readyNextUpdate)
                    {
                        this.readyNextUpdate = false;

                        this.playerDir = this.setup.playerArrow.forward;

                        Vector3 lowerPoint = selectedList[selectedIndex - 1], higherPoint = selectedList[selectedIndex + 1];

                        this.indexDir = Vector3.Angle(this.playerDir, lowerPoint - keyPos) < Vector3.Angle(this.playerDir, higherPoint - keyPos) ? -1 : 1;
                    }
                }
            }
        }

        #endregion
    }
}