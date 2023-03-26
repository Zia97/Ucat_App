﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    class SituationalJudgementQuestion
    {

        public int questionNumber;
        public String resource;
        public bool answerClicked = false;

        public string questionText;
        public string questionAnswer;
        public string usersAnswer;
        public int labelSet;


        public bool flagged = false;


        public SituationalJudgementQuestion(String _resource, int _questionNumber, String _questionText, String _answer, int _labelSet)
        {
            resource = _resource;
            questionText = _questionText;
            questionNumber = _questionNumber;
            questionAnswer = _answer;
            labelSet = _labelSet;
        }

    }
}
