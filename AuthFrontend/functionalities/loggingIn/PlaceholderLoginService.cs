namespace AuthFrontend.functionalities.loggingIn
{
    internal class PlaceholderLoginService : ILogInService
    {
        public string MakeAccessToken(UserInfoDto userInfo)
        {
            return string.Empty;
        }

        public UserInfoDto? ValidateToken(string token)
        {
            //return null;
            return new UserInfoDto
            {
                UserName = "Vico",
                Email = "e@e.e"
            };
        }
    }
}