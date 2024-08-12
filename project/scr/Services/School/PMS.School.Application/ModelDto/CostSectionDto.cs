using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.School.Application.ModelDto
{
    public class CostSectionMemberDto
    {
        protected static int _MAX = 999999999;

        public int Min { get; set; }
        public int Max { get; set; }

        public static CostSectionMemberDto Create(int? min, int? max)
        {
            return new CostSectionMemberDto()
            {
                Min = min != null ? min.Value : 0,
                Max = max != null ? max.Value : _MAX
            };
        }

        public string GetFormatString()
        {
            if (Min == 0)
            {
                return $"{Max}以内";
            }

            var minString = Min >= 10000 ? $"{MathF.Round(Min / 10000, 2)}万" : Min.ToString();
            if (Max == _MAX)
            {
                return $"{minString}以上";
            }

            var maxString = Max >= 10000 ? $"{MathF.Round(Max / 10000, 2)}万" : Max.ToString();
            return $"{minString}~{maxString}";
        }

    }

    public class CostSectionDto : CostSectionMemberDto
    {

        //public double Score { get; set; }
        public double Ratio { get; set; }

        public string RatioString => $"{MathF.Round((float)Ratio * 100, 2)}%";

        public string FormatString => GetFormatString();

        public static IEnumerable<CostSectionDto> Default => DefaultData();

        private static IEnumerable<CostSectionDto> DefaultData()
        {
            yield return new CostSectionDto()
            {
                Min = 0,
                Max = 1000,
                Ratio = 0.3
            };
            yield return new CostSectionDto()
            {
                Min = 1000,
                Max = 10000,
                Ratio = 0.4
            };
            yield return new CostSectionDto()
            {
                Min = 10000,
                Max = _MAX,
                Ratio = 0.3
            };
        }

    }
}
