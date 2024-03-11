namespace AuthFrontend.functionalities.loggingIn.DTOs
{
    public struct RefreshTokenDto
    {
        public Guid JTI { get; set; }
        public Guid AuthUserId { get; set; }
        public string HashedToken { get; set; }
        public long Expire { get; set; }
        public string Salt { get; set; }
    }
}
