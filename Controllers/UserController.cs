
using Microsoft.AspNetCore.Mvc;
namespace DotNetAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class UserController : ControllerBase
{
    public UserController(){
        //Constructor to be used later
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



