using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class VerbalReasoningQuestion
    {
        public String resource;
        public bool answerClicked = false;
        public int questionCount = 0;

        //QuestionNumber:URI:questionAnswer:usersAnswer
        public VerbalReasoningTupleHolder q1;
        public VerbalReasoningTupleHolder q2;
        public VerbalReasoningTupleHolder q3;
        public VerbalReasoningTupleHolder q4;

        public String userQuestion1Answer = "";
        public String userQuestion2Answer = "";
        public String userQuestion3Answer = "";
        public String userQuestion4Answer = "";

        public bool question1Answered = false;
        public bool question2Answered = false;
        public bool question3Answered = false;
        public bool question4Answered = false;

        public bool flagged = false;

        public string totalQuestionNumber = "";


        public VerbalReasoningQuestion(String _resource)
        {
            resource = _resource;
        }

        public VerbalReasoningTupleHolder LoadQuestion(string question)
        {
            switch (question)
            {
                case "q1":
                    return q1;
                case "q2":
                    return q2;
                case "q3":
                    return q3;
                case "q4":
                    return q4;
            }
            return q1;
        }

        public void AddQuestion(int number, Tuple<int, String, String> question, string op1, string op2, string op3, string op4, string totalQNo)
        {
            switch (number)
            {
                case 1:
                    q1 = new VerbalReasoningTupleHolder(question.Item1, question.Item2, question.Item3, "",op1,op2,op3,op4, totalQNo);
                    questionCount = 1;
                    break;
                case 2:
                   q2 = new VerbalReasoningTupleHolder(question.Item1, question.Item2, question.Item3, "",op1, op2, op3, op4, totalQNo);
                    questionCount = 2;
                    break;
                case 3:
                    q3 = new VerbalReasoningTupleHolder(question.Item1, question.Item2, question.Item3, "",op1, op2, op3, op4, totalQNo);
                    questionCount = 3;
                    break;
                case 4:
                    q4 = new VerbalReasoningTupleHolder(question.Item1, question.Item2, question.Item3, "",op1, op2, op3, op4, totalQNo);
                    questionCount = 4;
                    break;
            }

        }

    }
}
