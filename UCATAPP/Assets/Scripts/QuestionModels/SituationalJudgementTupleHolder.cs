using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class SituationalJudgementTupleHolder
    {
        public int questionNumber;
        public string questionText;
        public string questionAnswer;
        public string usersAnswer;
        public bool answerClickedinTuple = false;

        public SituationalJudgementTupleHolder(int _questionNumber, string _questionText, string _questionAnswer, string _usersAnswer)
        {
            questionNumber = _questionNumber;
            questionText = _questionText;
            questionAnswer = _questionAnswer;
            usersAnswer = _usersAnswer;
        }

        public void setAnswerClickedTrue()
        {
            answerClickedinTuple = true;
        }
    }
}
