using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class VerbalReasoningTupleHolder
    {
        public int questionNumber;
        public string questionText;
        public string questionAnswer;
        public string usersAnswer;
        public string option1Label;
        public string option2Label;
        public string option3Label;
        public string option4Label;
        public bool answerClickedinTuple = false;
        public string totalQuestionNumber = "";


        public VerbalReasoningTupleHolder(int _questionNumber, string _questionText, string _questionAnswer, string _usersAnswer, string op1, string op2, string op3, string op4, string totalQNo)
        {
            questionNumber = _questionNumber;
            questionText = _questionText;
            questionAnswer = _questionAnswer;
            usersAnswer = _usersAnswer;
            option1Label = op1;
            option2Label = op2;
            option3Label = op3;
            option4Label = op4;
            totalQuestionNumber = totalQNo;
        }

        public void setAnswerClickedTrue()
        {
            answerClickedinTuple = true;
        }
    }
}
