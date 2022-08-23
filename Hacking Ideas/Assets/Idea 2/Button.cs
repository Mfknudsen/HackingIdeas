namespace Idea_2
{
    public class Button : VrGrabObject
    {
        protected override void OnGrab()
        {
            transform.parent = this.originParent;

            foreach (GridKey key in this.originParent.GetComponentsInChildren<GridKey>())
                key.TriggerFirst();
        }

        protected override void OnRelease()
        {
            transform.parent = this.originParent;
        }
    }
}