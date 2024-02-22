﻿using Microsoft.IdentityModel.Tokens;

namespace AuthFrontend.functionalities.loggingIn.JwtStuff
{
    public interface IJwtKeySetGetter
    {
        /// <summary>
        /// Gets the public keyset of a specidic identity provider
        /// </summary>
        /// <returns>The public keyset</returns>
        Task<JsonWebKeySet?> GetKeySet();
    }
}
