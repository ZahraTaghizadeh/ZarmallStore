using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ZarmallStore.Application.Extention
{
    public static class CommonExtentions
    {
        public static string GetEnumsName(this System.Enum myEnum )
        {
            var enumDisplayName = myEnum.GetType().GetMember(myEnum.ToString()).FirstOrDefault();
            if (enumDisplayName != null)
            {
                return enumDisplayName.GetCustomAttribute<DisplayAttribute>()?.GetName();
            }
            return "";
        }
    }
}
