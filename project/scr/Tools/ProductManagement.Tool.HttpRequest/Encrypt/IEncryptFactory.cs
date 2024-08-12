namespace ProductManagement.Tool.HttpRequest.Encrypt
{
    public interface IEncryptFactory
    {
        IEncrypt Instantiation(string key);
    }
}
