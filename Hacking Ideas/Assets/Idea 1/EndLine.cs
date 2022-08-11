namespace Idea_1
{
    public class EndLine : Line
    {
        #region MonoBehaviour

        private void Start()
        {
            this.front = transform.position;
            this.back = front;
        }

        #endregion

        #region In
        public override void UpdateDist(Key ball, float dir)
        {
            //Endlines only function is to keep it within reach of the player to be grabed.
            ball.enabled = false;
        }
        #endregion
    }
}
