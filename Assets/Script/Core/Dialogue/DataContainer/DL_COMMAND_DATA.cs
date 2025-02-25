﻿using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// 命令系统 数据
/// </summary>
public class DL_COMMAND_DATA
{
    public DL_COMMAND_DATA(string rawCommands)
    {
        Commands = RipCommands(rawCommands);
    }

    public List<Command> Commands;
    private const string WAITCOMMAND_ID = "[wait]";
    private const string COMMAND_PATTERN = @"([\w\.|\d+|\[|\]])*\(([^)]*)\),?";

    public struct Command
    {
        public string Name;
        public string[] Arguments;
        /// <summary>
        /// 是否等待完成
        /// </summary>
        public bool WaitForCompletion;
    }

    /// <summary>
    /// 解析Command命令
    /// </summary>
    /// <param name="rawCommands"></param>
    /// <returns></returns>
    private List<Command> RipCommands(string rawCommands)
    {
        MatchCollection data = Regex.Matches(rawCommands, COMMAND_PATTERN);
        List<Command> result = new List<Command>();
        foreach (Match cmd in data)
        {
            Command command = new Command();
            string[] parts = cmd.Value.Split('(');

            command.Name = parts[0].Trim();

            if (command.Name.ToLower().StartsWith(WAITCOMMAND_ID))
            {
                command.Name = command.Name.Substring(WAITCOMMAND_ID.Length);
                command.WaitForCompletion = true;
            }
            else
                command.WaitForCompletion = false;

            string arguments = parts[1].TrimEnd(')', ',');
            command.Arguments = GetArgs(arguments);

            result.Add(command);
        }

        return result;
        // string[] data = rawCommands.Split(COMMANDSPLITTER_ID, System.StringSplitOptions.RemoveEmptyEntries);
        // List<Command> result = new List<Command>();
        // foreach (string cmd in data)
        // {
        //     Command command = new Command();
        //     int index = cmd.IndexOf(ARGUMENTSCONTAINER_ID);
        //     command.Name = cmd.Substring(0, index).Trim();
        //     if (command.Name.ToLower().StartsWith(WAITCOMMAND_ID))
        //     {
        //         command.Name = command.Name.Substring(WAITCOMMAND_ID.Length);
        //         command.WaitForCompletion = true;//是否等待完成执行
        //     }
        //     else
        //     {
        //         command.WaitForCompletion = false;
        //     }
        //
        //     string content = cmd.Substring(index + 1, cmd.Length - index - 2);
        //     command.Arguments = GetArgs(content);
        //     result.Add(command);
        // }
        //
        // return result;
    }

    private string[] GetArgs(string args)
    {
        List<string> argList = new List<string>();
        StringBuilder currentArg = new StringBuilder();
        bool inQuotes = false;//是否在口号内
        for (int i = 0; i < args.Length; i++)
        {
            char content = args[i];
            if (content == '"')//注意这个符号"
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (!inQuotes && content == ' ')
            {
                argList.Add(currentArg.ToString());
                currentArg.Clear();
                continue;
            }

            currentArg.Append(content);
        }

        if (currentArg.Length > 0)
        {
            argList.Add(currentArg.ToString());
        }

        return argList.ToArray();
    }
}