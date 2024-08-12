namespace ProductManagement.Tool.HttpRequest.Encrypt
{
    public class DefaultEncrypt : IEncrypt
    {
        public const string Default = "default";

        public string Encrypt(string data)
        {
            return data;
        }
    }
}
