using System;
namespace PMS.School.Domain.Common
{
    public static class LodgingUtil
    {
        public static LodgingEnum Reason(bool? lodging, bool? sdextern)
        {
            if (lodging == true && sdextern == true)
            {
                return LodgingEnum.LodgingOrGo;
            }
            if (lodging == true)
            {
                return LodgingEnum.Lodging;
            }
            if (sdextern == true)
            {
                return LodgingEnum.Walking;
            }
            return LodgingEnum.Unkown;
        }
    }
}
