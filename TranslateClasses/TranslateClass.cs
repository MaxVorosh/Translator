using System;
using System.Text;

namespace TranslateClass;

public class TranslateClass
{

    private StringBuilder _translatedText;
    private int _indent;
    private Dictionary<string, VarType> vars;
    private Dictionary<VarType, string> cTypes;

    public TranslateClass()
    {
        _translatedText = new StringBuilder();
        _indent = 0;
        vars = new Dictionary<string, VarType>();
        cTypes = new Dictionary<VarType, string>();
        FillCTypes();
    }

    public void FillCTypes()
    {
        cTypes[VarType.Int] = "int";
        cTypes[VarType.Float] = "double";
        cTypes[VarType.String] = "string";
    }

    public string TranslateText(string text)
    {
        _translatedText = new StringBuilder();
        _indent = 0;
        WriteMain();
        string[] lines = text.Split("\r\n");
        foreach (var line in lines)
        {
            AddVar(line);
        }

        foreach (var line in lines)
        {
            if (line.Length > 0)
            {
                WriteLine(line);
            }
        }

        _translatedText.Append("}\r\n");
        return _translatedText.ToString();
    }

    public void WriteLine(string line)
    {
        var translated = new StringBuilder();
        for (int i = 0; i < _indent; ++i)
        {
            translated.Append('\t');
        }

        if (IsAssignment(line))
        {
            translated.Append(line);
            translated.Append(';');
        }
        else
        {
            translated.Append("//");
            translated.Append(line);
        }

        _translatedText.Append(translated.ToString());
        _translatedText.Append("\r\n");
    }

    public bool IsAssignment(string line)
    {
        var expression = GetAssignmentExpression(line);
        return IsVar(expression[0]) && GetExpressionType(expression[1]) != VarType.None;
    }

    public string[] GetAssignmentExpression(string line)
    {
        string[] expressions = line.Split('=');
        StringBuilder secondPart = new StringBuilder();
        for (int i = 1; i < expressions.Length; ++i)
        {
            secondPart.Append(expressions[i]);
            if (i != expressions.Length - 1)
            {
                secondPart.Append('=');
            }
        }

        string[] result = { expressions[0], secondPart.ToString() };
        return result;
    }

    public void AddVar(string line)
    {
        var expression = GetAssignmentExpression(line);
        if (expression[1] == String.Empty)
        {
            return;
        }

        if (IsVar(expression[0]))
        {
            if (IsVar(expression[1]))
            {
                if (vars.ContainsKey(expression[1]) && vars[expression[1]] != VarType.None)
                    vars[expression[0]] = vars[expression[1]];
                else
                    return;
            }
            else
            {
                vars[expression[0]] = GetExpressionType(expression[1]);
                if (vars[expression[0]] == VarType.None)
                    return;
            }
        }
        else
            return;

        for (int i = 0; i < _indent; ++i)
        {
            _translatedText.Append('\t');
        }

        _translatedText.Append($"{cTypes[vars[expression[0]]]} {expression[0]};\r\n");
    }

    public VarType GetExpressionType(string expression)
    {
        expression = DeleteSpaces(expression);
        if (expression == String.Empty)
        {
            return VarType.None;
        }
        if (IsExpressionInt(expression))
        {
            return VarType.Int;
        }

        if (IsExpressionFloat(expression))
        {
            return VarType.Float;
        }

        if (IsExpressionString(expression))
        {
            return VarType.String;
        }

        return VarType.None;
    }

    public string DeleteSpaces(string expression)
    {
        var nameParts = expression.Split(' ').Where(x => x.Length >= 1).ToList();
        if (nameParts.Count != 1)
        {
            return String.Empty;
        }

        expression = nameParts[0];
        return expression;
    }

    public bool IsExpressionInt(string expression)
    {
        for (int i = 0; i < expression.Length; ++i)
        {
            if (!(IsDigit(expression[i]) || i == 0 && expression[i] == '-'))
            {
                return false;
            }
        }

        return true;
    }

    public bool IsExpressionString(string expression)
    {
        char c = expression[0];
        return ((c == '"' || c == '\'') && c == expression[^1]);
    }

    public bool IsExpressionFloat(string expression)
    {
        bool isDot = false;
        for (int i = 0; i < expression.Length; ++i)
        {
            if (expression[i] == '.')
            {
                if (isDot)
                {
                    return false;
                }
                isDot = true;
            }
            else if (!(IsDigit(expression[i]) || (i == 0 && expression[i] == '-')))
            {
                return false;
            }
        }
        return true;
    }

    public bool IsVar(string name)
    {
        var nameParts = name.Split(' ').Where(x => x.Length >= 1).ToList();
        if (nameParts.Count != 1)
        {
            return false;
        }
        name = nameParts[0];
        if (!IsLetter(name[0]))
        {
            return false;
        }
        for (int i = 1; i < name.Length; ++i)
        {
            if (!IsLetter(name[i]) && !IsDigit(name[i]))
            {
                return false;
            }
        }
        return true;
    }

    public bool IsLetter(char c)
    {
        return ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z') || (c == '_');
    }

    public bool IsDigit(char c)
    {
        return ('0' <= c && c <= '9');
    }

    public void WriteMain()
    {
        _translatedText.Append("public static void Main(string[] args)\r\n");
        _translatedText.Append("{\r\n");
        _indent++;
    }

    public static void Main(string[] args)
    {
    }
}