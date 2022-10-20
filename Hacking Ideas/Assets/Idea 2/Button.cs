using System.Collections.Generic;
using System.Linq;

namespace Idea_2
{
    /// <summary>
    /// A "hack" for creating a quick working button.
    /// The button is a grabbable item that upon being grabbed will instantly release itself by un-parenting.
    /// </summary>
    public class Button : VrGrabObject
    {
        /// <summary>
        /// When grabbed, release by un-parenting and trigger the keys or reset them.
        /// </summary>
        protected override void OnGrab()
        {
            this.transform.parent = this.originParent;

            List<GridKey> keys = this.originParent.GetComponentsInChildren<GridKey>().ToList();

            bool ready = keys.Any(k => !k.ready);

            foreach (GridKey gridKey in keys.Where(gridKey => !(gridKey is EndGoal)))
            {
                if (ready)
                    gridKey.TriggerFirst();
                else
                    gridKey.Reset();
            }
        }

        protected override void OnRelease() => 
            this.transform.parent = this.originParent;
    }
}