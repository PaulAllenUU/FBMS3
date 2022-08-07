using System;
using FBMS3.Core.Models;

namespace FBMS3.Web.ViewModels
{
    public class ClientSearchViewModel
    {
        public IList<Client> Clients { get; set; } = new List<Client>();

        public string Query { get; set; } = "";
    }
}