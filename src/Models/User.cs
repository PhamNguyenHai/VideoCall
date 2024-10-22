namespace PetProject.Models
{
    public class User
    {
        public string Username { get; set; }
        public string ConnectionId { get; set; }
        public bool InCall { get; set; }

        public bool IsCameraEnabled { get; set; } = true; // Thêm thuộc tính cho camera
        public bool IsMicrophoneEnabled { get; set; } = true; // Thêm thuộc tính cho microphone
    }
}
