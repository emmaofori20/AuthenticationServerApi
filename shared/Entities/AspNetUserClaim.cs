<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;

namespace shared.Entities
{
    public partial class AspNetUserClaim
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string? ClaimType { get; set; }
        public string? ClaimValue { get; set; }

        public virtual AspNetUser User { get; set; } = null!;
    }
}
=======
﻿using System;
using System.Collections.Generic;

namespace shared.Entities
{
    public partial class AspNetUserClaim
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string? ClaimType { get; set; }
        public string? ClaimValue { get; set; }

        public virtual AspNetUser User { get; set; } = null!;
    }
}
>>>>>>> 2720e2f4f567bdcded344254714730085c834078
