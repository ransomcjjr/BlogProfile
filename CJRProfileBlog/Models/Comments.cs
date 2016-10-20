using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJRProfileBlog.Models
{
    public class Comments
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string AuthorId { get; set; }
        public string Body { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdateReason { get; set; }

        public virtual BlogPost Post { get; set; }
        public virtual ApplicationUser Author { get; set; }

    }
}