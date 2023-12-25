using MyHttpServer.Attribuets;
using System.Reflection;


namespace MyHttpServer.route;

public class Route
{
    public object Instance { get; set; }
    public string ControllerName { get; set; }
    public string ActionName { get; set; }
    public MethodInfo MethodInfo { get; set; }

    public static List<Route> GetRoutes()
    {
        var routes = new List<Route>();

        var types = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => type.GetCustomAttributes(typeof(ControllerAttribute), true).Length > 0);

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);

            var controllerAttr = (ControllerAttribute)type.GetCustomAttributes(typeof(ControllerAttribute), true)[0];
            var methods = type.GetMethods()
                .Where(method => method.GetCustomAttributes(typeof(GetAttribute), true).Length > 0
                || method.GetCustomAttributes(typeof(PostAttribute), true).Length > 0);

            foreach (var method in methods)
            {
                var attributsa = method.GetCustomAttributes(true);
                var actionAttr = (HttpMethodAttribuet)method.GetCustomAttributes(true).LastOrDefault();
                routes.Add(new Route
                {
                    Instance = instance,
                    ControllerName = controllerAttr.ControllerName,
                    ActionName = actionAttr.ActionName,
                    MethodInfo = method
                });
            }
        }

        return routes;

    }
}


//{
//    public class Route
//    {
//        public string ControllerName { get; set; }
//        public string ActionName { get; set; }
//        public MethodInfo MethodInfo { get; set; }

//        public static List<Route> GetRoutes()
//        {
//            var routes = new List<Route>();
//            var types = Assembly.GetExecutingAssembly().GetTypes()
//                .Where(type => type.GetCustomAttributes(typeof(ControllerAttribute), true).Length > 0);

//            foreach (var type in types)
//            {
//                var controllerAttr = (ControllerAttribute)type.GetCustomAttributes(typeof(ControllerAttribute), true)[0];
//                var methods = type.GetMethods()
//                    .Where(method => method.GetCustomAttributes(typeof(GetAttribute), true).Length > 0);

//                foreach (var method in methods)
//                {
//                    var actionAttr = (GetAttribute)method.GetCustomAttributes(typeof(GetAttribute), true)[0];

//                    routes.Add(new Route
//                    {
//                        ControllerName = controllerAttr.ControllerName,
//                        ActionName = actionAttr.ActionName,
//                        MethodInfo = method
//                    });
//                }
//            }

//            return routes;
//        }
//    }
//}
