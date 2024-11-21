using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AW.Core.DTOs
{
    public class Message
    {
        public MessageType Type { get; }

        public string Code { get; }

        public string Description { get; }

        public string Field { get; } = string.Empty;

        public Message(MessageType type, string code, string description, string field = "")
        {
            Type = type;
            Code = code;
            Description = description;
            Field = field;
        }
    }
    public enum MessageType
    {
        Error,
        Warning,
        Information,
        Confirmation
    }

}
