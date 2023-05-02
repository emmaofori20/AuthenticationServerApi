<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;

namespace shared.Entities
{
    public partial class Application
    {
        public Application()
        {
            UserApplications = new HashSet<UserApplication>();
        }

        public int ApplicationId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<UserApplication> UserApplications { get; set; }
    }
}
=======
﻿using System;
using System.Collections.Generic;

namespace shared.Entities
{
    public partial class Application
    {
        public Application()
        {
            UserApplications = new HashSet<UserApplication>();
        }

        public int ApplicationId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<UserApplication> UserApplications { get; set; }
    }
}
>>>>>>> 2720e2f4f567bdcded344254714730085c834078
