using Idea_2;
using TMPro;
using UnityEngine;

namespace Idea_6
{
    public class AnswerButton : Button
    {
        public Answer answer { get; private set; }

        [SerializeField] private ChooseHackSetup setup;
        [SerializeField] private TextMeshPro textMeshPro;

        protected override void Start()
        {
            base.Start();

            this.textMeshPro.text = "";
        }

        protected override void OnGrab()
        {
            transform.parent = this.originParent;

            this.setup.AnswerQuestion(this.answer);
        }

        public void SetAnswer(Answer answer)
        {
            this.answer = answer;

            this.textMeshPro.text = answer != null ? answer.GetAnswer() : "";
        }
    }
}