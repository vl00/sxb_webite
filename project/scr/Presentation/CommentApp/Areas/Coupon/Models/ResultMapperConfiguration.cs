using AutoMapper;
using PMS.UserManage.Domain;
using PMS.UserManage.Domain.Dtos;
using PMS.UserManage.Domain.Entities;
using Sxb.Web.Areas.Coupon.Models.Coupon;
using System;
using System.Linq.Expressions;
namespace Sxb.Web.Areas.Coupon.Models
{
    public class ResultMapperConfiguration : Profile
    {

        public ResultMapperConfiguration()
        {
            _ = CreateMap<CouponTakeDto, CouponTakeResult>()
                  .ForMember(t => t.CouponNo, opt => opt.MapFrom(s => s.CouponNo.ToString("D8")))
                  .ForMember(t => t.Status, opt => opt.MapFrom(s => CouponTakeStatusToTakeStatus(s)))
                  .ForMember(t => t.IsNewCoupon, opt => opt.MapFrom(s => s.ReadTime == null));


            _ = CreateMap<CouponInfo, CouponInfoResult>();

        }

        public TakeStatus CouponTakeStatusToTakeStatus(CouponTakeDto couponTakeDto)
        {
            if (DateTime.Now > couponTakeDto.VaildEndTime)
            {
                return TakeStatus.Failure;
            }
            else
            {
                return (TakeStatus)((int)couponTakeDto.Status);
            }
        }
    }
}
