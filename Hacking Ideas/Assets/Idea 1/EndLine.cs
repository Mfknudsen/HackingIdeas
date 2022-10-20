namespace Idea_1
{
    /// <summary>
    /// Used to different between normal lines and the final end line that
    /// the key will not be able to move from. 
    /// </summary>
    public sealed class EndLine : Line
    {
        #region MonoBehaviour

        private void Start()
        {
            this.front = this.transform.position;
            this.back = this.front;
        }

        #endregion

        #region In

        public override void UpdateDist(Key ball, float dir)
        {
            //Endlines only function is to keep it within reach of the player to be grabbed.
            ball.enabled = false;
        }

        #endregion
    }
}