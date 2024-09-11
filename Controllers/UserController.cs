
using Microsoft.AspNetCore.Mvc;
namespace DotNetAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class UserController : ControllerBase
{
    DataContextDapper _dapper; 
    public UserController(IConfiguration config){
        //Constructor to be used later
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("TestConnection")]
    public DateTime TestConnection(){
        return _dapper.LoadDataSingle<DateTime>("SELECT.GETDATE()");
    }
    //IActionResult means API response
    // public IActionResult Test()
    
    [HttpGet("GetUsers")]
    //This will return an IEnumerable of all user models in DB
    public IEnumerable<User> GetUsers()
    {
        /*SQl command for dapper to use
        @ allows for multiple lines of string*/
        string sql = @"
            SELECT[UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active]
            FROM TutorialAppSchema.Users";
        /*_dapper is instance of dataContextDapper 
        it is passed the sql and result is put into users*/
        IEnumerable<User> users = _dapper.LoadData<User>(sql);
        return users;
    }

    //Will pass the int user ID parameter to be searched with
    [HttpGet("GetSingleUser/{userId}")]
    //Returns a single model (user)
    public User GetSingleUser(int userId)
    {
        string sql = @"
            SELECT[UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active]
            FROM TutorialAppSchema.Users
                WHERE UserId = " + userId.ToString();
        User user = _dapper.LoadDataSingle<User>(sql);
        return user;
    }
}



