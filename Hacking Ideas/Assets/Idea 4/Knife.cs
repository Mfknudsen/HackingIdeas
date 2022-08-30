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
            if (!this.touchingBrain && this.currentLine != null)
            {
                this.currentLine.Deselect();
                this.currentLine = null;
            }

            if (!this.active || !this.touchingBrain) return;

            if (this.currentLine == null)
            {
                Vector3 point = cutPoint.position;
                float dist = 10;
                foreach (CutLine c in this.setup.brain.cutLines)
                {
                    if (c.done) continue;

                    float d = Vector3.Distance(point, c.startPoint);

                    if (d < this.setup.brain.maxDistanceFromLine && d < dist)
                    {
                        dist = d;
                        this.currentLine = c;
                    }
                }

                this.currentLine?.Selected();
            }
            else
                this.currentLine.Update(this);
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