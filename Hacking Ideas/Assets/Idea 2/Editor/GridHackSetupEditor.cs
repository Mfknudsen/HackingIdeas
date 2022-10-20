using System.Collections.Generic;
using System.Linq;
using Idea_2.Blockers;
using UnityEditor;
using UnityEngine;

namespace Idea_2.Editor
{
    /// <summary>
    /// Editor script for adding inspector functionality. 
    /// </summary>
    [CustomEditor(typeof(GridHackSetup))]
    public class GridHackSetupEditor : UnityEditor.Editor
    {
        private GridHackSetup setup;

        private GameObject dirBlocker, deniedBlocker, switchBlocker;

        /// <summary>
        /// Set target and load the prefabs for the different types of blockers.
        /// </summary>
        private void OnEnable()
        {
            this.setup = this.target as GridHackSetup;

            this.dirBlocker ??=
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Idea 2/Blockers/DirectionalBlocker.prefab");
            this.deniedBlocker ??=
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Idea 2/Blockers/DeniedBlocker.prefab");
            this.switchBlocker ??=
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Idea 2/Blockers/SwitchBlocker.prefab");
        }

        /// <summary>
        /// Add the different buttons and setup features.
        /// Also show the default inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Setup new grid"))
                this.setup.Setup();

            Vector2Int size = this.setup.currentSize;

            EditorGUILayout.Separator();

            this.BlockerPositionAndTypes(size);

            EditorGUILayout.Separator();

            this.KeyStartPositions(size);

            EditorGUILayout.Separator();

            base.OnInspectorGUI();
        }

        /// <summary>
        /// With a grid setup show the different positions and what type of static blocker.
        /// Allows to place new blockers in the grid and change the current to a different type or remove it.
        /// </summary>
        /// <param name="size">The size of the current grid board</param>
        private void BlockerPositionAndTypes(Vector2Int size)
        {
            EditorGUILayout.LabelField("Blockers:");
            InputBoard board = this.setup.inputBoard;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            Rect scale = GUILayoutUtility.GetLastRect();
            const float ySize = 20;

            if (board.blockerTypes.Count == 0 || board.blockerTypes[0].Count == 0)
                return;

            //For each position show an enum dropdown menu.
            for (int y = 0; y < size.y; y++)
            {
                EditorGUILayout.BeginHorizontal();

                for (int x = 0; x < size.x; x++)
                {
                    BlockerType preType = board.blockerTypes[x][y];
                    board.blockerTypes[x].Set(y, (BlockerType)EditorGUI.EnumPopup(
                        new Rect(
                            scale.size.x / size.x * x + 15,
                            scale.position.y + ySize * Mathf.Abs(y - size.y + 1),
                            scale.size.x / size.x,
                            10),
                        "",
                        preType));

                    //If a new type was selected then set it up.
                    if (board.blockerTypes[x][y] != preType)
                        this.SetupBlocker(new Vector2Int(x, y), board.gridTransforms[x][y], board.blockerTypes[x][y]);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(ySize * size.y);
        }

        /// <summary>
        /// Show the possible start positions of the keys and allows the set a true/false if a key should spawn there.
        /// The possible start positions are outside of the input board.
        /// </summary>
        /// <param name="size">The size of the current grid board</param>
        private void KeyStartPositions(Vector2Int size)
        {
            EditorGUILayout.LabelField("Key Start Positions:");

            //Positions +1 on the y axis.
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            for (int x = 0; x < size.x; x++)
            {
                bool value = this.KeyStartsAtIndex(new Vector2Int(x, size.y));

                GUILayout.FlexibleSpace();
                bool result = GUILayout.Toggle(value, "");
                GUILayout.FlexibleSpace();

                if (value == result)
                    continue;

                if (!result)
                {
                    for (int i = 0; i < this.setup.keyStartPositions.Count; i++)
                    {
                        if (this.setup.keyStartPositions[i] != new Vector2Int(x, size.y)) continue;

                        this.setup.keyStartPositions.RemoveAt(i);
                        break;
                    }
                }
                else
                    this.setup.keyStartPositions.Add(new Vector2Int(x, size.y));
            }

            GUILayout.Space(20);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(); 
            //Positions -1 on the x axis.
            EditorGUILayout.BeginVertical();
            for (int y = size.y - 1; y >= 0; y--)
            {
                bool value = this.KeyStartsAtIndex(new Vector2Int(-1, y));

                bool result = GUILayout.Toggle(value, "");

                if (value == result)
                    continue;

                if (!result)
                {
                    for (int i = 0; i < this.setup.keyStartPositions.Count; i++)
                    {
                        if (this.setup.keyStartPositions[i] != new Vector2Int(-1, y)) continue;

                        this.setup.keyStartPositions.RemoveAt(i);
                        break;
                    }
                }
                else
                    this.setup.keyStartPositions.Add(new Vector2Int(-1, y));
            }

            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            //Positions +1 on the x axis.
            EditorGUILayout.BeginVertical();
            for (int y = size.y - 1; y >= 0; y--)
            {
                bool value = this.KeyStartsAtIndex(new Vector2Int(size.x, y));

                bool result = GUILayout.Toggle(value, "");

                if (value == result)
                    continue;

                if (!result)
                {
                    for (int i = 0; i < this.setup.keyStartPositions.Count; i++)
                    {
                        if (this.setup.keyStartPositions[i] != new Vector2Int(size.x, y)) continue;

                        this.setup.keyStartPositions.RemoveAt(i);
                        break;
                    }
                }
                else
                    this.setup.keyStartPositions.Add(new Vector2Int(size.x, y));
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            //Positions -1 on the y axis.
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);

            for (int x = 0; x < size.x; x++)
            {
                bool value = this.KeyStartsAtIndex(new Vector2Int(x, -1));

                GUILayout.FlexibleSpace();
                bool result = GUILayout.Toggle(value, "");
                GUILayout.FlexibleSpace();

                if (value == result)
                    continue;

                if (!result)
                {
                    for (int i = 0; i < this.setup.keyStartPositions.Count; i++)
                    {
                        if (this.setup.keyStartPositions[i] != new Vector2Int(x, -1)) continue;

                        this.setup.keyStartPositions.RemoveAt(i);
                        break;
                    }
                }
                else
                    this.setup.keyStartPositions.Add(new Vector2Int(x, -1));
            }

            GUILayout.Space(20);
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Spawn or remove a blocker on the grid board.
        /// </summary>
        /// <param name="id">ID position on the grid board</param>
        /// <param name="t">The transform to parent to</param>
        /// <param name="type">Type of the blocker</param>
        private void SetupBlocker(Vector2Int id, Transform t, BlockerType type)
        {
            List<GameObject> toDestroy = (from Transform child in t
                let name = child.name
                where name.Contains("Blocker")
                select child.gameObject).ToList();

            foreach (GameObject g in toDestroy)
                DestroyImmediate(g);

            GameObject instants;
            PlaceDirection direction;
            switch (type)
            {
                case BlockerType.Down:
                    instants = Instantiate(this.dirBlocker, t);
                    instants.name = "DirectionalBlocker";
                    instants.transform.LookAt(instants.transform.position + t.forward);
                    direction = PlaceDirection.MinusY;
                    break;

                case BlockerType.Up:
                    instants = Instantiate(this.dirBlocker, t);
                    instants.name = "DirectionalBlocker";
                    instants.transform.LookAt(instants.transform.position - t.forward);
                    direction = PlaceDirection.PlusY;
                    break;

                case BlockerType.Left:
                    instants = Instantiate(this.dirBlocker, t);
                    instants.name = "DirectionalBlocker";
                    instants.transform.LookAt(instants.transform.position + t.right, t.up);
                    direction = PlaceDirection.MinusX;
                    break;

                case BlockerType.Right:
                    instants = Instantiate(this.dirBlocker, t);
                    instants.name = "DirectionalBlocker";
                    instants.transform.LookAt(instants.transform.position - t.right, t.up);
                    direction = PlaceDirection.PlusX;
                    break;

                case BlockerType.DeniedDown:
                    instants = Instantiate(this.deniedBlocker, t);
                    instants.name = "DeniedBlocker";
                    instants.transform.LookAt(instants.transform.position + t.forward);
                    direction = PlaceDirection.MinusY;
                    break;

                case BlockerType.DeniedUp:
                    instants = Instantiate(this.deniedBlocker, t);
                    instants.name = "DeniedBlocker";
                    instants.transform.LookAt(instants.transform.position - t.forward);
                    direction = PlaceDirection.PlusY;
                    break;

                case BlockerType.DeniedLeft:
                    instants = Instantiate(this.deniedBlocker, t);
                    instants.name = "DeniedBlocker";
                    instants.transform.LookAt(instants.transform.position + t.right);
                    direction = PlaceDirection.MinusX;
                    break;

                case BlockerType.DeniedRight:
                    instants = Instantiate(this.deniedBlocker, t);
                    instants.name = "DeniedBlocker";
                    instants.transform.LookAt(instants.transform.position - t.right);
                    direction = PlaceDirection.PlusX;
                    break;

                case BlockerType.SwitchUp:
                    instants = Instantiate(this.switchBlocker, t);
                    instants.name = "SwitchBlocker";
                    instants.transform.LookAt(instants.transform.position - t.forward);
                    direction = PlaceDirection.PlusY;
                    break;

                case BlockerType.SwitchDown:
                    instants = Instantiate(this.switchBlocker, t);
                    instants.name = "SwitchBlocker";
                    instants.transform.LookAt(instants.transform.position + t.forward);
                    direction = PlaceDirection.MinusY;
                    break;

                case BlockerType.SwitchLeft:
                    instants = Instantiate(this.switchBlocker, t);
                    instants.name = "SwitchBlocker";
                    instants.transform.LookAt(instants.transform.position + t.right);
                    direction = PlaceDirection.MinusX;
                    break;

                case BlockerType.SwitchRight:
                    instants = Instantiate(this.switchBlocker, t);
                    instants.name = "SwitchBlocker";
                    instants.transform.LookAt(instants.transform.position - t.right);
                    direction = PlaceDirection.PlusX;
                    break;
                case BlockerType.None:
                default:
                    return;
            }

            if (instants == null)
                return;

            instants.transform.localPosition = Vector3.zero;
            instants.transform.localScale *= 5;
            instants.transform.LookAt(instants.transform.position + instants.transform.forward, t.up);

            if (!(instants.GetComponent<Blocker>() is { } b)) return;

            b.inputBoard = this.setup.inputBoard;
            b.id = id;
            b.placeDirection = direction;
        }

        /// <summary>
        /// If there is any key at the index.
        /// </summary>
        /// <param name="index">Input board position id</param>
        /// <returns>True if any key has the id</returns>
        private bool KeyStartsAtIndex(Vector2Int index) => 
            this.setup.keyStartPositions.Any(pos => pos == index);
    }
}