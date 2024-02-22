namespace Api.LmcLib.Interfaces
{
    public interface ILineMessageValidation
    {
        public bool Validate(string signature, object body);
    }
}