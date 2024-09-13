namespace DotNetAPI.Models
{
    //Partial allows it to be added to from another class
    public partial class UserSalary {
        public int UserId {get; set;}
        public decimal Salary {get; set;}
        public decimal AvgSalary {get; set;}
    }
}