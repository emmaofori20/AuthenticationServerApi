using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.DTOs
{
	public class UserDTO
	{
        public string UserId { get; set; }
        public string username { get; set; }
        public string Email { get; set; }

        public List<string> roles { get; set;}
    }

    public class IsUserAssigned
    {
        public int userApplicationId { get; set; }
        public int ApplicationId { get; set; }
        public string Name { get; set; }
        public bool IsAssigned { get; set; }
    }

    public class UserAssignmentViewModel
    {
        public string username { get; set; }
        public string UserId { get; set; }
        public List<IsUserAssigned> userAssigned { get; set; }

    }
}
