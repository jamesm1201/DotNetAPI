
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

    //Allows for the request to be passed to function using testValue var
    [HttpGet("GetUsers/{testValue}")]
    public string[] GetUsers(string testValue)
    {
        string[] responseArray = new string[]{
            "test1",
            "test2",
            "test3",
            testValue
        };
        return responseArray;
    }
}



