using System;
using System.Text;

namespace TranslateClass;

public class TranslateClass
{
    /// <summary>
    /// Translate python code to C# code
    /// Recognize next patterns:
    /// var = number (int or float)
    /// var = string
    /// var = list of numbers or strings
    /// var = arithmetic expression
    /// if-elif-else operator
    /// while and for cycles
    /// list.append()
    /// var.find()
    /// </summary>
    private StringBuilder _translatedText;
    private string _textToTranslate;
    private int _indent; // how many tabs line should include before main text
    private Dictionary<string, VarType> vars; // Var type by it's name
    private Dictionary<VarType, string> cTypes; // Var type name by type

    public TranslateClass()
    {
        _textToTranslate = "";
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
        // Main function, that translate all text
        _textToTranslate = text;
        _translatedText = new StringBuilder();
        _indent = 0; 
        vars = new Dictionary<string, VarType>(); // Set default values
        WriteMain();
        string[] lines = text.Split("\r\n");
        foreach (var line in lines)
        {
            try
            {
                AddVar(line);
            }
            catch
            {
                continue;
            }
        } // Declares vars

        foreach (var variable in vars.Keys)
        {
            if (vars[variable] == VarType.List)
            {
                AddList(variable);
            }
        } // Declares lists

        foreach (var line in lines)
        {
            if (line.Length > 0)
            {
                WriteLine(line);
            }
        } // Translate lines

        UpdateIndent("");
        _translatedText.Append("}\r\n");
        return _translatedText.ToString();
    }

    public string UpdateIndent(string line)
    {
        // Add tabs ans close brackets until line will not have good indent. Returns line without tabs
        string oldLine = line;
        line = "";
        int space = 0;
        int tab = 0;
        for (int i = 0; i < oldLine.Length; ++i)
        {
            if (oldLine[i] == ' ' && line == String.Empty)
                space++;
            else if (oldLine[i] == '\t' && line == String.Empty)
                tab++;
            else
                line += oldLine[i];
        }
        tab += space / 4; // count tabs of line
        if (tab + 1 >= _indent)
        {
            _indent = tab + 1;
            return line; // If we should add tab
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
            _translatedText.Append(bracket); // Add lines with close brackets
        }

        return line;
    }

    public void WriteLine(string line)
    {
        // Translate one line
        line = UpdateIndent(line);
        for (int i = 0; i < _indent; ++i)
        {
            _translatedText.Append('\t');
        } // Set indents
        try
        {
            ConvertIfPattern(line); // Tries translate line
        }
        catch (Exception e)
        {
            ConvertToComment(line); // Marked line as a comment
        }
        _translatedText.Append("\r\n");
    }

    public void ConvertIfPattern(string line)
    {
        // Translate one line by match with some pattern
        if (IsInput(line))
            ConvertFromInput(line);
        else if (IsPrint(line))
            ConvertFromPrint(line);
        else if (IsListDefinition(line))
            return;
        else if (IsMethod(line))
            ConvertMethod(line);
        else if (IsAssignment(line))
        {
            _translatedText.Append(line);
            _translatedText.Append(';');
        }
        else if (IsIf(line))
            ConvertFromIf(line);
        else if (IsElse(line))
            ConvertFromElse(line);
        else if (IsElif(line))
            ConvertFromElif(line);
        else if (IsWhile(line))
            ConvertFromWhile(line);
        else if (IsFor(line))
            ConvertFromFor(line);
        else
            ConvertToComment(line);
    }

    public bool IsMethod(string line)
    {
        // Check if line is append or find method
        if (!(line.Split('=').Length == 1 && line.Split('.').Length == 2))
        {
            return false;
        }

        int dotIndex = line.IndexOf('.');
        string variable = line.Substring(0, dotIndex);
        return IsVar(variable);
    }

    public void ConvertMethod(string line)
    {
        // Translate line if it is method
        Dictionary<string, string> methods = new Dictionary<string, string>();
        methods["append"] = "Add";
        methods["find"] = "IndexOf";
        foreach (string method in methods.Keys)
        {
            line = line.Replace(method, methods[method]);
        }

        _translatedText.Append(line);
        _translatedText.Append(';');
    }

    public bool IsListDefinition(string line)
    {
        // Check if line is list definition (a = [...])
        line = DeleteSpaces(line);
        if (line.Split('=').Length != 2)
        {
            return false;
        }
        int equalIndex = line.IndexOf('=');
        string variable = line.Substring(0, equalIndex);
        if (!vars.ContainsKey(variable) || vars[variable] != VarType.List)
        {
            return false;
        }
        string value = line.Substring(equalIndex + 1, line.Length - equalIndex - 1);
        return GetExpressionType(value) == VarType.List;
    }

    public void ConvertToComment(string line)
    {
        // Mark line as a comment
        _translatedText.Append("//");
        _translatedText.Append(line);
    }

    public bool IsPrint(string line)
    {
        // Check if line print something
        return line.Substring(0, 6) == "print(" && line[^1] == ')';
    }

    public void ConvertFromPrint(string line)
    {
        // Translate print
        line = line.Replace("print", "Console.WriteLine");
        _translatedText.Append(line);
        _translatedText.Append(';');
    }

    public bool IsInput(string line)
    {
        // Check if line contains input
        return line.Contains("input()");
    }

    public void ConvertFromInput(string line)
    {
        // Translate line a = input() or a = ToTypeFunction(input())
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
        // Translate line with "if" construction
        line = line.Substring(0, 3) + '(' + line.Substring(3, line.Length - 4) + ')';
        _translatedText.Append(line);
        _translatedText.Append("\r\n");
        AddOpenBracket();
    }

    public void ConvertFromWhile(string line)
    {
        // Translate while-cycle
        line = line.Substring(0, 6) + '(' + line.Substring(6, line.Length - 7) + ')';
        _translatedText.Append(line);
        _translatedText.Append("\r\n");
        AddOpenBracket();
    }

    public bool IsWhile(string line)
    {
        // Check if line is while-cycle
        return line.Length >= 6 && line.Substring(0, 6) == "while " && line[^1] == ':';
    }

    public void ConvertFromFor(string line)
    {
        // Translate line with for-cycle
        var patterns = line.Split(' ').Where(x => x.Length >= 1).ToList();
        if (patterns[3].Length >= 6 && patterns[3].Substring(0, 6) == "range(")
        {
            int a = 0, b = 0, c = 1, currentValue = 0, cnt = 0;
            List<string> rangeList = new List<string>();
            for (int i = 3; i < patterns.Count; ++i)
                rangeList.Add(patterns[i]);
            string range = String.Join(String.Empty, rangeList);
            bool isLastDigit = false;
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
                    switch (cnt)
                    {
                        case 0:
                            b = currentValue;
                            cnt++;
                            break;
                        case 1:
                            a = currentValue;
                            cnt++;
                            break;
                        default:
                            c = currentValue;
                            cnt++;
                            break;
                    }

                    currentValue = 0;
                    isLastDigit = false;
                }
            }
            if (cnt >= 2)
                (a, b) = (b, a);
            line = $"for (int {patterns[1]} = {a}; {patterns[1]} < {b}; {patterns[1]}+={c})";
        }
        else
        {
            if (patterns[3][^1] == ':')
                patterns[3] = patterns[3].Substring(0, patterns[3].Length - 1);
            line = $"foreach (var {patterns[1]} in {patterns[3]})";
        }
        _translatedText.Append(line);
        _translatedText.Append("\r\n");
        AddOpenBracket();
    }

    public bool IsFor(string line)
    {
        // Check if line is for-cycle
        var patterns = line.Split(' ').Where(x => x.Length >= 1).ToList();
        return patterns.Count >= 4 && patterns[0] == "for" && patterns[2] == "in" && line[^1] == ':';
    }

    public bool IsIf(string line)
    {
        // Check if line is "if ...:"
        return (line.Length >= 3 && line[0] == 'i' && line[1] == 'f' && line[2] == ' ' && line[^1] == ':');
    }

    public bool IsElse(string line)
    {
        // Check if line is a else-part of if-construction
        line = DeleteSpaces(line);
        return line == "else:";
    }

    public bool IsElif(string line)
    {
        // Check if line is elif-part of if-construction
        return (line.Length >= 5 && line.Substring(0, 5) == "elif " && line[^1] == ':');
    }

    public void AddOpenBracket()
    {
        // Add line with open bracket (needs after cycles and if)
        for (int i = 0; i < _indent; ++i)
        {
            _translatedText.Append("\t");
        }

        _translatedText.Append("{");
    }

    public void ConvertFromElse(string line)
    {
        // Translate else-line
        line = line.Substring(0, line.Length - 1);
        _translatedText.Append(line);
        _translatedText.Append("\r\n");
        AddOpenBracket();
    }

    public void ConvertFromElif(string line)
    {
        // Translate elif-line
        line = "else if (" + line.Substring(5, line.Length - 6) + ')';
        _translatedText.Append(line);
        _translatedText.Append("\r\n");
        AddOpenBracket();
    }

    public bool IsAssignment(string line)
    {
        // Check if line is algebra expression
        var expression = GetAssignmentExpression(line);
        return IsVar(expression[0]) && GetAlgebraTypeExpression(expression[1]) != VarType.None;
    }

    public string[] GetAssignmentExpression(string line)
    {
        // Returns 2 strings - var name and expression, that equals this var
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
        // Add declaration of var, if line is "var = something"
        var expression = GetAssignmentExpression(line);
        expression[0] = DeleteSpaces(expression[0]);
        expression[1] = DeleteSpaces(expression[1]);
        if (expression[1] == String.Empty || vars.ContainsKey(expression[0]))
            return;
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
        if (vars[expression[0]] == VarType.List)
        {
            return;
        }
        for (int i = 0; i < _indent; ++i)
        {
            _translatedText.Append('\t');
        }
        _translatedText.Append($"{cTypes[vars[expression[0]]]} {expression[0]};\r\n");
    }

    public void AddList(string varName)
    {
        // Add declaration of list with given var name
        for (int i = 0; i < _indent; ++i)
            _translatedText.Append('\t');
        List<string> text = _textToTranslate.Split("\r\n").ToList();
        int nameLength = varName.Length;
        foreach (string line in text)
        {
            var expression = DeleteSpaces(line);
            if (expression.Length >= nameLength + 1 && expression.Substring(0, nameLength + 1) == $"{varName}=")
            {
                string value = expression.Substring(nameLength + 1, expression.Length - nameLength - 1);
                if (IsVar(value))
                {
                    if (vars.ContainsKey(value))
                        _translatedText.Append($"var {varName} = {value};\r\n");
                    return;
                }

                if (value[0] != '[' || value[^1] != ']')
                    return;

                if (value != "[]")
                {
                    value = value.Substring(1, value.Length - 2);
                    List<string> listValues = value.Split(',').ToList();
                    var type = GetExpressionType(listValues[0]);
                    if (type != VarType.None && type != VarType.List)
                        _translatedText.Append($"List<{cTypes[type]}> {varName} = {{{value}}};\r\n");
                    return;
                }
            }
            else if (expression.Length >= nameLength + 8 &&
                     expression.Substring(0, nameLength + 8) == $"{varName}.append(")
            {
                string value = expression.Replace($"{varName}.append(", "");
                value = value.Substring(0, value.Length - 1);
                var type = GetExpressionType(value);
                if (type == VarType.VarName && vars.ContainsKey(value))
                    type = vars[value];
                if (type != VarType.None && type != VarType.List && type != VarType.VarName)
                    _translatedText.Append($"List<{cTypes[type]}> {varName} = new List<{cTypes[type]}>();\r\n");
                return;
            }
        }
    }

    public VarType GetExpressionType(string expression)
    {
        // Match expression (without arithmetic) with types
        expression = DeleteSpaces(expression);
        if (expression == String.Empty)
            return VarType.None;
        if (IsVar(expression))
            return VarType.VarName;
        if (IsExpressionInt(expression))
            return VarType.Int;
        if (IsExpressionFloat(expression))
            return VarType.Float;
        if (IsExpressionList(expression))
            return VarType.List;
        if (IsExpressionString(expression))
            return VarType.String;
        if (IsInput(expression))
            return GetInputExpressionType(expression);

        return VarType.None;
    }

    public VarType GetInputExpressionType(string expression)
    {
        // Returns type of var, that definite with given expression
        if (expression.Substring(0, 3) == "int")
        {
            return VarType.Int;
        }

        if (expression.Substring(0, 5) == "float")
        {
            return VarType.Float;
        }

        if (expression.Substring(0, 4) == "list")
        {
            return VarType.List;
        }

        return VarType.String;
    }

    public VarType GetAlgebraTypeExpression(string expression)
    {
        // Returns type of arithmetic expression
        expression = DeleteSpaces(expression);
        expression += '+';
        char[] operations = { '+', '-', '/', '*', '%' };
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
                            type = vars[currentExpression];
                        else
                            return VarType.None;
                    }

                    if (type == VarType.Float || type == VarType.String || type == VarType.None || type == VarType.List)
                        return type;
                }
            }
            else
                currentExpression += expression[i];
        }
        return VarType.Int;
    }

    public string DeleteSpaces(string expression)
    {
        // returns line that equals expression without whitespaces
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
        // Check if expression is int number
        for (int i = 0; i < expression.Length; ++i)
        {
            if (!(IsDigit(expression[i]) || i == 0 && expression[i] == '-'))
            {
                return false;
            }
        }

        return true;
    }

    public bool IsExpressionList(string expression)
    {
        // Check if expression list
        return expression[0] == '[' && expression[^1] == ']';
    }

    public bool IsExpressionString(string expression)
    {
        // Check if expression string
        char c = expression[0];
        return ((c == '"' || c == '\'') && c == expression[^1]);
    }

    public bool IsExpressionFloat(string expression)
    {
        // check if expression is float number
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
        // Check if given name can be a var name
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
        // Check if given char is letter
        return ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z') || (c == '_');
    }

    public bool IsDigit(char c)
    {
        // Check if given char is a digit
        return ('0' <= c && c <= '9');
    }

    public void WriteMain()
    {
        // Write default Main function
        _translatedText.Append("public static void Main(string[] args)\r\n");
        _translatedText.Append("{\r\n");
        _indent++;
    }

    public static void Main(string[] args)
    {
    }
}