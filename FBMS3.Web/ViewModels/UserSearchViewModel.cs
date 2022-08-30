using FBMS3.Core.Models;

namespace FBMS3.Web.ViewModels
{
    public class UserSearchViewModel
    {
        public IList<User> Users { get; set; } = new List<User>();

        public string Query { get; set; } = "";
    }
}