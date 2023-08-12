namespace Api.MessageLib.Interfaces
{
    public interface ILineMessageValidation
    {
        public bool Validate(string signature, object body);
    }
}