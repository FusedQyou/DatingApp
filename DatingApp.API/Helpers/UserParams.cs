using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 50;

        public int PageNumber { get; set; } = 1;
        private int _pageSize = MaxPageSize;

        public int PageSize
        {
            get => _pageSize;
            set { _pageSize = Math.Clamp(value, 0, MaxPageSize); }
        }

        public int UserId { get; set; }
        public string Gender { get; set; }
        public int? MinimumAge { get; set; } = null;
        public int? MaximumAge { get; set; } = null;
        public string OrderBy { get; set; }
        public bool Likees { get; set; } = false;
        public bool Likers { get; set; } = false;
    }
}
