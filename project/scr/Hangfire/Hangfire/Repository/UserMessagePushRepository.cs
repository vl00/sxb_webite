using Hangfire.ConsoleWeb.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hangfire.ConsoleWeb.Repository
{
    public class UserMessagePushRepository
    {
        private DataDbContext _context;

        public UserMessagePushRepository(DataDbContext context)
        {
            _context = context;
        }


        public int MessagePush(List<Message> mes) 
        {
            string sql = @"insert into message(id,userID,senderID,type,title,content,dataID,dataType,eID,[time],push)
                            values(@id,@userID,@senderID,@type,@title,@content,@dataID,@dataType,@eID,@time,@push)";

            return _context.Execute(sql,mes);
        }

    }
}
