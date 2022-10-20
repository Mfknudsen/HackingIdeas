using UnityEngine;

namespace Idea_4
{
    public class BrainSurgeryHackSetup : MonoBehaviour
    {
        public Knife knife;
        public Brain brain;
        public int lineCount;
        public float damagePerSecond = 1;

        private void Start()
        {
            this.knife ??= this.GetComponentInChildren<Knife>();
            this.brain ??= this.GetComponentInChildren<Brain>();
            this.brain.Setup(this.lineCount);
        }
    }
}