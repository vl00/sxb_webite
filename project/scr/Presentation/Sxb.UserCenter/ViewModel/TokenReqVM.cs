using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.UserCenter.ViewModel
{
    public class TokenReqVM
    {
        public string AppId { get; set; }

        public string Secret { get; set; }

        public GrantType GrantType { get; set; }
    }

    public enum GrantType
    {
        Unknown,
        Token,
        AllowAll
    }
}
