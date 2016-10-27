using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChatterApp.Models
{
    public class Chat
    {
        [Key]
        public int ChatID { get; set; }

        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        [StringLength(150)]
        public string Content { get; set; }
    }
}