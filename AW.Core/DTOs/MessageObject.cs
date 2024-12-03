using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AW.Core.DTOs.Interfaces;
using Newtonsoft.Json;

namespace AW.Core.DTOs
{
    public class MessageObject<T> : IMessageObject
    {
        public List<Message> Errors { get; }

        public List<Message> Warnings { get; }

        public List<Message> Confirmations { get; }

        public List<Message> Informations { get; }

        [JsonProperty]
        public bool ProcessingStatus { get; private set; } = true;

        public Exception Exception { get; private set; }

        public T Data { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MessageObject()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            Errors = new List<Message>();
            Warnings = new List<Message>();
            Confirmations = new List<Message>();
            Informations = new List<Message>();
        }

        public MessageObject(T Data)
            : this()
        {
            this.Data = Data;
        }

        public void AddException(Exception ex)
        {
            Exception = ex;
            ProcessingStatus = false;
        }

        public void AddMessage(Message msg)
        {
            switch (msg.Type)
            {
                case MessageType.Error:
                    Errors.Add(msg);
                    ProcessingStatus = false;
                    break;
                case MessageType.Warning:
                    Warnings.Add(msg);
                    break;
                case MessageType.Confirmation:
                    Confirmations.Add(msg);
                    break;
                default:
                    Informations.Add(msg);
                    break;
            }
        }

        public void AddMessage(MessageType type, string code, string desc, string field = "")
        {
            AddMessage(new Message(type, code, desc, field));
        }

        public void AddMessage(IMessageObject msg)
        {
            Errors.AddRange(msg.Errors);
            Warnings.AddRange(msg.Warnings);
            Confirmations.AddRange(msg.Confirmations);
            Informations.AddRange(msg.Informations);
            if (Exception == null)
            {
                Exception = msg.Exception;
            }

            if (ProcessingStatus && !msg.ProcessingStatus)
            {
                ProcessingStatus = msg.ProcessingStatus;
            }
        }

        public void AddMessage(List<IMessageObject> msgList)
        {
            if (msgList == null || msgList.Count <= 0)
            {
                return;
            }

            foreach (IMessageObject msg in msgList)
            {
                AddMessage(msg);
            }
        }

        public bool HasError()
        {
            return Errors.Count > 0;
        }
    }
}
