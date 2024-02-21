namespace AuthFrontend.functionalities.loggingIn
{
    internal class PlaceholderLoginService : ILogInService
    {
        public string MakeAccessToken(UserInfoDto userInfo)
        {
            return string.Empty;
        }

        public Task<UserInfoDto?> ValidateToken(string token)
        {
            //return null;
            return Task.FromResult<UserInfoDto?>(new UserInfoDto
            {
                UserName = "Vico",
                Email = "e@e.e"
            });
        }
    }
}