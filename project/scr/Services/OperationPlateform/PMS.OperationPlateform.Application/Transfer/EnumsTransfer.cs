using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Transfer
{
    public class EnumsTransfer
    {
        public static OperationPlateform.Domain.Enums.SCMRole UserRoleEnumToSCMRoleEnum(UserManage.Domain.Common.UserRole userRole)
        {
            switch (userRole)
            {
                case UserManage.Domain.Common.UserRole.School:
                    return Domain.Enums.SCMRole.SA;
                case UserManage.Domain.Common.UserRole.PersonTalent:
                    return Domain.Enums.SCMRole.SA;
                case UserManage.Domain.Common.UserRole.OrgTalent:
                    return Domain.Enums.SCMRole.SA;
                default:
                    return default( Domain.Enums.SCMRole);
            }
        }
    }
}
