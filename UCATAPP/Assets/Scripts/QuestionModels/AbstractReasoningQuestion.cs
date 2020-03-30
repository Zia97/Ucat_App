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

        //QuestionNumber:URI:questionAnswer:usersAnswer
        public Tuple<int, String, String, String> q1;
        public Tuple<int, String, String, String> q2;
        public Tuple<int, String, String, String> q3;
        public Tuple<int, String, String, String> q4;
        public Tuple<int, String, String, String> q5;

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


        public AbstractReasoningQuestion(String _setImageUri)
        {
            setImageUri = _setImageUri;
        }

        public Tuple<int,String,String,String> LoadQuestion(string question)
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
                    q1 = new Tuple<int, string, string, string>(question.Item1, question.Item2, question.Item3, "");
                    break;
                case 2:
                   q2 = new Tuple<int, string, string, string>(question.Item1, question.Item2, question.Item3, "");
                    break;
                case 3:
                    q3 = new Tuple<int, string, string, string>(question.Item1, question.Item2, question.Item3, "");
                    break;
                case 4:
                    q4 = new Tuple<int, string, string, string>(question.Item1, question.Item2, question.Item3, "");
                    break;
                case 5:
                    q5 = new Tuple<int, string, string, string>(question.Item1, question.Item2, question.Item3, "");
                    break;
            }

        }


        public void AnswerSelected(string question, string answer)
        {
            switch (question)
            {
                case "q1":
                    question1Answered = true;
                    q1 = new Tuple<int,string, string, string>(q1.Item1, q1.Item2,q1.Item3,answer);
                    break;
                case "q2":
                    question2Answered = true;
                    q2 = new Tuple<int,string, string, string>(q2.Item1, q2.Item2, q2.Item3,answer);
                    break;
                case "q3":
                    question3Answered = true;
                    q3 = new Tuple<int,string, string, string>(q3.Item1, q3.Item2,q3.Item3, answer);
                    break;
                case "q4":
                    question4Answered = true;
                    q4 = new Tuple<int,string, string, string>(q4.Item1, q4.Item2,q4.Item3, answer);
                    break;
                case "q5":
                    question5Answered = true;
                    q5 = new Tuple<int,string, string, string>(q5.Item1, q5.Item2, q5.Item3, answer);
                    break;
            }

        }
    }
}
