using iSchool.Library.Common;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.School.Domain.Common;
using Sxb.Web.ViewModels.Sign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sxb.Web.Utils
{
    public static class LSRFSchoolDetailToVoHelper
    {
        public static List<LSRFSchoolDetailViewModel> LSRFSchoolDetailHelper(List<LSRFSchoolDetail> model)
        {
            if (model.Any())
            {
                List<LSRFSchoolDetailViewModel> lSRFSchools = new List<LSRFSchoolDetailViewModel>();
                foreach (var item in model)
                {
                    var lodgingType = LodgingUtil.Reason(item.Lodging, item.Sdextern);

                    LSRFSchoolDetailViewModel viewModel = new LSRFSchoolDetailViewModel();
                    viewModel.Eid = item.Eid;
                    viewModel.Sid = item.Sid;
                    viewModel.Sname = item.Sname;
                    viewModel.Count = item.Count == "" || String.IsNullOrEmpty(item.Count) ? -1 : int.Parse(item.Count);
                    viewModel.Type = item.Type;
                    viewModel.Intro = item.Intro;

                    viewModel.AdvType = item.AdvType;
                    viewModel.AdvPicUrl = item.AdvPicUrl;
                    viewModel.LodgingType = (int)lodgingType;
                    viewModel.LodgingReason = lodgingType.Description();
                    var couTemp = JsonToKV.ConvertObject(item.Abroad)?.Select(x => x.Key).ToList();
                    viewModel.Abroad = couTemp.Any() ? string.Join('、', couTemp) : "";

                    viewModel.Authentication = JsonToKV.ConvertObject(item.Authentication);
                    viewModel.Courses = JsonToKV.ConvertObject(item.Courses);

                    var subTemp = JsonToKV.ConvertObject(item.Subjects)?.Select(x => x.Key).ToList();
                    viewModel.Subjects = subTemp.Any() ? string.Join('、', subTemp) : "";
                    viewModel.Hardware = HardwareStringToList(item.Hardware);
                    lSRFSchools.Add(viewModel);
                }
                return lSRFSchools;
            }
            else
            {
                return new List<LSRFSchoolDetailViewModel>();
            }
        }

        public static List<string> HardwareStringToList(string hardware)
        {
            List<string> images = new List<string>();
            if (hardware == null || hardware == "")
            {
                return images;
            }

            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
            //string str = "";

            foreach (Match item in regImg.Matches(hardware))
            {
                images.Add(item.Groups[1].Value);
            }
            return images;
        }
    }
}
