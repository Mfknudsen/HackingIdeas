using System;
using System.Collections.Generic;
using System.Linq;
using Idea_2.Blockers;
using UnityEngine;

namespace Idea_2
{
    public enum BlockerType
    {
        None,
        Total,
        Up,
        Down,
        Right,
        Left,
        DeniedUp,
        DeniedDown,
        DeniedLeft,
        DeniedRight,
        SwitchUp,
        SwitchDown,
        SwitchLeft,
        SwitchRight
    }

    public class InputBoard : MonoBehaviour
    {
        public GridHackSetup setup;
        [SerializeField] private float placeDist, sizeScale = 1;
        [SerializeField, HideInInspector] private Vector2Int gridSize;
        [SerializeField] private GridKey endPoint;

        [SerializeField, HideInInspector] private List<Storage<InputBlock>> gridBlocks;
        [SerializeField, HideInInspector] public List<Storage<Transform>> gridTransforms;
        [SerializeField, HideInInspector] public List<Storage<BlockerType>> blockerTypes;

        private int keysInGoal;

        private void OnValidate()
        {
            gridSize = new Vector2Int(
                Mathf.Clamp(gridSize.x, 2, 100),
                Mathf.Clamp(gridSize.y, 2, 100)
            );
        }

        public void Setup(Vector2Int size, GameObject tilePrefab)
        {
            this.gridSize = size;
            List<GameObject> toDelete =
                (from Transform d in transform select d.gameObject)
                .ToList();
            foreach (GameObject o in toDelete)
                DestroyImmediate(o);

            this.blockerTypes = new List<Storage<BlockerType>>();
            this.gridBlocks = new List<Storage<InputBlock>>();
            this.gridTransforms = new List<Storage<Transform>>();
            for (int x = 0; x < size.x; x++)
            {
                this.blockerTypes.Add(new Storage<BlockerType>(size.y));
                this.gridBlocks.Add(new Storage<InputBlock>(size.y));
                this.gridTransforms.Add(new Storage<Transform>(size.y));
            }

            Transform t = transform;
            Vector3 startPos = t.position + t.up * .55f;

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    GameObject obj = Instantiate(tilePrefab, transform);
                    obj.name = "Square + " + x + "-" + y;

                    Transform oTransform = obj.transform;
                    Vector3 tempScale = oTransform.localScale * sizeScale;
                    float localSize = tempScale.x * 10;
                    oTransform.localScale = tempScale;

                    gridTransforms[x].Set(y, oTransform);

                    Vector3 objPosition = startPos;
                    Vector3 forward = t.forward, right = t.right;

                    objPosition -=
                        right * (x * localSize) + forward * (y * localSize);
                    objPosition += right * (size.x / 2f * localSize) +
                                   forward * (size.y / 2f * localSize);
                    oTransform.position = objPosition;
                    oTransform.LookAt(objPosition + forward);
                }
            }
        }

        public void RemoveFromBoard(InputBlock block)
        {
            foreach (Storage<InputBlock> t in this.gridBlocks)
            {
                for (int y = 0; y < this.gridBlocks[0].Count; y++)
                {
                    if (t[y] != block) continue;

                    t.Set(y, null);
                    break;
                }
            }
        }

        public void Trigger(GridKey key, Vector2Int id)
        {
            if (id == this.endPoint.id)
            {
                key.gameObject.SetActive(false);
                this.keysInGoal++;

                if (this.keysInGoal != this.setup.keyStartPositions.Count) return;

                key.gameObject.SetActive(true);
                key.transform.position = transform.parent.GetComponentInChildren<Button>().transform.position;

                return;
            }

            if (id.x < 0 || id.y < 0 ||
                id.x >= this.gridSize.x || id.y >= this.gridSize.y ||
                (this.gridBlocks[id.x][id.y] == null && this.blockerTypes[id.x][id.y] == BlockerType.None))
            {
                Reset();

                return;
            }

            if (this.blockerTypes[id.x][id.y] == BlockerType.Total)
            {
                Reset();

                return;
            }

            if (this.blockerTypes[id.x][id.y] != BlockerType.None)
            {
                key.currentCoroutine = StartCoroutine(this.gridTransforms[id.x][id.y].GetComponentInChildren<Blocker>()
                    .Trigger(
                        key,
                        this.setup.timePerBlock,
                        this.setup.visualGrid,
                        this));

                return;
            }

            key.currentCoroutine = StartCoroutine(
                this.gridBlocks[id.x][id.y].TriggerInput(
                    key,
                    this.setup.timePerBlock,
                    this.setup.visualGrid));
        }

        public bool TryPlaceBlock(InputBlock block)
        {
            Vector2Int closestID = -Vector2Int.one;
            float lowestDist = 0;

            for (int x = 0; x < this.gridTransforms.Count; x++)
            {
                for (int y = 0; y < this.gridTransforms[0].Count; y++)
                {
                    if (this.blockerTypes[x][y] != BlockerType.None)
                        continue;

                    Transform gridTransform = this.gridTransforms[x][y];

                    float dist = Vector3.Distance(gridTransform.position, block.transform.position);

                    if (dist >= this.placeDist)
                        continue;

                    if (closestID == -Vector2Int.one)
                    {
                        closestID = new Vector2Int(x, y);
                        lowestDist = dist;
                        continue;
                    }

                    if (dist >= lowestDist) continue;

                    lowestDist = dist;
                    closestID = new Vector2Int(x, y);
                }
            }
            
            if (closestID == -Vector2Int.one)
                return false;

            try
            {
                if (this.gridBlocks[closestID.x][closestID.y] != null)
                    return false;

                this.gridBlocks[closestID.x].Set(closestID.y, block);
            }
            catch
            {
                return false;
            }

            block.id = closestID;
            Transform tile = this.gridTransforms[closestID.x][closestID.y],
                blockTransform = block.transform;
            Vector3 blockForward = blockTransform.forward,
                f = tile.forward,
                r = tile.right;
            float aForward = Vector3.Angle(blockForward, f),
                aRight = Vector3.Angle(blockForward, r),
                aBackwards = Vector3.Angle(blockForward, -f),
                aLeft = Vector3.Angle(blockForward, -r);

            if (aForward < aRight && aForward < aBackwards && aForward < aLeft)
                block.placeDirection = PlaceDirection.MinusY;
            else if (aRight < aBackwards && aRight < aLeft)
                block.placeDirection = PlaceDirection.MinusX;
            else if (aBackwards < aLeft)
                block.placeDirection = PlaceDirection.PlusY;
            else
                block.placeDirection = PlaceDirection.PlusX;

            blockTransform.position = this.gridTransforms[closestID.x][closestID.y].position;

            Vector3 pos = this.gridTransforms[0][0].position;
            Vector3 forward = this.gridTransforms[0][1].position - pos,
                right = this.gridTransforms[1][0].position - pos;

            blockTransform.LookAt(
                blockTransform.position
                + forward * block.idDir.y
                + right * block.idDir.x,
                this.gridTransforms[0][0].up);

            this.gridBlocks[block.id.x].Set(block.id.y, block);

            return true;
        }

        private void Reset()
        {
            foreach (GridKey g in transform.parent.GetComponentsInChildren<GridKey>())
            {
                if (g is EndGoal)
                    continue;

                if (g.currentCoroutine != null)
                    StopCoroutine(g.currentCoroutine);
                g.Reset();
            }

            foreach (Blocker b in transform.parent.GetComponentsInChildren<Blocker>())
                b.Reset();

            foreach (InputBlock inputBlock in gridBlocks.SelectMany(b => b.list).Where(b => b != null))
                inputBlock.Reset();
        }
    }

    [Serializable]
    public struct Storage<T>
    {
        public List<T> list;

        public Storage(int size)
        {
            list = new List<T>();
            for (int i = 0; i < size; i++)
            {
                list.Add(default);
            }
        }

        public T this[int index] => list[index];

        public void Set(int index, T set)
        {
            list[index] = set;
        }

        public int Count => list.Count;
    }
}