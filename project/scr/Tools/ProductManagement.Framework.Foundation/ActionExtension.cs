using System;
using System.Linq;

namespace ProductManagement.Framework.Foundation
{
    public static class ActionExtension
    {
        public static string Description(string controller, string action)
        {
            var types =
                from a in AppDomain.CurrentDomain.GetAssemblies()
                from t in a.GetTypes()
                where string.Equals(controller + "Controller", t.Name, StringComparison.OrdinalIgnoreCase)
                select t;

            var controllerType = types.FirstOrDefault();
            if (controllerType == null) return string.Empty;
            System.Reflection.MethodInfo methodInfo = controllerType.GetMethod(action);

            if (methodInfo == null)
            {
                action = action.ToTitleCase();
                methodInfo = controllerType.GetMethod(action);
            }

            if (methodInfo == null) return string.Empty;

            object[] objs = methodInfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (objs == null || objs.Length == 0) return action;
            System.ComponentModel.DescriptionAttribute da = (System.ComponentModel.DescriptionAttribute)objs[0];
            return da.Description;
        }
    }
}
