using System.Runtime.Serialization;

namespace Brain.Utils;

[Serializable]
public class BrainException : Exception
{
    public BrainException()
    {
    }

    public BrainException(string message) : base(message)
    {
    }

    public BrainException(string message,
        Exception inner) : base(message, inner)
    {
    }

    protected BrainException(SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}
