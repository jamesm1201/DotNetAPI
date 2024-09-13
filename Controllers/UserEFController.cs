
using DotNetAPI.Data;
using DotNetAPI.Dtos;
using DotNetAPI.Models;
using Microsoft.AspNetCore.Mvc;
namespace DotNetAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class UserEFController : ControllerBase
{
    DataContextEF _entityframework; 
    public UserEFController(IConfiguration config){
        //Constructor to be used later
        _entityframework = new DataContextEF(config);
    }

    [HttpGet("GetUsers")]
    //This will return an IEnumerable of all user models in DB
    public IEnumerable<User> GetUsers()
    {
        /*Less syntax than dapper 
        Works as EF context has linked to users table in config file*/
        IEnumerable<User> users = _entityframework.Users.ToList<User>();
        return users;
    }

    //Will pass the int user ID parameter to be searched with
    [HttpGet("GetSingleUser/{userId}")]
    //Returns a single model (user)
    public User GetSingleUser(int userId)
    {
        /*Nullable in case there is no User with passed ID*/
        User? user = _entityframework.Users
            .Where(u => u.UserId == userId)
            .FirstOrDefault<User>();
        if(user != null){
            return user;
        }
        throw new Exception("Failed to get user.");
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user){
        //Pulls user from DB
        User? userDb = _entityframework.Users
            .Where(u => u.UserId == user.UserId)
            .FirstOrDefault<User>();

        if(userDb != null){
            //Sets DB user fields to ones passed in
            userDb.Active = user.Active;
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Email = user.Email;
            userDb.Gender = user.Gender;

            //If the changes are saved and affect a row then it has worked
            if(_entityframework.SaveChanges() > 0){
            return Ok();
            }
        }
        throw new Exception("Failed to update user.");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user){
        //Makes new User object to give to DB
        User userDb = new User();
        //Sets new user fields to ones passed in
        userDb.Active = user.Active;
        userDb.FirstName = user.FirstName;
        userDb.LastName = user.LastName;
        userDb.Email = user.Email;
        userDb.Gender = user.Gender;
        
        //Adds new user to DB
        _entityframework.Add(userDb);
        //If the changes are saved and affect a row then it has worked
        if(_entityframework.SaveChanges() > 0){
            return Ok();
        }
        throw new Exception("Failed to add user.");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId){
         User? userDb = _entityframework.Users
            .Where(u => u.UserId == userId)
            .FirstOrDefault<User>();

        if(userDb != null){
            //Deletes user that matches the ID
            _entityframework.Users.Remove(userDb);

            //If the changes are saved and affect a row then it has worked
            if(_entityframework.SaveChanges() > 0){
                return Ok();
            }
        }
        throw new Exception("Failed to update user.");
    }
    
}



