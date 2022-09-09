using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Idea_6
{
    public class ChooseHackSetup : MonoBehaviour
    {
        [SerializeField] private int goal, maxMissed;

        [SerializeField] private float timeBetween;

        [SerializeField] private TextMeshPro questionText;

        [SerializeField] private List<Question> questions = new List<Question>();

        [SerializeField] private List<AnswerButton> possibleAnswerPositions = new List<AnswerButton>();

        private int currentCorrect, currentWrong;
        private int questionIndex;
        private Coroutine currentCoroutine;
        private Question currentQuestion;

        private void Start()
        {
            if (this.questions.Count < this.possibleAnswerPositions.Count)
            {
                this.questionText.text = "Not enough possible answers";
                return;
            }
            
            this.questionText.text = this.goal + " corrects => Victory\n" + this.maxMissed + " wrongs => Defeat";
            this.currentCoroutine = StartCoroutine(ShowNewQuestion());
        }

        private IEnumerator ShowNewQuestion()
        {
            yield return new WaitForSeconds(2);

            this.questionIndex.UniqueIndex(this.questions.Count);

            this.currentQuestion = this.questions[this.questionIndex];

            this.questionText.text = this.currentQuestion.GetText();

            this.possibleAnswerPositions[
                    Random.Range(0, this.possibleAnswerPositions.Count)]
                .SetAnswer(this.currentQuestion.GetAnswer());

            List<int> usedIndexes = new List<int> { this.questionIndex };
            foreach (AnswerButton answerPos in this.possibleAnswerPositions
                         .Where(answerPos => answerPos.answer == null))
            {
                int i = Random.Range(0, this.questions.Count);

                while (usedIndexes.Contains(i))
                    i = Random.Range(0, this.questions.Count);

                usedIndexes.Add(i);

                answerPos.SetAnswer(this.questions[i].GetAnswer());
            }

            yield return new WaitForSeconds(this.timeBetween);

            AnswerQuestion(null);
        }

        public void AnswerQuestion(Answer answer)
        {
            foreach (AnswerButton answerPos in this.possibleAnswerPositions)
                answerPos.SetAnswer(null);
            
            if (this.currentQuestion.IsCorrect(answer))
            {
                this.currentCorrect++;
                this.questionText.text = "Correct: " + this.currentCorrect + "/" + this.goal;
            }
            else
            {
                currentWrong++;
                this.questionText.text = "Wrong: " + this.currentWrong;
            }

            StopCoroutine(currentCoroutine);

            if (this.currentCorrect < this.goal && this.currentWrong < this.maxMissed)
            {
                this.currentCoroutine = StartCoroutine(ShowNewQuestion());
                return;
            }

            this.questionText.text = this.currentCorrect >= this.goal ? "Victory" : "Defeat";
        }
    }
}