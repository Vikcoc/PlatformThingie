namespace AuthFrontend.entities
{
    internal class AuthUserAuth
    {
        // todo rethink https://stackoverflow.com/questions/28907831/how-to-use-jti-claim-in-a-jwt
        public Guid AuthUserId { get; set; }
        public string HashedToken { get; set; } = string.Empty;
        public long Expire {  get; set; }
        public string Salt { get; set; } = string.Empty;
    }
}
