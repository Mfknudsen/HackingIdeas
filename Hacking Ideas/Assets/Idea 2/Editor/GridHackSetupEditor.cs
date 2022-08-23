using System.Collections.Generic;
using System.Linq;
using Idea_2.Blockers;
using UnityEditor;
using UnityEngine;

namespace Idea_2.Editor
{
    [CustomEditor(typeof(GridHackSetup))]
    public class GridHackSetupEditor : UnityEditor.Editor
    {
        private GridHackSetup setup;

        private GameObject totalBlocker, dirBlocker, deniedBlocker, switchBlocker;

        private void OnEnable()
        {
            this.setup = target as GridHackSetup;

            this.totalBlocker ??=
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Idea 2/Blockers/Blocker.prefab");
            this.dirBlocker ??=
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Idea 2/Blockers/DirectionalBlocker.prefab");
            this.deniedBlocker ??=
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Idea 2/Blockers/DeniedBlocker.prefab");
            this.switchBlocker ??=
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Idea 2/Blockers/SwitchBlocker.prefab");
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Setup new grid"))
                this.setup.Setup();
            
            Vector2Int size = this.setup.currentSize;

            EditorGUILayout.Separator();
            
            BlockerPositionAndTypes(size);

            EditorGUILayout.Separator();

            KeyStartPositions(size);

            EditorGUILayout.Separator();

            base.OnInspectorGUI();
        }

        private void BlockerPositionAndTypes(Vector2Int size)
        {
            EditorGUILayout.LabelField("Blockers:");
            InputBoard board = this.setup.inputBoard;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            Rect scale = GUILayoutUtility.GetLastRect();
            const float ySize = 20;

            if (board.blockerTypes.Count == 0 || board.blockerTypes[0].Count() == 0)
            {
                base.OnInspectorGUI();
                return;
            }

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

                    if (board.blockerTypes[x][y] != preType)
                        SetupBlocker(new Vector2Int(x, y), board.gridTransforms[x][y], board.blockerTypes[x][y]);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(ySize * size.y);
        }
        
        private void KeyStartPositions(Vector2Int size)
        {
            EditorGUILayout.LabelField("Key Start Positions:");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            for (int x = 0; x < size.x; x++)
            {
                bool value = KeyStartsAtIndex(new Vector2Int(x, size.y));

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

            EditorGUILayout.BeginVertical();
            for (int y = size.y - 1; y >= 0; y--)
            {
                bool value = KeyStartsAtIndex(new Vector2Int(-1, y));

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

            EditorGUILayout.BeginVertical();
            for (int y = size.y - 1; y >= 0; y--)
            {
                bool value = KeyStartsAtIndex(new Vector2Int(size.x, y));

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

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);

            for (int x = 0; x < size.x; x++)
            {
                bool value = KeyStartsAtIndex(new Vector2Int(x, -1));

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
                    this.setup.keyStartPositions.Add(new Vector2Int(x, size.y));
            }

            GUILayout.Space(20);
            EditorGUILayout.EndHorizontal();
        }

        private void SetupBlocker(Vector2Int id, Transform t, BlockerType type)
        {
            List<GameObject> toDestroy = (from Transform child in t
                let name = child.name
                where name.Contains("Blocker")
                select child.gameObject).ToList();

            foreach (GameObject g in toDestroy)
                DestroyImmediate(g);

            GameObject instants = null;
            PlaceDirection direction = PlaceDirection.MinusX;
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (type)
            {
                case BlockerType.Total:
                    instants = Instantiate(this.totalBlocker, t);
                    instants.name = "TotalBlocker";
                    instants.transform.LookAt(instants.transform.position + t.forward);
                    break;

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
            }

            if (instants == null)
                return;

            instants.transform.localPosition = Vector3.zero;
            instants.transform.localScale *= 5;
            instants.transform.LookAt(instants.transform.position + instants.transform.forward, t.up);

            if (!(instants.GetComponent<Blocker>() is { } b)) return;

            b.id = id;
            b.placeDirection = direction;
        }

        private bool KeyStartsAtIndex(Vector2Int index)
        {
            return setup.keyStartPositions.Any(pos => pos == index);
        }
    }
}