using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class SituationalJudgementQuestion
    {
        public String resource;
        public int labelSet;
        public bool answerClicked = false;
        public int questionCount = 0;

        //QuestionNumber:URI:questionAnswer:usersAnswer
        public SituationalJudgementTupleHolder q1;
        public SituationalJudgementTupleHolder q2;
        public SituationalJudgementTupleHolder q3;
        public SituationalJudgementTupleHolder q4;

        public String userQuestion1Answer = "";
        public String userQuestion2Answer = "";
        public String userQuestion3Answer = "";
        public String userQuestion4Answer = "";

        public bool question1Answered = false;
        public bool question2Answered = false;
        public bool question3Answered = false;
        public bool question4Answered = false;


        public SituationalJudgementQuestion(String _resource, int _labelSet)
        {
            resource = _resource;
            labelSet = _labelSet;
        }

        public SituationalJudgementTupleHolder LoadQuestion(string question)
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

        public void AddQuestion(int number, Tuple<int, String, String> question)
        {
            switch (number)
            {
                case 1:
                    q1 = new SituationalJudgementTupleHolder(question.Item1, question.Item2, question.Item3, "");
                    questionCount = 1;
                    break;
                case 2:
                   q2 = new SituationalJudgementTupleHolder(question.Item1, question.Item2, question.Item3, "");
                    questionCount = 2;
                    break;
                case 3:
                    q3 = new SituationalJudgementTupleHolder(question.Item1, question.Item2, question.Item3, "");
                    questionCount = 3;
                    break;
                case 4:
                    q4 = new SituationalJudgementTupleHolder(question.Item1, question.Item2, question.Item3, "");
                    questionCount = 4;
                    break;
            }

        }

    }
}
