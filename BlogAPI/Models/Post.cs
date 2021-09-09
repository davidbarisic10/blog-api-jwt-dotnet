using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlogAPI.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Models
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required] 
        public string Description { get; set; }
        
        // [ForeignKey("UserId")]
        // public virtual ApplicationUser ApplicationUser { get; set; }
        //
        public string UID { get; set; }
    }
}