﻿namespace UserInfo.functionalities.user.dtos
{
    public struct UserWithEmailAndGroupDto
    {
        public Guid UserId { get; set; }
        public string[] Emails { get; set; }
        public string[] Groups { get; set; }
    }
}
