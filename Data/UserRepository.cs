namespace DotNetAPI.Data;

public class UserRepository : IUserRepository
{
    DataContextEF _entityframework; 
    public UserRepository(IConfiguration config){
        _entityframework = new DataContextEF(config);
    }

    public bool SaveChanges(){
        return _entityframework.SaveChanges() > 0;
    }

    //Can take in any type and add to DB using EF
    public void AddEntity<T>(T entityToAdd){
        if(entityToAdd != null){
            _entityframework.Add(entityToAdd);
        }
    }
    
    public void RemoveEntity<T>(T entityToAdd){
        if(entityToAdd != null){
            _entityframework.Remove(entityToAdd);
        }
    }
}