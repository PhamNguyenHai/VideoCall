using PetProject.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace PetProject
{
    public class VideoCall : IHasKey
    {
        public Guid VideoCallId { set; get; }

        [Required(ErrorMessage = "Thời gian bắt đầu không được phép để trống")]
        public DateTime StartTime { set; get; }
        public DateTime? EndTime { set; get; }
        public Guid ChatId { set; get; }

        public Guid GetKey()
        {
            return VideoCallId;
        }

        public void SetKey(Guid key)
        {
            VideoCallId = key;
        }
    }
}
