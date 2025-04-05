[System.Serializable]
public class UserSavedAnswerModel
{
    public int questionNumber;
    public string usersAnswer;

    private UserSavedAnswerModel() { }

    public class Builder
    {
        private int questionNumber;
        private string usersAnswer;

        public Builder SetQuestionNumber(int questionNumber)
        {
            this.questionNumber = questionNumber;
            return this;
        }

        public Builder SetUsersAnswer(string usersAnswer)
        {
            this.usersAnswer = usersAnswer;
            return this;
        }

        public UserSavedAnswerModel Build()
        {
            return new UserSavedAnswerModel
            {
                questionNumber = this.questionNumber,
                usersAnswer = this.usersAnswer
            };
        }
    }
}