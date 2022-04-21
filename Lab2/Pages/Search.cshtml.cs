using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab2.Pages
{
    public class SearchModel : PageModel
    {
        private static string Name { get; set; }
        private static List<string> Tactics { get; set; }
        private static string ShortName { get; set; }
        private static string Description { get; set; }
        public static List<string> Results { get; private set; }

        public void OnGet()
        {
            Results = new List<string>();
        }

        public ActionResult OnPostSearch()
        {
            Name = Request.Form["pName"].ToString().ToLower();
            ShortName = Name.ToLower().Replace(" ", "-");
            Description = Request.Form["Description"].ToString().ToLower();
            Tactics = new List<string>();

            foreach (var tacticID in JSONTypes.TacticNames.Keys)
            {
                if (Request.Form.ContainsKey(tacticID))
                {
                    var tactic = JSONTypes.TacticNames.Where(x => x.Key == tacticID).Select(y => y.Value.Key).First();
                    Tactics.Add(tactic);

                }
            }


            var nameResults = JSONTypes.PatternTactics.Where(x => x.Key.ToLower().Contains(Name)).Select(y => y.Key).ToList();
            var descriptionResults = JSONTypes.Descriptions.Where(x => x.Value.ToLower().Contains(Description)).Select(y => y.Key).ToList();
            var tacticResults = new List<List<string>>();
            var tacticsToSearch = new List<string>();

            foreach(var tactic in Tactics)
            {
                if (JSONTypes.PatternTactics.Values.Any(x => x.Contains(tactic)))
                {
                    tacticResults.Add(JSONTypes.PatternTactics.Where(x => x.Value.Contains(tactic)).Select(y => y.Key).ToList());
                }
            }

            foreach (var res in tacticResults)
            {
                foreach (var res1 in res)
                {
                    tacticsToSearch.Add(res1);
                }
            }

            Results = nameResults.Intersect(descriptionResults).ToList();

            if(tacticsToSearch.Count > 0)
            {
                Results = Results.Intersect(tacticsToSearch).ToList();
            }

            return RedirectToPage("SearchResults");
        }
    }
}
