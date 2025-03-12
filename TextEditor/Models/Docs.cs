using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TextEditor.Models
{
	public class Docs
	{
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

		[Required] //if user deleted , its documents deleted also .
        public string? UserId { get; set; }
        [ForeignKey(nameof(UserId))]

		//we get it from Individual account Authentication type .
		public IdentityUser? user { get; set; }
	}
}
