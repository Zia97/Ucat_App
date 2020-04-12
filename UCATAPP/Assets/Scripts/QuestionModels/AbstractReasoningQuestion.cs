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
        public GenericTupleHolder q1;
        public GenericTupleHolder q2;
        public GenericTupleHolder q3;
        public GenericTupleHolder q4;
        public GenericTupleHolder q5;

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

        public GenericTupleHolder LoadQuestion(string question)
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
                    q1 = new GenericTupleHolder(question.Item1, question.Item2, question.Item3, "");
                    break;
                case 2:
                   q2 = new GenericTupleHolder(question.Item1, question.Item2, question.Item3, "");
                    break;
                case 3:
                    q3 = new GenericTupleHolder(question.Item1, question.Item2, question.Item3, "");
                    break;
                case 4:
                    q4 = new GenericTupleHolder(question.Item1, question.Item2, question.Item3, "");
                    break;
                case 5:
                    q5 = new GenericTupleHolder(question.Item1, question.Item2, question.Item3, "");
                    break;
            }

        }

    }
}
