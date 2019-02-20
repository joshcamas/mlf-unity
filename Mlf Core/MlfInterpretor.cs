using Microsoft.Scripting.Hosting;
using Microsoft.Scripting;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Ardenfall.Mlf
{

    /// <summary>
    /// Utility for interpreting a snek script, and converting it into a script instance.
    /// </summary>
    public class MlfInterpretor
    {

        //Make code runnable 
        public static string CommentCode(string script)
        {
            script = script.Replace("{{", "#{{");

            return script;
        }

        //Undo commenting
        public static string UnCommentCode(string script)
        {
            script = script.Replace("#{{", "{{");

            return script;
        }

        public static void CleanCode(MlfInstance instance)
        {
            foreach(MlfBlock block in instance.Blocks)
            {
                //Remove #
                block.Content = Regex.Replace(block.Content, "[#].*", "");

                //Remove double newlines
                block.Content = Regex.Replace(block.Content, @"\n\s*\n", "\n");

            }
        }


        public static List<MlfFlag> FindFlags(string script)
        {
            List<MlfFlag> codeFlags = new List<MlfFlag>();

            string pattern = @"\#*\{\{.+\}\}";

            MatchCollection matches = Regex.Matches(script, pattern);

            string[] headers = new string[matches.Count];

            for (int i = 0; i < matches.Count; i++)
                headers[i] = matches[i].Value;

            //Build flags
            for (int i = 0; i < headers.Length; i++)
            {
                Match m = Regex.Match(headers[i], @"\#*\{\{(.+?)\s*(|"".+"")\s*(|\(.+\))\}\}");

                MlfFlag flag = new MlfFlag();

                //ID Required
                if (!m.Groups[1].Success)
                    continue;

                flag.id = m.Groups[1].Value;

                //Tags
                if (m.Groups[2].Success && m.Groups[2].Value.Length != 0)
                    flag.tags = new List<string>(m.Groups[2].Value.Trim('"').Split(','));
                else
                    flag.tags = new List<string>();

                //Arguments
                if (m.Groups[3].Success && m.Groups[3].Value.Length != 0)
                    flag.arguments = new List<string>(m.Groups[3].Value.Trim('(', ')').Split(','));
                else
                    flag.arguments = new List<string>();

                //Trim quotes from arguments
                for(int k=0; k < flag.arguments.Count; k++)
                    flag.arguments[k] = flag.arguments[k].Trim('"', '\'');

                codeFlags.Add(flag);
            }

            return codeFlags;
        }

        public static List<MlfBlock> FindBlocks(MlfInstance instance,string script,string path)
        {
            List<MlfBlock> codeblocks = new List<MlfBlock>();

            //Block header pattern
            string pattern = @"(?:@.+\n|\r)?\[\[.+\]\]";

            MatchCollection matches = Regex.Matches(script, pattern);

            string[] headers = new string[matches.Count];

            for (int i = 0; i < matches.Count; i++)
                headers[i] = matches[i].Value;

            string[] contents = Regex.Split(script, pattern);
            int[] lineCounts = new int[contents.Length];

            //Build line counts
            for (int i=0;i<lineCounts.Length;i++)
                lineCounts[i] = Regex.Matches(contents[i], "\n|\r\n?").Count - 1;
            
            //Build blocks
            for (int i = 0; i < headers.Length; i++)
            {
                Match m = Regex.Match(headers[i], @"(|@.+\n|\r)\[\[(.+?)\s*(|"".+"")\s*(|\(.+\))\]\]");

                int format_group = 1;
                int id_group = 2;
                int tag_group = 3;
                int argument_group = 4;

                MlfBlock block = new MlfBlock(instance);
                block.line = lineCounts[i];
                block.path = path;

                //ID Required
                if (!m.Groups[id_group].Success)
                    continue;

                //ID
                block.id = m.Groups[id_group].Value;

                //Format
                if (m.Groups[format_group].Success) 
                    block.format = m.Groups[format_group].Value.TrimStart('@').Trim();

                //Tags
                if (m.Groups[tag_group].Success && m.Groups[tag_group].Value.Length != 0)
                    block.tags = new List<string>(m.Groups[tag_group].Value.Trim('"').Split(','));
                else
                    block.tags = new List<string>();

                //Arguments
                if (m.Groups[argument_group].Success && m.Groups[argument_group].Value.Length != 0)
                    block.arguments = new List<string>(m.Groups[argument_group].Value.Trim('(', ')').Split(','));
                else
                    block.arguments = new List<string>();

                //Expression
                if (contents.Length > i + 1)
                    block.Content = contents[i+1];

                block.path = path;
                codeblocks.Add(block);
            }
            
            //Find "root code" (Code without a block) and implement it into all blocks.
            if(codeblocks.Count > 1)
            {
                foreach (MlfBlock block in codeblocks)
                {
                    block.AddPrefixText(contents[0]);
                }
            }
            
            return codeblocks;
        }

    }

}
