using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class VerbalReasoningQuestion
    {
        public string resource;
        public int questionNumber;
        public string questionText;
        public string answeringReason;
        public string questionAnswer;
        public string usersAnswer;
        public string option1Label;
        public string option2Label;
        public string option3Label;
        public string option4Label;
        public bool answerClicked = false;
        public string totalQuestionNumber = "";
        public bool flagged;


        public VerbalReasoningQuestion(string _resource, int _questionNumber, string _questionText, string _answeringReason, string _questionAnswer, string op1, string op2, string op3, string op4)
        {
            resource = _resource;
            questionNumber = _questionNumber;
            questionText = _questionText;
            answeringReason = _answeringReason;
            questionAnswer = _questionAnswer;
            option1Label = op1;
            option2Label = op2;
            option3Label = op3;
            option4Label = op4;
        }

        public void setUserAnswer(string _userAnswer)
        {
            usersAnswer = _userAnswer;
        }

        public void setAnswerClickedTrue()
        {
            answerClicked = true;
        }

        public void setFlagged(bool flag)
        {
            flagged = flag;
        }
    }
}
