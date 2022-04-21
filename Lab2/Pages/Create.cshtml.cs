using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab2.Pages
{
    public class CreateModel : PageModel
    {
        private static string Name { get; set; }
        private static List<string> Tactics { get; set; }
        private static string ShortName { get; set; }
        private static string Description { get; set; }
        
        public void OnGet()
        {
            ViewData["ShowVal"] = "none";
        }

        public void OnPostCreate()
        {
            if(!Validate())
            {
                ViewData["ShowVal"] = "normal";
                ViewData["Col"] = "rgba(255, 23, 35, 0.3)";
                return;
            }

            Tactics = new List<string>();


            foreach (var tacticID in JSONTypes.TacticNames.Keys)
            {
                if (Request.Form.ContainsKey(tacticID))
                {
                    var tactic = JSONTypes.TacticNames.Where(x => x.Key == tacticID).Select(y => y.Value.Key).First();
                    JSONTypes.PatternsUsed[tactic].Add(Name);
                    Tactics.Add(tactic);
                    
                }
            }
            JSONTypes.PatternTactics.Add(Name, Tactics);
            JSONTypes.Descriptions.Add(Name, Description);

            ViewData["SuccessMsg"] = "Succesfully created " + Name;
            ViewData["Col"] = "rgba(0, 151, 19, 0.3)";

        }

        private bool Validate()
        {
            Name = Request.Form["pName"].ToString();
            ShortName = Name.ToLower().Replace(" ", "-");
            Description = Request.Form["Description"].ToString();

            var toReturn = true;

            if(string.IsNullOrEmpty(Name))
            {
                ViewData["NameVal"] = "<li>Pattern name is required</li>";
                toReturn = false;
            }

            if(JSONTypes.PatternTactics.Keys.Contains(Name) || JSONTypes.Descriptions.Keys.Contains(Name))
            {
                ViewData["NameVal"] = $"<li>A pattern with the name {Name} already exists</li>";
                toReturn = false;
            }

            if (string.IsNullOrEmpty(Description))
            {
                ViewData["DescrVal"] = "<li>A description is required</li>";
                toReturn = false;
            }

            if(!JSONTypes.TacticNames.Keys.Any(k => Request.Form.ContainsKey(k)))
            {
                ViewData["TacticVal"] = "<li>At least one tactic must be selected</li>";
                toReturn = false;
            }

            return toReturn;
        }
    }
}
