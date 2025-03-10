﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// 标签系统
/// </summary>
public class TagSystem : SM<TagSystem>
{
    private static readonly Dictionary<string, Func<string>> tags = new Dictionary<string, Func<string>>()
    {
        { "<mainChar>", () => VNGameSave.activeFile.playerName },
        { "<time>", () => DateTime.Now.ToString("hh:mm tt") },
        { "<playerLevel>", () => "15" },
        { "<input>", () => R.UISystem.UIPlayerInput.lastInput },
    };

    private static readonly Regex tagRegex = new Regex("<\\w+>");

    public static string Inject(string text, bool injectTags = true, bool injectVariables = true)
    {
        if (injectTags)
            text = InjectTags(text);

        if (injectVariables)
            text = InjectVariables(text);

        return text;
    }

    /// <summary>
    /// 注入标签
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static string InjectTags(string value)
    {
        if (tagRegex.IsMatch(value))
        {
            foreach (Match match in tagRegex.Matches(value))
            {
                if (tags.TryGetValue(match.Value, out var tagValueRequest))
                {
                    value = value.Replace(match.Value, tagValueRequest());
                }
            }
        }

        return value;
    }

    /// <summary>
    /// 注入变量
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static string InjectVariables(string value)
    {
        MatchCollection matches = Regex.Matches(value, VariableStore.REGEX_VARIABLE_IDS);
        List<Match> matchesList = matches.Cast<Match>().ToList();

        for (int i = matchesList.Count - 1; i >= 0; i--)
        {
            var match = matchesList[i];
            string variableName = match.Value.TrimStart(VariableStore.VARIABLE_ID, '!');
            bool negate = match.Value.StartsWith('!');

            bool endsInIllegalCharacter = variableName.EndsWith(VariableStore.DATABASE_VARIABLE_RELATIONAL_ID);
            if (endsInIllegalCharacter)
                variableName = variableName.Substring(0, variableName.Length - 1);

            if (!VariableStore.TryGetValue(variableName, out object variableValue))
            {
                UnityEngine.Debug.LogError($"变量 {variableName} 在字符串赋值中未找到");
                continue;
            }

            if (negate && variableValue is bool)
                variableValue = !(bool)variableValue;

            int lengthToBeRemoved = match.Index + match.Length > value.Length ? value.Length - match.Index : match.Length;
            if (endsInIllegalCharacter)
                lengthToBeRemoved -= 1;

            value = value.Remove(match.Index, lengthToBeRemoved);
            value = value.Insert(match.Index, variableValue.ToString());
        }

        return value;
    }
}