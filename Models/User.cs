namespace DotNetAPI
{
    //Partial allows it to be added to from another class
    public partial class User {
        public int UserId {get; set;}
        public string FirstName {get; set;}
        public string LastName {get; set;}
        public string Email {get; set;}
        public string Gender {get; set;}
        public bool Active {get; set;}

    //In order to stop errors - string properties are set to empty instead of null
        public User(){
            if (FirstName == null){
                FirstName = "";
            }
            if (LastName == null){
                LastName = "";
            }
            if (Email == null){
                Email = "";
            }
            if (Gender == null){
                Gender = "";
            }
        }
    }
}