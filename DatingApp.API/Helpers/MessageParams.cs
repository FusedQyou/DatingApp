using System;

namespace DatingApp.API.Helpers
{
    public class MessageParams
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
        public string MessageContainer { get; set; } = "Unread";
    }
}