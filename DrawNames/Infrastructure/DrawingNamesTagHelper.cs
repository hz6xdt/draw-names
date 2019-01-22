using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using DrawNames.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DrawNames.Infrastructure
{

    [HtmlTargetElement("td", Attributes = "drawing-name")]
    public class DrawingNamesTagHelper : TagHelper
    {
        public DrawNamesDbContext db { get; set; }
        private UserManager<AppUser> userManager;

        public DrawingNamesTagHelper(DrawNamesDbContext db, UserManager<AppUser> userMgr)
        {
            this.db = db;
            userManager = userMgr;
        }

        [HtmlAttributeName("drawing-name")]
        public int Drawing { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            List<string> names = new List<string>();
            List<DrawingUsers> drawingUsers = await db.DrawingUsers.Where(du => du.DrawingId == Drawing).ToListAsync<DrawingUsers>();

            foreach (var drawingUser in drawingUsers)
            {
                AppUser user = await userManager.FindByIdAsync(drawingUser.UserId);
                if (user != null)
                {
                    names.Add(user.UserName);
                }
            }

            output.Content.SetContent(names.Count == 0
                ? "No Names Yet..."
                : string.Join(", ", names));
        }
    }
}
