using UnityEngine;

namespace Idea_6
{
    [CreateAssetMenu(menuName = "Idea 6/Answer")]
    public class Answer : ScriptableObject
    {
        [TextArea, SerializeField] private string answer;

        public string GetAnswer() => this.answer;
    }
}