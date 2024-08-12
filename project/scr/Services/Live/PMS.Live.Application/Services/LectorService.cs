using PMS.Live.Application.IServices;
using PMS.Live.Domain.Dtos;
using PMS.Live.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.Live.Application.Services
{
    public class LectorService : ILectorService
    {
        ILectureRepository _lectureRepository;
        public LectorService(ILectureRepository lectureRepository)
        {
            _lectureRepository = lectureRepository;
        }
    }
}
