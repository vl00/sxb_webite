using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using PMS.TopicCircle.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using static PMS.TopicCircle.Domain.Enum.GlobalEnum;

namespace PMS.TopicCircle.Domain.Entities
{
    public partial class Topic
    {
        /// <summary>
        /// 是否公开
        /// </summary>
        public bool IsOpen { get; set; }

        /// <summary>
        /// 动态时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 评论列表
        /// </summary>
        public IEnumerable<TopicReply> Replies { get; set; }

        /// <summary>
        /// 关联附件
        /// </summary>
        public TopicReplyAttachment Attachment { get; set; }

        /// <summary>
        /// 关联图片
        /// </summary>
        public IEnumerable<TopicReplyImage> Images { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public IEnumerable<TopicTag> Tags { get; set; }


        public Topic()
        {
        }

        public Topic(bool isOpen, Guid id, string content,  bool isQA, Guid userId, Circle circle)
        {
            SetTopic(isOpen, id, content, isQA, userId, userId, circle);

            //新增
            var current = DateTime.Now;
            CreateTime = current;
            UpdateTime = current;
            LastReplyTime = current;
            LastEditTime = current;
        }

        public void SetTopic(bool isOpen, Guid id, string content, bool isQA, Guid creator, Guid updator, Circle circle)
        {
            //默认 1文字  
            //类型 0 无 1 文本 2 图片 4 直播 8 文章 16 院校 32 点评 64 回答
            Type = (int)TopicType.Text;

            IsOpen = isOpen;
            Id = id;
            CircleId = circle.Id;
            Content = content;
            IsQA = (byte)(isQA ? 1 : 0);
            Creator = creator;
            Updator = updator;

            //更新话题
            var current = DateTime.Now;
            UpdateTime = current;
            LastEditTime = current;

            if (!IsOpen && circle.UserId != null) OpenUserId = circle.UserId;
            else OpenUserId = null;
        }

        public bool SetAttachment(Guid replyId, string content, Guid? attchId, string attachUrl, TopicType type)
        {
            if (string.IsNullOrWhiteSpace(content) || type == TopicType.None)
                return false;

            var typee = (int)type;
            if (type != TopicType.OuterUrl)
                Type = typee;

            if (Attachment == null)
                Attachment = new TopicReplyAttachment() { Id = Guid.NewGuid() };


            Attachment.IsDeleted = 0;
            Attachment.TopicId = Id;
            Attachment.TopicReplyId = replyId;
            Attachment.AttachId = attchId;
            Attachment.Content = content;
            Attachment.AttachUrl = attachUrl;
            Attachment.Type = typee;

            return true;
        }

        public void SetImages(IEnumerable<TopicReplyImage> images)
        {
            if (Attachment == null)
                Type = (int)TopicType.Image;
            Images = images;
        }


        public IEnumerable<TopicReply> GetChildren()
        {
            return Replies.Where(s => s.Depth != 0);
        }


        /// <summary>
        /// 获取前count个回复
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<TopicReply> GetChildren(int count)
        {
            if (count <= 0)
                return GetChildren();

            List<TopicReply> newReplies = new List<TopicReply>();
            List<TopicReply> repliesD1 = Replies.Where(s => s.Depth == 1).ToList();
            repliesD1.Sort((x, y) => x.CreateTime > y.CreateTime ? 1: 0);

            int surplus = count;
            foreach (var reply in repliesD1)
            {
                if (surplus <= 0)
                    break;

                surplus--;
                newReplies.Add(reply);

                if (surplus == 0)
                    break;

                List<TopicReply> children = Replies.Where(s => s.FirstParentId == reply.Id).ToList();
                surplus -= children.Count;
                surplus = surplus <= 0 ? 0 : surplus;
                if (surplus != 0)
                {
                    newReplies.AddRange(children.GetRange(0, 8));
                }
            }
            return newReplies;
        }

        /// <summary>
        /// 获取评论的所有子评论
        /// </summary>
        /// <param name="topicReplyId"></param>
        /// <returns></returns>
        public IEnumerable<TopicReply> GetReplyChildren(Guid topicReplyId)
        {
            return Replies.Where(s => s.FirstParentId == topicReplyId);
        }

        /// <summary>
        /// 递归获取评论的所有子评论
        /// </summary>
        /// <param name="topicReplyId"></param>
        /// <returns></returns>
        public IEnumerable<TopicReply> GetReplyChildrenRe(Guid topicReplyId)
        {
            IEnumerable<TopicReply> children = Replies.Where(s => s.ParentId == topicReplyId);
            if (children.Count() > 0)
            {
                foreach (var child in children)
                {
                    children.Concat(GetReplyChildrenRe(child.Id));
                }
            }
            return children;
        }
    }
}
