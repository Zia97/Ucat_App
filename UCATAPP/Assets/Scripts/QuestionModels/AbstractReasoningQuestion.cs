using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class AbstractReasoningQuestion
    {
        public String setImageUri;
        public String answer;
        public bool answerClicked = false;

        //QuestionNumber:URI:questionAnswer:usersAnswer
        public AbstractReasoningTupleHolder q1;
        public AbstractReasoningTupleHolder q2;
        public AbstractReasoningTupleHolder q3;
        public AbstractReasoningTupleHolder q4;
        public AbstractReasoningTupleHolder q5;

        public String userQuestion1Answer = "";
        public String userQuestion2Answer = "";
        public String userQuestion3Answer = "";
        public String userQuestion4Answer = "";
        public String userQuestion5Answer = "";

        public bool question1Answered = false;
        public bool question2Answered = false;
        public bool question3Answered = false;
        public bool question4Answered = false;
        public bool question5Answered = false;

        public bool flagged = false;


        public AbstractReasoningQuestion(String _setImageUri, String _answer)
        {
            setImageUri = _setImageUri;
            answer = _answer;
        }

        public AbstractReasoningTupleHolder LoadQuestion(string question)
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
                case "q5":
                    return q5;
            }
            return q1;
        }

        public void AddQuestion(int number, Tuple<int, String, String> question)
        {
            switch (number)
            {
                case 1:
                    q1 = new AbstractReasoningTupleHolder(question.Item1, question.Item2, question.Item3, "");
                    break;
                case 2:
                   q2 = new AbstractReasoningTupleHolder(question.Item1, question.Item2, question.Item3, "");
                    break;
                case 3:
                    q3 = new AbstractReasoningTupleHolder(question.Item1, question.Item2, question.Item3, "");
                    break;
                case 4:
                    q4 = new AbstractReasoningTupleHolder(question.Item1, question.Item2, question.Item3, "");
                    break;
                case 5:
                    q5 = new AbstractReasoningTupleHolder(question.Item1, question.Item2, question.Item3, "");
                    break;
            }

        }

    }
}
