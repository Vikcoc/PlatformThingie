﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthFrontend.entities
{
    [PrimaryKey(nameof(AuthUserId), nameof(AuthClaimName))]
    internal class AuthUserClaim
    {
        
        public Guid AuthUserId { get; set; }
        
        [ForeignKey(nameof(AuthClaim))]
        public required string AuthClaimName { get; set; }

        public string ClaimValue { get; set; } = string.Empty;

        public required AuthUser AuthUser { get; set; }
        public required AuthClaim AuthClaim { get; set; }
    }
}