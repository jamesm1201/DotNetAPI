namespace DotNetAPI.Models
{
    //Partial allows it to be added to from another class
    public partial class UserJobInfo {
        public int UserId {get; set;}
        public string JobTitle {get; set;}
        public string Department {get; set;}
        

    //In order to stop errors - string properties are set to empty instead of null
        public UserJobInfo(){
            if (JobTitle == null){
                JobTitle = "";
            }
            if (Department == null){
                Department = "";
            }
        }
    }
}