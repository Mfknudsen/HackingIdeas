using UnityEngine;

namespace Idea_4
{
    public class Knife : VrGrabObject
    {
        public BrainSurgeryHackSetup setup;
        private bool active;
        [HideInInspector] public bool touchingBrain;
        public Transform cutPoint;

        private CutLine currentLine;

        private void Update()
        {
            if (!touchingBrain && currentLine != null)
            {
                currentLine.Deselect();
                currentLine = null;
            }

            if (!active || !touchingBrain) return;

            if (currentLine == null)
            {
                Vector3 point = transform.position;
                float dist = 10;
                foreach (CutLine c in this.setup.brain.cutLines)
                {
                    if (c.done) continue;

                    float d = Vector3.Distance(point, c.startPoint);

                    if (d > this.setup.brain.maxDistanceFromLine && dist < d) continue;

                    dist = d;
                    currentLine = c;
                }

                currentLine?.Selected();
            }
            else
                currentLine.Update(this);
        }

        protected override void OnGrab()
        {
            active = true;
        }

        protected override void OnRelease()
        {
            active = false;
        }
    }
}