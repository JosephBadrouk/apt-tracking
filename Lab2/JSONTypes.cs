using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections;

namespace Lab2
{
    public class JSONTypes
    {
        public static List<Objects> MatrixObjects { get; set; }
        public static List<string> PhaseNames { get; set; }
        public static Dictionary<string, KeyValuePair<string,string>> TacticNames { get; set; }
        public static Dictionary<string, Patterns> Patterns { get; set; }
        public static Dictionary<string, List<KillChainPhase>> MatrixObjs { get; set; }
        public static Dictionary<string, List<string>> PatternsUsed { get; set; }
        public static Dictionary<string, List<string>> PatternTactics { get; set; }
        public static Dictionary<string, string> Descriptions { get; set; }
        public static MatrixObject obj { get; private set; }
        private static IEnumerable<string> TacticIDs { get; set; }

        public static void LoadJson()
        {
            var jsonText = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "json/attack_matrix.json"));

            obj = JsonConvert.DeserializeObject<MatrixObject>(jsonText);

            var names = obj.Objects.Select(x => x.name).ToList();
            PatternTactics = new Dictionary<string, List<string>>();

            //only one x-mitre-matrix so we can call first()
            TacticIDs = obj.Objects.Where(x => x.Type == "x-mitre-matrix").First().TacticRefs;

            //id -> (name, short name)
            TacticNames = new Dictionary<string, KeyValuePair<string,string>>();

            foreach(var tacticID in TacticIDs)
            {
                var tacticName = obj.Objects.Where(x => x.Id == tacticID && x.Type == "x-mitre-tactic").Select(y => y.name).First();
                var tacticShortName = obj.Objects.Where(x => x.Id == tacticID && x.Type == "x-mitre-tactic").Select(y => y.XMShortName).First();
                var kvp = new KeyValuePair<string, string>(tacticShortName, tacticName);

                TacticNames.Add(tacticID, kvp);
            }

            foreach (var pattern in obj.Objects.Where(x => x.Type == "attack-pattern"))
            {
                //get the tactics used by this pattern
                var patternName = pattern.name;
                List<string> tactics = new List<string>();

                foreach(var t in pattern.kill_chain_phases)
                {
                    tactics.Add(TacticNames.Values.Where(x => x.Key == t.phase_name).Select(x => x.Key).First());
                }

                PatternTactics.Add(patternName, tactics);

            }

            //shortname -> list of tactics
            PatternsUsed = new Dictionary<string, List<string>>();
            foreach (var tacticShortName in TacticNames.Values.Select(x => x.Key))
            {
                PatternsUsed.Add(tacticShortName, 
                    PatternTactics.Where(x => x.Value.Contains(tacticShortName)).Select(y => y.Key).ToList());
            }

            Descriptions = new Dictionary<string, string>();
            foreach (var pattern in PatternTactics.Keys)
            {
                Descriptions.Add(pattern, obj.Objects.Where(x => x.name == pattern).Select(y => y.Description).First());
            }
        }
    }


    public class MatrixObject
    {
        [JsonProperty("objects")]
        public List<Objects> Objects { get;set;}
    }


    public class Objects
    {
        [JsonProperty("kill_chain_phases")]
        public List<KillChainPhase> kill_chain_phases { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("x_mitre_shortname")]
        public string XMShortName { get; set; }

        [JsonProperty("tactic_refs")]
        public List<string> TacticRefs { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class KillChainPhase
    {
        [JsonProperty("phase_name")]
        public string phase_name { get; set; }

        [JsonProperty("kill_chain_name")]
        public string kill_chain_name { get; set; }
    }

    public class Patterns
    {
        public Patterns(string id, string name, List<KillChainPhase> phases)
        {
            Id = id;
            Name = name;
            Phases = phases;
        }

        public string Id;
        public string Name;
        public List<KillChainPhase> Phases;

    }
}
