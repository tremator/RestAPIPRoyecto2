namespace ProyectoWeb2Api.Models
{
    public class Email
    {
        public  string to { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public string fromEmail { get; set; }
        public string fromPassword { get; set; }

        public Email(){
            this.fromEmail = "dmsolisa@gmail.com";
            this.fromPassword = "dmsolisa";
        }
    }
}