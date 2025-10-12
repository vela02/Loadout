using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Features.Common
{
    public sealed class PageRequest
    {
        private const int MaxPageSize = 100;
        public int Page { get; init; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            init => _pageSize = value <= 0 ? 10 : value > MaxPageSize ? MaxPageSize : value;
        }

        public int SkipCount => (Page - 1) * PageSize;
    }
}
