using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class DecisionMakingTupleHolder
    {
        public int questionNumber;
        public string questionText;
        public string questionAnswer;
        public string usersAnswer;
        public string answerReasoning;
        public string option1Label;
        public string option2Label;
        public string option3Label;
        public string option4Label;
        public bool answerClickedinTuple;


        public DecisionMakingTupleHolder(int _questionNumber, string _questionText, string _questionAnswer, string _answerReasoning, string _usersAnswer, string _opt1, string _opt2, string _opt3, string _opt4)
        {
            questionNumber = _questionNumber;
            questionText = _questionText;
            questionAnswer = _questionAnswer;
            usersAnswer = _usersAnswer;
            answerReasoning = _answerReasoning;
            option1Label = _opt1;
            option2Label = _opt2;
            option3Label = _opt3;
            option4Label = _opt4;
            answerClickedinTuple = false;
        }

        public void setAnswerClickedTrue()
        {
            answerClickedinTuple = true;
        }
    }
}
