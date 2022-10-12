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
        vars = new Dictionary<string, VarType>();
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
        UpdateIndent("");
        _translatedText.Append("}\r\n");
        return _translatedText.ToString();
    }

    public string UpdateIndent(string line)
    {
        string oldLine = line;
        line = "";
        int space = 0;
        int tab = 0;
        for (int i = 0; i < oldLine.Length; ++i)
        {
            if (oldLine[i] == ' ' && line == String.Empty)
            {
                space++;
            }
            else if (oldLine[i] == '\t' && line == String.Empty)
            {
                tab++;
            }
            else
            {
                line += oldLine[i];
            }
        }
        tab += space / 4;
        if (tab + 1 >= _indent)
        {
            _indent = tab + 1;
            return line;
        }

        while (_indent > tab + 1)
        {
            StringBuilder bracket = new StringBuilder();
            for (int i = 0; i < _indent - 1; ++i)
            {
                bracket.Append("\t");
            }
            bracket.Append("}\r\n");
            _indent--;
            _translatedText.Append(bracket);
        }
        return line;
    }

    public void WriteLine(string line)
    {
        var translated = new StringBuilder();
        line = UpdateIndent(line);
        for (int i = 0; i < _indent; ++i)
        {
            _translatedText.Append('\t');
        }

        if (IsInput(line))
        {
            ConvertFromInput(line);
        }
        else if (IsAssignment(line))
        {
            translated.Append(line);
            translated.Append(';');
        }
        else if (IsIf(line))
        {
            ConvertFromIf(line);
        }
        else if (IsElse(line))
        {
            ConvertFromElse(line);
        }
        else if (IsElif(line))
        {
            ConvertFromElif(line);
        }
        else if (IsWhile(line))
        {
            ConvertFromWhile(line);
        }
        else if (IsFor(line))
        {
            ConvertFromFor(line);
        }
        else
        {
            translated.Append("//");
            translated.Append(line);
        }

        _translatedText.Append(translated.ToString());
        _translatedText.Append("\r\n");
    }

    public bool IsInput(string line)
    {
        return line.Contains("input()");
    }

    public void ConvertFromInput(string line)
    {
        if (line.Contains("int"))
        {
            line = line.Replace("int(input())", "Convert.ToInt32(Console.ReadLine());");
        }
        else if (line.Contains("float"))
        {
            line = line.Replace("float(input())", "Convert.ToDouble(Console.ReadLine());");
        }
        else
        {
            line = line.Replace("input()", "Console.ReadLine();");
        }

        _translatedText.Append(line);
    }

    public void ConvertFromIf(string line)
    {
        line = line.Substring(0, 3) + '(' + line.Substring(3, line.Length - 4) + ')';
        _translatedText.Append(line);
        _translatedText.Append("\r\n");
        AddOpenBracket();
    }

    public void ConvertFromWhile(string line)
    {
        line = line.Substring(0, 6) + '(' + line.Substring(6, line.Length - 7) + ')';
        _translatedText.Append(line);
        _translatedText.Append("\r\n");
        AddOpenBracket();
    }

    public bool IsWhile(string line)
    {
        return line.Length >= 6 && line.Substring(0, 6) == "while " && line[^1] == ':';
    }
    
    public void ConvertFromFor(string line)
    {
        var patterns = line.Split(' ').Where(x => x.Length >= 1).ToList();
        if (patterns[3].Length >= 6 && patterns[3].Substring(0, 6) == "range(")
        {
            int a = 0, b = 0, c = 1;
            List<string> rangeList = new List<string>();
            for (int i = 3; i < patterns.Count; ++i)
                rangeList.Add(patterns[i]);
            string range = String.Join(String.Empty, rangeList);
            bool isLastDigit = false;
            int currentValue = 0;
            int cnt = 0;
            for (int i = 0; i < range.Length; ++i)
            {
                if (IsDigit(range[i]))
                {
                    isLastDigit = true;
                    currentValue *= 10;
                    currentValue += range[i] - '0';
                }
                else if (isLastDigit)
                {
                    if (cnt == 0)
                    {
                        b = currentValue;
                        cnt++;
                    }
                    else if (cnt == 1)
                    {
                        a = currentValue;
                        cnt++;
                    }
                    else
                    {
                        c = currentValue;
                        cnt++;
                    }
                    currentValue = 0;
                    isLastDigit = false;
                }
            }
            if (cnt >= 2)
            {
                (a, b) = (b, a);
            }
            line = $"for (int {patterns[1]} = {a}; {patterns[1]} < {b}; {patterns[1]}+={c})";
        }
        else
        {
            if (patterns[3][^1] == ':')
            {
                patterns[3] = patterns[3].Substring(0, patterns[3].Length - 1);
            }
            line = $"foreach (var {patterns[1]} in {patterns[3]})";
        }
        _translatedText.Append(line);
        _translatedText.Append("\r\n");
        AddOpenBracket();
    }

    public bool IsFor(string line)
    {
        var patterns = line.Split(' ').Where(x => x.Length >= 1).ToList();
        return patterns.Count >= 4 && patterns[0] == "for" && patterns[2] == "in" && line[^1] == ':';
    }

    public bool IsIf(string line)
    {
        return (line.Length >= 3 && line[0] == 'i' && line[1] == 'f' && line[2] == ' ' && line[^1] == ':');
    }

    public bool IsElse(string line)
    {
        line = DeleteSpaces(line);
        return line == "else:";
    }

    public bool IsElif(string line)
    {
        return (line.Length >= 5 && line.Substring(0, 5) == "elif " && line[^1] == ':');
    }

    public void AddOpenBracket()
    {
        for (int i = 0; i < _indent; ++i)
        {
            _translatedText.Append("\t");
        }
        _translatedText.Append("{");
    }

    public void ConvertFromElse(string line)
    {
        line = line.Substring(0, line.Length - 1);
        _translatedText.Append(line);
        _translatedText.Append("\r\n");
        AddOpenBracket();
    }
    public void ConvertFromElif(string line)
    {
        line = "else if (" + line.Substring(5, line.Length - 6) + ')';
        _translatedText.Append(line);
        _translatedText.Append("\r\n");
        AddOpenBracket();
    }

    public bool IsAssignment(string line)
    {
        var expression = GetAssignmentExpression(line);
        return IsVar(expression[0]) && GetAlgebraTypeExpression(expression[1]) != VarType.None;
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
        expression[0] = DeleteSpaces(expression[0]);
        expression[1] = DeleteSpaces(expression[1]);
        if (expression[1] == String.Empty || vars.ContainsKey(expression[0]))
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
                vars[expression[0]] = GetAlgebraTypeExpression(expression[1]);
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

        if (IsVar(expression))
        {
            return VarType.VarName;
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

        if (IsInput(expression))
        {
            return GetInputExpressionType(expression);
        }

        return VarType.None;
    }

    public VarType GetInputExpressionType(string expression)
    {
        if (expression.Substring(0, 3) == "int")
        {
            return VarType.Int;
        }

        if (expression.Substring(0, 5) == "float")
        {
            return VarType.Float;
        }

        return VarType.String;
    }

    public VarType GetAlgebraTypeExpression(string expression)
    {
        expression = DeleteSpaces(expression);
        expression += '+';
        char[] operations = { '+', '-', '/', '*', '%'};
        string currentExpression = "";
        for (int i = 0; i < expression.Length; ++i)
        {
            if (operations.Contains(expression[i]))
            {
                if (currentExpression != String.Empty)
                {
                    var type = GetExpressionType(currentExpression);
                    if (type == VarType.VarName)
                    {
                        if (vars.ContainsKey(currentExpression))
                        {
                            type = vars[currentExpression];
                        }
                        else
                        {
                            return VarType.None;
                        }
                    }
                    if (type == VarType.Float || type == VarType.String || type == VarType.None)
                    {
                        return type;
                    }
                }
            }
            else
            {
                currentExpression += expression[i];
            }
        }
        return VarType.Int;
    }

    public string DeleteSpaces(string expression)
    {
        var nameParts = expression.Split(' ').Where(x => x.Length >= 1).ToList();
        if (nameParts.Count == 0)
        {
            return String.Empty;
        }

        expression = String.Join(String.Empty, nameParts);
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