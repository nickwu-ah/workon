using System;
using System.Collections.Generic;
using System.IO;

namespace workon
{
    // NOTE: must run this from Administrator command line.
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length != 2)
            {
                Console.WriteLine("Usage: workon.exe <path-to-.aws> <profile>\n\teg. > workon.exe C:\\Users\\drewc\\.aws default");
                return;
            }

            var path = $"{args[0]}\\credentials";
            if (!File.Exists(path))
            {
                Console.WriteLine($"Couldn't find credentials in path: {args[0]}");
                return;
            }

            var section = $"[{args[1]}]";

            var envs = new Dictionary<string, string>();
            var envMap = new Dictionary<string, string>
            {
                ["region"] = "AWS_Region",
                ["aws_access_key_id"] = "AWS_ACCESS_KEY_ID",
                ["aws_secret_access_key"] = "AWS_SECRET_ACCESS_KEY",
                ["aws_session_token"] = "AWS_SESSION_TOKEN"
            };
            
            var lines = File.ReadAllLines(path);
            bool hasFoundSection = false;
            foreach (var line in lines)
            {
                if (line.Trim() == section)
                {
                    hasFoundSection = true;
                    continue;
                }
                
                if (hasFoundSection)
                {
                    // This means we're at the next section after the one we want
                    if (line.StartsWith("["))
                    {
                        break;
                    }

                    var parts = line.Split("=");
                    var key = parts[0].Trim();

                    if (envMap.ContainsKey(key))
                    {
                        key = envMap[key];
                        var value = parts[1].Trim();

                        envs[key] = value;
                    }
                }
            }
            
            foreach (var kv in envs)
            {
                Environment.SetEnvironmentVariable(kv.Key, kv.Value, EnvironmentVariableTarget.Machine);
                Console.WriteLine($"Profile: {section}\nVariable: {kv.Key}\nValue: {kv.Value}\n\n");
            }

            Console.WriteLine("Done.");
        }
    }
}
