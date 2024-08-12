using PMS.OperationPlateform.Application.IServices;
using PMS.OperationPlateform.Domain.IRespositories;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PMS.OperationPlateform.Application.Services
{
    public class JumpUrlService : IJumpUrlService
    {
        private readonly IJumpUrlRepository _jumpUrlRepository;
        public JumpUrlService(IJumpUrlRepository jumpUrlRepository)
        {
            _jumpUrlRepository = jumpUrlRepository;
        }


        public async Task AddCount(string url ,string fw)
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            DateTime today = DateTime.Now;
            string id = MD5Helper.GetMD5(url + (string.IsNullOrWhiteSpace(fw) ? "" : ("_" + fw)) + "_"+ today.ToString("yyyyMMdd"));//根据url+fw拼接后的字符串计算的md5作为id

            var jumpUrl = await _jumpUrlRepository.GetJumpUrl(id);
            if(jumpUrl == null)
            {
                await _jumpUrlRepository.AddJumpUrl(id, url, fw);
            }
            else
            {
                await _jumpUrlRepository.IncreJumpUrl(id);
            }
        }
    }
}
