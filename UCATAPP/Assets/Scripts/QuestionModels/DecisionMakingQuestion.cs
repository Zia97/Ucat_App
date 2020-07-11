using System;

namespace Assets.Scripts
{
    class DecisionMakingQuestion
    {
        public String resource;
        public bool hasImage;
        public bool answerClicked = false;
        public int questionCount = 0;
        public string imageUri;

        //QuestionNumber:URI:questionAnswer:usersAnswer
        public DecisionMakingTupleHolder q1;

        public String userQuestion1Answer = "";

        public bool question1Answered = false;

        public bool flagged = false;

        public DecisionMakingQuestion(String _resource, bool _hasImage, string _imageUri)
        {
            resource = _resource;
            hasImage = _hasImage;
            imageUri = _imageUri;
        }

        public DecisionMakingTupleHolder LoadQuestion(string question)
        {
            switch (question)
            {
                case "q1":
                    return q1;
            
            }
            return q1;
        }

        public void AddQuestion(int number, Tuple<int, String, String,String> question, Tuple<String,String,String,String> labels)
        {
            switch (number)
            {
                case 1:
                    q1 = new DecisionMakingTupleHolder(question.Item1, question.Item2, question.Item3, question.Item4,"", labels.Item1,labels.Item2,labels.Item3,labels.Item4);
                    questionCount = 1;
                    break;
            }

        }

    }
}
