using OnSolve.Mobile.Data.Enum;
using System;
using System.ComponentModel.DataAnnotations;

namespace OnSolve.Mobile.Data.Entites
{
    public class MessageDetail
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public long MessageTransactionId { get; set; }
        public long ContactPointId { get; set; }
        public bool IsGetWordBack { get; set; }
        public long RecipientId { get; set; }
        [Required]
        public DateTime DateTimeSent { get; set; }
        public MessageSenderDetail MessageSenderDetail { get; set; }
        [Required]
        public DateTime ExpirationDate { get; set; }
        public MobileUser MobileUser { get; set; }
        public MessageType MessageType { get; set; }
        public long MessageId { get; set; }
        public bool IsRead { get; set; }
        public bool IsConference { get; set; }
        public bool ResponseRecieved { get; set; }
        public bool IsVanishEnabled { get; set; }
        public bool IsDeleted { get; set; }
        public ConferenceBridgeDetail ConferenceBridgeDetail { get; set; }
    }
}