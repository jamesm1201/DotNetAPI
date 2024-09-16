namespace DotNetAPI.Dtos
{
    public partial class UserForLoginConfirmDto
    {
        public byte[] PasswordHash {get; set;}
        public byte[] PasswordSalt {get; set;}
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