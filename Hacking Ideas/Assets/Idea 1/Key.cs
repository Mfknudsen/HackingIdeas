using UnityEngine;

namespace Idea_1
{
    public class Key : MonoBehaviour
    {
        #region Values

        //currentLine must be set before start
        //- The ball will start on this line
        //- Is set during line auto setup
        [HideInInspector] public Line currentLine;
        public float speed = 2f;
        public HackLinesSetup setup;

        //If the ball is to travel forward or backwards
        private Vector2 inputDir;
        private PlayerInput playerInput;
        [HideInInspector] public bool facingHigherIndexPoint;

        #endregion

        #region MonoBehaviour

        private void Start()
        {
            ResetStartPosition();
        }

        private void Update()
        {
            //Get input
            // --
            //- Later change from keyboard input to something like a lever input for VR controlls.
            // --
            this.inputDir = new Vector2(
                this.playerInput.Player.GrabRight.ReadValue<float>(),
                this.playerInput.Player.GrabLeft.ReadValue<float>());

            float dir = this.inputDir.x - this.inputDir.y;

            dir = Mathf.Clamp(dir, -1, 1);

            if (this.setup.playerControls == Controls.V1)
                transform.position += transform.forward * (dir * this.speed * Time.deltaTime);

            this.currentLine.UpdateDist(this, dir);
        }

        #endregion

        #region In

        public void SetCurrentLine(Line set, TransferTo transferTo, float dir)
        {
            if (set is EndLine)
            {
                transform.position = set.front;
                this.enabled = false;
                return;
            }

            set.indexDir = 0;
            set.readyNextUpdate = currentLine.readyNextUpdate;

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

            if (this.setup.playerControls == Controls.V2)
            {
                if (transferTo != TransferTo.Back && set.indexDir == 1)
                    set.indexDir = -1;
                else if (transferTo == TransferTo.Back && set.indexDir == -1)
                    set.indexDir = 1;
            }
        }

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

            currentLine.onSplitPath = false;
            currentLine.splitIndex = 0;
        }

        #endregion
    }
}
