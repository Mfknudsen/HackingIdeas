using UnityEngine;

namespace Idea_1
{
    public class Key : MonoBehaviour
    {
        #region Values

        /// <summary>
        /// CurrentLine must be set before start.
        /// The ball will start on this line.
        /// Is set during line auto setup.
        /// </summary>
        [HideInInspector] public Line currentLine;

        /// <summary>
        /// The keys travel speed.
        /// </summary>
        public float speed = 2f;

        /// <summary>
        /// Reference to the minigame setup.
        /// </summary>
        public HackLinesSetup setup;

        /// <summary>
        /// If the ball is to travel forward or backwards
        /// </summary>
        private Vector2 inputDir;

        /// <summary>
        /// Getting input from the player to decide when and how it should move.
        /// </summary>
        private PlayerInput playerInput;

        /// <summary>
        /// Used when traveling between points on a line.
        /// Will also be used when switching lines to decide if the key should face out from the gate or the other way.
        /// </summary>
        [HideInInspector] public bool facingHigherIndexPoint;

        #endregion

        #region MonoBehaviour

        /// <summary>
        /// Resets it's position and rotation.
        /// </summary>
        private void Start() =>
            ResetStartPosition();

        /// <summary>
        /// Get the input and move the key.
        /// </summary>
        private void Update()
        {
            this.inputDir = new Vector2(
                this.playerInput.Player.GrabRight.ReadValue<float>(),
                this.playerInput.Player.GrabLeft.ReadValue<float>());

            float dir = this.inputDir.x - this.inputDir.y;

            dir = Mathf.Clamp(dir, -1, 1);

            if (this.setup.playerControls == Controls.V1)
            {
                Transform keyTransform = transform;
                keyTransform.position += keyTransform.forward * (dir * this.speed * Time.deltaTime);
            }

            //Update the keys position using the current line it's placed on.
            this.currentLine.UpdateDist(this, dir);
        }

        #endregion

        #region In

        /// <summary>
        /// Set the current line when switching between it's current and a new line.
        /// If the new line is the end line then it will be disable.
        /// </summary>
        /// <param name="set">The new line to set</param>
        /// <param name="transferTo">Which gate to come out of</param>
        /// <param name="dir">What direction the key will face</param>
        public void SetCurrentLine(Line set, TransferTo transferTo, float dir)
        {
            //Check if the new line is the end.
            if (set is EndLine)
            {
                transform.position = set.front;
                enabled = false;
                return;
            }

            set.indexDir = 0;
            set.readyNextUpdate = this.currentLine.readyNextUpdate;

            //Transfer the key to one of the gates on the new line.
            if (transferTo != TransferTo.Middle)
            {
                set.index = transferTo == TransferTo.Back ? 1 : set.calculatedPath.Length - 2;
                set.splitIndex = 0;
                set.onSplitPath = false;

                transform.position = set.calculatedPath[set.index];

                this.currentLine = set;

                this.facingHigherIndexPoint = ((set.index == 1 && dir > 0) ||
                                               (set.index == set.calculatedPath.Length - 2 && dir < 0));
            }
            else
            {
                set.splitIndex = set.splitCalculatedPath.Length - 2;
                set.index = set.crossIndex;
                set.onSplitPath = true;

                transform.position = set.splitCalculatedPath[set.splitIndex];

                this.facingHigherIndexPoint = dir < 0;

                this.currentLine = set;
            }

            //Extra step for how one of the control versions act when switching line.
            if (this.setup.playerControls == Controls.V2)
            {
                if (transferTo != TransferTo.Back && set.indexDir == 1)
                    set.indexDir = -1;
                else if (transferTo == TransferTo.Back && set.indexDir == -1)
                    set.indexDir = 1;
            }
        }

        /// <summary>
        /// Resets the keys position and current line to the root line.
        /// Same as when the minigame first starts.
        /// </summary>
        public void ResetStartPosition()
        {
            this.currentLine = transform.parent.GetComponent<HackLinesSetup>().GetRootLine();

            int i = Mathf.FloorToInt(this.currentLine.calculatedPath.Length / 2f);
            transform.position = this.currentLine.calculatedPath[i];
            transform.LookAt(this.currentLine.calculatedPath[i + 1]);

            this.currentLine.index = i;

            this.playerInput = new PlayerInput();
            this.playerInput.Enable();

            this.facingHigherIndexPoint = true;

            this.currentLine.onSplitPath = false;
            this.currentLine.splitIndex = 0;
        }

        #endregion
    }
}