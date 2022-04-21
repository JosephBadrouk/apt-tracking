using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab2.Pages
{
    public class TechniqueModel : PageModel
    {
        public static string Name { get; private set; }
        public static string ButtonColor { get; set; }
        private static string ShortName { get; set; }
        private static List<string> TacticShortNames { get; set; }
        public static List<string> Tactics { get; private set; } 
        public static string Description { get; private set; }

        public void OnGet()
        {
            Name = HttpContext.Request.Query["name"].ToString();
            ShortName = HttpContext.Request.Query["tactic"].ToString();
            //Tactic = JSONTypes.TacticNames.Where(x => x.Value.Key == ShortName).Select(y => y.Value.Value).First();

            TacticShortNames = JSONTypes.PatternTactics.Where(x => x.Key == Name).First().Value;

            Tactics = new List<string>();
            foreach(var shortName in TacticShortNames)
            {
                Tactics.Add(JSONTypes.TacticNames.Where(x => x.Value.Key == shortName).Select(y => y.Value.Value).First());
            }


            Description = JSONTypes.Descriptions.Where(x => x.Key == Name).Select(y => y.Value).First();

            if (Track.CheckIfTracked(Name))
            {
                ButtonColor = "w3-red";
                ViewData["TrackStatus"] = "Untrack";
            } else
            {
                ButtonColor = "w3-green";
                ViewData["TrackStatus"] = "Track";
            }            
        }


        public void OnPostTrack()
        {
            if (Track.CheckIfTracked(Name))
            {
                Track.RemoveTrack(Name);
                ViewData["TrackStatus"] = "Track";
                ButtonColor = "w3-green";
            }
            else
            {
                Track.AddTrack(Name);
                ViewData["TrackStatus"] = "Untrack";
                ButtonColor = "w3-red";
            }
        }

    }
}
