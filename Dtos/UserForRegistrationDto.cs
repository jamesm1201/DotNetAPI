namespace DotNetAPI.Dtos
{
    partial class UserForRegistrationDto
    {
        string Email {get; set;}
        string Password {get; set;}
        string PasswordConfirm {get; set;}

        public UserForRegistrationDto(){
            if(Email == null){
                Email = "";
            }
            if(Password == null){
                Password = "";
            }
            if(PasswordConfirm == null){
                PasswordConfirm = "";
            }
        }
    }
}