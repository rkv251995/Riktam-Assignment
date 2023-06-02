namespace Assignment.Utilities.Helpers
{
    [Serializable]
    public class RaiseError : Exception
    {
        public RaiseError(string message) : base(message) { }
    }
}
