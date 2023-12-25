namespace ORM;

public interface IMyDataContext
{
    public string ConnectionString { get; }
    public void Add<T>(T row) where T : class;
    public void Update<T>(T row) where T : class;
    public void Delete<T>(int id) where T : class;
    public List<T> Select<T>() where T : class;
    public T SelectByID<T>(int id) where T : class;
    public long Count<T>() where T : class;
}