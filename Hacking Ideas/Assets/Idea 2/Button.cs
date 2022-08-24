using System.Collections.Generic;
using System.Linq;

namespace Idea_2
{
    public class Button : VrGrabObject
    {
        protected override void OnGrab()
        {
            transform.parent = this.originParent;

            List<GridKey> keys = this.originParent.GetComponentsInChildren<GridKey>().ToList();

            bool ready = keys.Any(k => !k.ready);

            foreach (GridKey gridKey in keys)
            {
                if (gridKey is EndGoal)
                    continue;

                if (ready)
                    gridKey.TriggerFirst();
                else
                    gridKey.Reset();
            }
        }

        protected override void OnRelease()
        {
            transform.parent = this.originParent;
        }
    }
}