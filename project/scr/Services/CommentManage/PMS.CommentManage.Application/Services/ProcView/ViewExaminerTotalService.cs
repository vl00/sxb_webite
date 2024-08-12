using PMS.CommentsManage.Application.IServices.IProcViewService;
using PMS.CommentsManage.Domain.Entities.ProcViewEntities;
using PMS.CommentsManage.Domain.Entities.Total;
using PMS.CommentsManage.Domain.IRepositories.IProcViewRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using PMS.CommentsManage.Application.ModelDto;

namespace PMS.CommentsManage.Application.Services.ProcView
{
    public class ViewExaminerTotalService : IViewExaminerTotalService
    {
        private readonly IViewExaminerTotalRepository _repo;
        public ViewExaminerTotalService(IViewExaminerTotalRepository repo)
        {
            _repo = repo;
        }

        public SupplierTotalDto GetSupplierTotal(Guid SupplierId, DateTime BeginTime, DateTime EndTime)
        {
            var supplierTotal = _repo.GetSupplierTotal(SupplierId, BeginTime, EndTime);
            if (supplierTotal == null)
            {
                return new SupplierTotalDto();
            }
            else 
            {
                return new SupplierTotalDto()
                {
                    CommitAnswerTotal = supplierTotal.CommitAnswerTotal,
                    ExaminerAnswerTotal = supplierTotal.ExaminerAnswerTotal,
                    ExaminerCommentTotal = supplierTotal.ExaminerCommentTotal,
                    PartTimeJobAdminTotal = supplierTotal.PartTimeJobAdminTotal,
                    ShieldJobTotal = supplierTotal.ShieldJobTotal,
                    SelectedAnswerTotal = supplierTotal.SelectedAnswerTotal,
                    SelectedCommentTotal = supplierTotal.SelectedCommentTotal,
                    CommitCommentTotal = supplierTotal.CommitCommentTotal,
                    ShieldJobAnswerTotal = supplierTotal.ShieldJobAnswerTotal,
                    ShieldJobCommentTotal = supplierTotal.ShieldJobCommentTotal,
                    ShieldJobSelectedAnswerTotal = supplierTotal.ShieldJobSelectedAnswerTotal,
                    ShieldJobSelectedCommentTotal = supplierTotal.ShieldJobSelectedCommentTotal
                };
            }
        }

        /// <summary>
        /// /兼职领队页数据统计
        /// </summary>
        /// <param name="ParentId"></param>
        /// <param name="BeginTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public SupplierTotalDto GetLeaderCurrentMonthTotal(Guid ParentId, DateTime BeginTime, DateTime EndTime,string Phone) 
        {
            var supplierTotal = _repo.GetLeaderCurrentMonthTotal(ParentId, BeginTime, EndTime, Phone);
            if (supplierTotal == null)
            {
                return new SupplierTotalDto();
            }
            else
            {
                return new SupplierTotalDto()
                {
                    CommitAnswerTotal = supplierTotal.CommitAnswerTotal,
                    ExaminerAnswerTotal = supplierTotal.ExaminerAnswerTotal,
                    ExaminerCommentTotal = supplierTotal.ExaminerCommentTotal,
                    PartTimeJobAdminTotal = supplierTotal.PartTimeJobAdminTotal,
                    ShieldJobTotal = supplierTotal.ShieldJobTotal,
                    SelectedAnswerTotal = supplierTotal.SelectedAnswerTotal,
                    SelectedCommentTotal = supplierTotal.SelectedCommentTotal,
                    CommitCommentTotal = supplierTotal.CommitCommentTotal,
                    ShieldJobAnswerTotal = supplierTotal.ShieldJobAnswerTotal,
                    ShieldJobCommentTotal = supplierTotal.ShieldJobCommentTotal,
                    ShieldJobSelectedAnswerTotal = supplierTotal.ShieldJobSelectedAnswerTotal,
                    ShieldJobSelectedCommentTotal = supplierTotal.ShieldJobSelectedCommentTotal
                };
            }
        }

        public ViewExaminerTotal GetViewExaminerTotal()
        {
            return _repo.GetViewExaminerTotal();
        }

        public SysAdminQuerySupplierTotalDto SysAdminQuerySupplierTotal(DateTime BeginTime, DateTime EndTime)
        {
            var supplierTotalDto = _repo.SysAdminQuerySupplierTotal(BeginTime, EndTime);
            if (supplierTotalDto == null) 
            {
                return new SysAdminQuerySupplierTotalDto();
            }
            else 
            {
                return new SysAdminQuerySupplierTotalDto()
                {
                    ExaminerAnswerTotal = supplierTotalDto.ExaminerAnswerTotal,
                    ExaminerCommentTotal = supplierTotalDto.ExaminerCommentTotal,
                    SelectedAnswerTotal = supplierTotalDto.SelectedAnswerTotal,
                    SelectedCommentTotal = supplierTotalDto.SelectedCommentTotal,
                    ShieldJobAnswerTotal = supplierTotalDto.ShieldJobAnswerTotal,
                    ShieldJobCommentTotal = supplierTotalDto.ShieldJobCommentTotal,
                    ShieldJobSelectedAnswerTotal = supplierTotalDto.ShieldJobSelectedAnswerTotal,
                    ShieldJobSelectedCommentTotal = supplierTotalDto.ShieldJobSelectedCommentTotal,
                    ShieldJobTotal = supplierTotalDto.ShieldJobTotal
                };
            }
        }
    }
}
