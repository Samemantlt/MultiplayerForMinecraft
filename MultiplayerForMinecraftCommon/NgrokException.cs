using System;

namespace MultiplayerForMinecraftCommon
{
    [Serializable]
    public class NgrokException : Exception
    {
        public NgrokException() { }
        public NgrokException(string message) : base(message) { }
        public NgrokException(string message, Exception inner) : base(message, inner) { }
        protected NgrokException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
