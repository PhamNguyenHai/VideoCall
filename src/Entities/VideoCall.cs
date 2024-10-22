using System;

namespace PetProject
{
    public class VideoCall
    {
        public Guid VideoCallId { set; get; }
        public DateTime StartTime { set; get; }
        public DateTime? EndTime { set; get; }
        public Guid ChatId { set; get; }
    }
}
