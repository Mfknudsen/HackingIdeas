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
            knife ??= GetComponentInChildren<Knife>();
            brain ??= GetComponentInChildren<Brain>();
            brain.Setup(lineCount);
        }
    }
}