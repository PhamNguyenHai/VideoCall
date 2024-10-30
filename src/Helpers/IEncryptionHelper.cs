namespace PetProject
{
    public interface IEncryptionHelper
    {
        public string Encrypt(string plainText);
        public string Decrypt(string cipherText);
    }
}
