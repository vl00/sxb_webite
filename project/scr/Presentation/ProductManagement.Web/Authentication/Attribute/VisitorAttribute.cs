using System;
using Microsoft.AspNetCore.Authorization;

namespace ProductManagement.Web.Authentication.Attribute
{
    public class VisitorAttribute: AuthorizeAttribute
    {
        public VisitorAttribute()
        {
        }
    }
}
