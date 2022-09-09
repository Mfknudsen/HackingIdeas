using Sirenix.OdinInspector;
using UnityEngine;

namespace Idea_6
{
    [CreateAssetMenu(menuName = "Idea 6/Question")]
    public class Question : ScriptableObject
    {
        [SerializeField, Required] private Answer correctAnswer;
        [TextArea, SerializeField] private string question;

        public bool IsCorrect(Answer answer) => answer == this.correctAnswer;

        public Answer GetAnswer() => this.correctAnswer;

        public string GetText() => this.question;
    }
}