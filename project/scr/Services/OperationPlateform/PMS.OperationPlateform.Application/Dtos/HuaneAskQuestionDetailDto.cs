using System;
using System.Collections.Generic;

namespace PMS.OperationPlateform.Application.Dtos
{
    public class HuaneAskQuestionDetailDto
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public int Click { get; set; }
        public string CreateTime { get; set; }

        public List<HuaneAskAnswerDto> Answers { get; set; }

        public List<HuaneAskQuestionListDto> RecommentQuestions { get; set; }
    }
}
