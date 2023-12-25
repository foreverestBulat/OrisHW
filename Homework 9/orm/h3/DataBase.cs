using ORM;


namespace MyHttpServer;
public static class Data
{
    public static IMyDataContext Base
        = new MyDataContext("localhost", "postgres", "postgres", "5432", "subuhankulov");
}