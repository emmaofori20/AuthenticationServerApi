<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;

namespace shared.Entities
{
    public partial class UserApplication
    {
        public int UserApplicationId { get; set; }
        public string UserId { get; set; } = null!;
        public int ApplicationId { get; set; }
        public string UserCredentials { get; set; } = null!;

        public virtual Application Application { get; set; } = null!;
        public virtual AspNetUser User { get; set; } = null!;
    }
}
=======
﻿using System;
using System.Collections.Generic;

namespace shared.Entities
{
    public partial class UserApplication
    {
        public int UserApplicationId { get; set; }
        public string UserId { get; set; } = null!;
        public int ApplicationId { get; set; }
        public string UserCredentials { get; set; } = null!;

        public virtual Application Application { get; set; } = null!;
        public virtual AspNetUser User { get; set; } = null!;
    }
}
>>>>>>> 2720e2f4f567bdcded344254714730085c834078
