<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;

namespace shared.Entities
{
    public partial class AspNetRoleClaim
    {
        public int Id { get; set; }
        public string RoleId { get; set; } = null!;
        public string? ClaimType { get; set; }
        public string? ClaimValue { get; set; }

        public virtual AspNetRole Role { get; set; } = null!;
    }
}
=======
﻿using System;
using System.Collections.Generic;

namespace shared.Entities
{
    public partial class AspNetRoleClaim
    {
        public int Id { get; set; }
        public string RoleId { get; set; } = null!;
        public string? ClaimType { get; set; }
        public string? ClaimValue { get; set; }

        public virtual AspNetRole Role { get; set; } = null!;
    }
}
>>>>>>> 2720e2f4f567bdcded344254714730085c834078
