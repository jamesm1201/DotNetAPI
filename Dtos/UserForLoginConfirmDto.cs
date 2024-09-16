namespace DotNetAPI.Dtos
{
    partial class UserForLoginConfirmDto
    {
        byte[] PasswordHash {get; set;}
        byte[] PasswordSalt {get; set;}
         public UserForLoginConfirmDto(){
            if(PasswordHash == null){
                PasswordHash = new byte[0];
            }
            if(PasswordSalt == null){
                PasswordSalt = new byte[0];
            }
        }
    }
}