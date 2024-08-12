using PMS.CommentsManage.Application.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagement.Web.Common
{
    public class JobAdminService
    {
        private readonly IPartTimeJobAdminService _timeJobAdminService;

        public JobAdminService(IPartTimeJobAdminService partTimeJobAdmin)
        {
            _timeJobAdminService = partTimeJobAdmin;
        }

        //随机生成注册码
        public  string RandomCode()
        {
            List<string> code = new List<string>();
            for (int i = 65; i <= 90; i++)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] btNumber = new byte[] { (byte)i };
                code.Add(asciiEncoding.GetString(btNumber));
            }

            for (int i = 0; i <= 9; i++)
            {
                code.Add(i.ToString());
            }

            string str = "";
            for (int i = 0; i < 8; i++)
            {
                Random random = new Random();
                int rez = random.Next(0, code.Count);
                str += code[rez];
            }

            //检测随机生成邀请码是否存在，注册码系统中唯一（用来登录，以及查找父级） ， 存在则继续递归生成邀请码
            return _timeJobAdminService.CheckCodeExists(str) == false ? str : RandomCode();
        }
    }
}
