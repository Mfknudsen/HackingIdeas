namespace Idea_2
{
    public class Button : VrGrabObject
    {
        public GridKey key;

        protected override void OnGrab()
        {
            this.key.TriggerFirst();

            transform.parent = this.originParent;
        }

        protected override void OnRelease()
        {
            transform.parent = this.originParent;
        }
    }
}