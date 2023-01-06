namespace AthenaBot.Plugins
{
    public interface IErrorInfo
    {
        string Name { get; }
        string Method { get; }

        Exception Exception { get; }
    }
}
