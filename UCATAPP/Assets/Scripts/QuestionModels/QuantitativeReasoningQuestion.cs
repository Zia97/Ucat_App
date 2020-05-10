using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class QuantitativeReasoningQuestion
    {
        public String resource;
        public bool hasImage;
        public bool answerClicked = false;
        public int questionCount = 0;
        public string imageUri;

        //QuestionNumber:URI:questionAnswer:usersAnswer
        public QuantitativeReasoningTupleHolder q1;
        public QuantitativeReasoningTupleHolder q2;
        public QuantitativeReasoningTupleHolder q3;
        public QuantitativeReasoningTupleHolder q4;

        public String userQuestion1Answer = "";
        public String userQuestion2Answer = "";
        public String userQuestion3Answer = "";
        public String userQuestion4Answer = "";

        public bool question1Answered = false;
        public bool question2Answered = false;
        public bool question3Answered = false;
        public bool question4Answered = false;


        public QuantitativeReasoningQuestion(String _resource, bool _hasImage, string _imageUri)
        {
            resource = _resource;
            hasImage = _hasImage;
            imageUri = _imageUri;
        }

        public QuantitativeReasoningTupleHolder LoadQuestion(string question)
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

        public void AddQuestion(int number, Tuple<int, String, String,String> question, Tuple<String,String,String,String,String> labels)
        {
            switch (number)
            {
                case 1:
                    q1 = new QuantitativeReasoningTupleHolder(question.Item1, question.Item2, question.Item3, question.Item4,"", labels.Item1,labels.Item2,labels.Item3,labels.Item4,labels.Item5);
                    questionCount = 1;
                    break;
                case 2:
                   q2 = new QuantitativeReasoningTupleHolder(question.Item1, question.Item2, question.Item3, question.Item4, "", labels.Item1, labels.Item2, labels.Item3, labels.Item4, labels.Item5);
                    questionCount = 2;
                    break;
                case 3:
                    q3 = new QuantitativeReasoningTupleHolder(question.Item1, question.Item2, question.Item3, question.Item4, "", labels.Item1, labels.Item2, labels.Item3, labels.Item4, labels.Item5);
                    questionCount = 3;
                    break;
                case 4:
                    q4 = new QuantitativeReasoningTupleHolder(question.Item1, question.Item2, question.Item3, question.Item4, "", labels.Item1, labels.Item2, labels.Item3, labels.Item4, labels.Item5);
                    questionCount = 4;
                    break;
            }

        }

    }
}
