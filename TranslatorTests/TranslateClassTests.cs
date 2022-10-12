using TranslateClass;
using static NUnit.Framework.Assert;

namespace TranslatorTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void IsVarTest()
    {
        var translator = new TranslateClass.TranslateClass();
        string var1 = "";
        string var2 = "239";
        string var3 = "_b";
        string var4 = "abacaba";
        string var5 = "12chairs";
        string var6 = "chairs_12";
        string var7 = "b;a";
        That(translator.IsVar(var1), Is.False);
        That(translator.IsVar(var2), Is.False);
        That(translator.IsVar(var3), Is.True);
        That(translator.IsVar(var4), Is.True);
        That(translator.IsVar(var5), Is.False);
        That(translator.IsVar(var6), Is.True);
        That(translator.IsVar(var7), Is.False);
    }

    [Test]
    public void IsExpressionFloatTest()
    {
        var translator = new TranslateClass.TranslateClass();
        string var1 = "1.";
        string var2 = "12.01";
        string var3 = "32";
        string var4 = "331.12.12";
        string var5 = "a.1";
        That(translator.IsExpressionFloat(var1), Is.False);
        That(translator.IsExpressionFloat(var2), Is.True);
        That(translator.IsExpressionFloat(var3), Is.True);
        That(translator.IsExpressionFloat(var4), Is.False);
        That(translator.IsExpressionFloat(var5), Is.False);
    }

    [Test]
    public void IsExpressionStringTest()
    {
        var translator = new TranslateClass.TranslateClass();
        string var1 = "''";
        string var2 = "'ababa17'";
        string var3 = "'";
        string var4 = "\"cake\"";
        string var5 = "12";
        string var6 = "32''";
        string var7 = "''aba";
        That(translator.IsExpressionString(var1), Is.True);
        That(translator.IsExpressionString(var2), Is.True);
        That(translator.IsExpressionString(var3), Is.False);
        That(translator.IsExpressionString(var4), Is.True);
        That(translator.IsExpressionString(var5), Is.False);
        That(translator.IsExpressionString(var6), Is.False);
        That(translator.IsExpressionString(var7), Is.False);
    }

    [Test]
    public void IsExpressionListTest()
    {
        var translator = new TranslateClass.TranslateClass();
        string var1 = "[]";
        string var2 = "[12, 12]";
        string var3 = "[a, d, 32]";
        string var4 = "]";
        string var5 = "[x, y";
        That(translator.IsExpressionList(var1), Is.True);
        That(translator.IsExpressionList(var2), Is.True);
        That(translator.IsExpressionList(var3), Is.True);
        That(translator.IsExpressionList(var4), Is.False);
        That(translator.IsExpressionList(var5), Is.False);
    }

    [Test]
    public void IsExpressionIntTest()
    {
        var translator = new TranslateClass.TranslateClass();
        string var1 = "";
        string var2 = "-12";
        string var3 = "132123";
        string var4 = "32.32";
        string var5 = "ababa";
        That(translator.IsExpressionInt(var1), Is.False);
        That(translator.IsExpressionInt(var2), Is.True);
        That(translator.IsExpressionInt(var3), Is.True);
        That(translator.IsExpressionInt(var4), Is.False);
        That(translator.IsExpressionInt(var5), Is.False);
    }

    [Test]
    public void DeleteSpacesTest()
    {
        var translator = new TranslateClass.TranslateClass();
        string var1 = "12  - 7986     + 23";
        string var2 = "";
        string var3 = "b = input()   ";
        That(translator.DeleteSpaces(var1), Is.EqualTo("12-7986+23"));
        That(translator.DeleteSpaces(var2), Is.EqualTo(""));
        That(translator.DeleteSpaces(var3), Is.EqualTo("b=input()"));
    }

    [Test]
    public void GetAlgebraTypeExpressionTest()
    {
        var translator = new TranslateClass.TranslateClass();
        string var1 = "a";
        string var2 = "12";
        string var3 = "'a'";
        string var4 = "12.12";
        string var5 = "[1, 2, 3]";
        string var6 = "[1, 2] * 4";
        string var7 = "'x3' * 3";
        string var8 = "3 + 12.12 * 10";
        string var9 = "6 - 6 * 6";
        string var10 = "var;";
        string var11 = "3 * 'x3'";
        string var12 = "12.12 * [12]";
        That(translator.GetAlgebraTypeExpression(var1), Is.EqualTo(VarType.None));
        That(translator.GetAlgebraTypeExpression(var2), Is.EqualTo(VarType.Int));
        That(translator.GetAlgebraTypeExpression(var3), Is.EqualTo(VarType.String));
        That(translator.GetAlgebraTypeExpression(var4), Is.EqualTo(VarType.Float));
        That(translator.GetAlgebraTypeExpression(var5), Is.EqualTo(VarType.List));
        That(translator.GetAlgebraTypeExpression(var6), Is.EqualTo(VarType.List));
        That(translator.GetAlgebraTypeExpression(var7), Is.EqualTo(VarType.String));
        That(translator.GetAlgebraTypeExpression(var8), Is.EqualTo(VarType.Float));
        That(translator.GetAlgebraTypeExpression(var9), Is.EqualTo(VarType.Int));
        That(translator.GetAlgebraTypeExpression(var10), Is.EqualTo(VarType.None));
        That(translator.GetAlgebraTypeExpression(var11), Is.EqualTo(VarType.String));
        That(translator.GetAlgebraTypeExpression(var12), Is.EqualTo(VarType.Float));
    }

    [Test]
    public void GetInputExpressionTypeTest()
    {
        var translator = new TranslateClass.TranslateClass();
        string var1 = "input()";
        string var2 = "int(input())";
        string var3 = "float(input())";
        string var4 = "string(input())";
        That(translator.GetInputExpressionType(var1), Is.EqualTo(VarType.String));
        That(translator.GetInputExpressionType(var2), Is.EqualTo(VarType.Int));
        That(translator.GetInputExpressionType(var3), Is.EqualTo(VarType.Float));
        That(translator.GetInputExpressionType(var4), Is.EqualTo(VarType.String));
    }

    [Test]
    public void GetExpressionType()
    {
        var translator = new TranslateClass.TranslateClass();
        string var1 = "12";
        string var2 = "k";
        string var3 = "32.32";
        string var4 = "[1, 2, 3]";
        string var5 = "'string'";
        string var6 = "][";
        string var7 = "int(input())";
        That(translator.GetExpressionType(var1), Is.EqualTo(VarType.Int));
        That(translator.GetExpressionType(var2), Is.EqualTo(VarType.VarName));
        That(translator.GetExpressionType(var3), Is.EqualTo(VarType.Float));
        That(translator.GetExpressionType(var4), Is.EqualTo(VarType.List));
        That(translator.GetExpressionType(var5), Is.EqualTo(VarType.String));
        That(translator.GetExpressionType(var6), Is.EqualTo(VarType.None));
        That(translator.GetExpressionType(var7), Is.EqualTo(VarType.Int));
    }

    [Test]
    public void AddListTestNotEmpty()
    {
        var translator = new TranslateClass.TranslateClass();
        string[] vars = { "a = [1, 2, 4]", "b = [\"\"]", "c = [12.12, 123]", "d = [kop]" };
        string[] ans = { "List<int> a = {1,2,4};", "List<string> b = {\"\"};", "List<double> c = {12.12,123};" };
        for (int i = 0; i < 3; ++i)
        {
            translator.SetText(vars[i]);
            char name = Convert.ToChar(i + 'a');
            translator.AddList(name.ToString());
            That(translator.GetText().Split("\r\n")[0], Is.EqualTo(ans[i]));
            translator.ClearText();
        }

        translator.AddList(vars[3]);
        That(translator.GetText().Split("\r\n")[0], Is.EqualTo(String.Empty));
    }

    [Test]
    public void AddListByAppendTest()
    {
        var translator = new TranslateClass.TranslateClass();
        translator.SetText("a = []\r\na.append(1)");
        translator.AddList("a");
        That(translator.GetText().Split("\r\n")[0], Is.EqualTo("List<int> a = new List<int>();"));
    }

    [Test]
    public void AddVarTest()
    {
        var translator = new TranslateClass.TranslateClass();
        string[] vars = { "a = 12", "b = \"cake\"", "c = 32.32", "d = 12k" };
        string[] ans = { "int a;", "string b;", "double c;", "" };
        for (int i = 0; i < 4; ++i)
        {
            translator.AddVar(vars[i]);
            That(translator.GetText().Split("\r\n")[0], Is.EqualTo(ans[i]));
            translator.ClearText();
        }

        translator.AddVar("x = 12");
        translator.AddVar("y = a");
        That(translator.GetText().Split("\r\n")[1], Is.EqualTo("int y;"));
    }

    [Test]
    public void GetAssignmentExpressionTest()
    {
        var translator = new TranslateClass.TranslateClass();
        string var1 = "a.append(1)";
        string var2 = "a = 12 + 32";
        string var3 = "a = d = (b == c)";
        That(translator.GetAssignmentExpression(var1)[0], Is.EqualTo("a.append(1)"));
        That(translator.GetAssignmentExpression(var1)[1], Is.EqualTo(""));
        That(translator.GetAssignmentExpression(var1).Length, Is.EqualTo(2));
        That(translator.GetAssignmentExpression(var2)[0], Is.EqualTo("a "));
        That(translator.GetAssignmentExpression(var2)[1], Is.EqualTo(" 12 + 32"));
        That(translator.GetAssignmentExpression(var2).Length, Is.EqualTo(2));
        That(translator.GetAssignmentExpression(var3)[0], Is.EqualTo("a "));
        That(translator.GetAssignmentExpression(var3)[1], Is.EqualTo(" d = (b == c)"));
        That(translator.GetAssignmentExpression(var3).Length, Is.EqualTo(2));
    }

    [Test]
    public void IsAssignmentTest()
    {
        var translator = new TranslateClass.TranslateClass();
        translator.AddVar("a = 12");
        string var1 = "a = 12";
        string var2 = "a = a + 1";
        string var3 = "a = ]";
        string var4 = "a.append(1)";
        That(translator.IsAssignment(var1), Is.True);
        That(translator.IsAssignment(var2), Is.True);
        That(translator.IsAssignment(var3), Is.False);
        That(translator.IsAssignment(var4), Is.False);
    }

    [Test]
    public void ConvertFromElifTest()
    {
        var translator = new TranslateClass.TranslateClass();
        translator.ConvertFromElif("elif a == 2:");
        That(translator.GetText(), Is.EqualTo("else if (a == 2)\r\n{"));
    }

    [Test]
    public void ConvertFromElseTest()
    {
        var translator = new TranslateClass.TranslateClass();
        translator.ConvertFromElse("else:");
        That(translator.GetText(), Is.EqualTo("else\r\n{"));
    }

    [Test]
    public void IsElifTest()
    {
        var translator = new TranslateClass.TranslateClass();
        string var1 = "elif a == 5:";
        string var2 = "elif:";
        string var3 = "else if";
        That(translator.IsElif(var1), Is.True);
        That(translator.IsElif(var2), Is.False);
        That(translator.IsElif(var3), Is.False);
    }

    [Test]
    public void IsElseTest()
    {
        var translator = new TranslateClass.TranslateClass();
        string var1 = "else:";
        string var2 = "elif a == 1:";
        That(translator.IsElse(var1), Is.True);
        That(translator.IsElse(var2), Is.False);
    }

    [Test]
    public void IsIfTest()
    {
        var translator = new TranslateClass.TranslateClass();
        string var1 = "if a == 5:";
        string var2 = "elif a == 1:";
        string var3 = "if";
        That(translator.IsIf(var1), Is.True);
        That(translator.IsIf(var2), Is.False);
        That(translator.IsIf(var3), Is.False);
    }

    [Test]
    public void IsForTest()
    {
        var translator = new TranslateClass.TranslateClass();
        string var1 = "for i in range(12):";
        string var2 = "for i in data:";
        string var3 = "for i in data";
        string var4 = "while True";
        That(translator.IsFor(var1), Is.True);
        That(translator.IsFor(var2), Is.True);
        That(translator.IsFor(var3), Is.False);
        That(translator.IsFor(var4), Is.False);
    }

    [Test]
    public void ConvertFromForTest()
    {
        var translator = new TranslateClass.TranslateClass();
        string[] vars =
            { "for i in range(12):", "for i in data:", "for i in range(1, 12):", "for i in range(1, 12, 2):" };
        string[] ans =
        {
            "for (int i = 0; i < 12; i+=1)\r\n{", "foreach (var i in data)\r\n{", "for (int i = 1; i < 12; i+=1)\r\n{",
            "for (int i = 1; i < 12; i+=2)\r\n{"
        };
        for (int i = 0; i < 4; ++i)
        {
            translator.ConvertFromFor(vars[i]);
            That(translator.GetText(), Is.EqualTo(ans[i]));
            translator.ClearText();
        }
    }

    [Test]
    public void IsWhileTest()
    {
        var translator = new TranslateClass.TranslateClass();
        string var1 = "while True:";
        string var2 = "while a == 0";
        string var3 = "wh = ile";
        string var4 = "\"while\"";
        That(translator.IsWhile(var1), Is.True);
        That(translator.IsWhile(var2), Is.False);
        That(translator.IsWhile(var3), Is.False);
        That(translator.IsWhile(var4), Is.False);
    }

    [Test]
    public void ConvertFromWhileTest()
    {
        var translator = new TranslateClass.TranslateClass();
        translator.ConvertFromWhile("while a == b:");
        That(translator.GetText(), Is.EqualTo("while (a == b)\r\n{"));
    }

    [Test]
    public void ConvertFromIfTest()
    {
        var translator = new TranslateClass.TranslateClass();
        translator.ConvertFromIf("if a == b:");
        That(translator.GetText(), Is.EqualTo("if (a == b)\r\n{"));
    }

    [Test]
    public void ConvertFromInputTest()
    {
        var translator = new TranslateClass.TranslateClass();
        string[] vars = { "a = input()", "a = int(input())", "a = string(input())", "a = float(input())" };
        string[] ans =
        {
            "a = Console.ReadLine();", "a = Convert.ToInt32(Console.ReadLine());", "a = Console.ReadLine();",
            "a = Convert.ToDouble(Console.ReadLine());"
        };
        for (int i = 0; i < 4; ++i)
        {
            translator.ConvertFromInput(vars[i]);
            That(translator.GetText(), Is.EqualTo(ans[i]));
            translator.ClearText();
        }
    }

    [Test]
    public void IsInputTest()
    {
        var translator = new TranslateClass.TranslateClass();
        That(translator.IsInput("a = input()"), Is.True);
        That(translator.IsInput("a = \"input\""), Is.False);
    }

    [Test]
    public void ConvertFromPrintTest()
    {
        var translator = new TranslateClass.TranslateClass();
        translator.ConvertFromPrint("print(11)");
        That(translator.GetText(), Is.EqualTo("Console.WriteLine(11);"));
    }

    [Test]
    public void IsPrintTest()
    {
        var translator = new TranslateClass.TranslateClass();
        That(translator.IsPrint("print(11)"), Is.True);
        That(translator.IsPrint("a = \"print(11)\""), Is.False);
    }

    [Test]
    public void ConvertToCommentTest()
    {
        var translator = new TranslateClass.TranslateClass();
        string var = "var translator = new TranslateClass.TranslateClass();";
        translator.ConvertToComment(var);
        string ans = "//var translator = new TranslateClass.TranslateClass();";
        That(translator.GetText(), Is.EqualTo(ans));
    }

    [Test]
    public void IsListDefinitionTest()
    {
        var translator = new TranslateClass.TranslateClass();
        string var1 = "a = []";
        string var2 = "a = [1, 2, 3]";
        string var3 = "a = \"[1, 2, 3]\"";
        translator.AddVar("a = []");
        That(translator.IsListDefinition(var1), Is.True);
        That(translator.IsListDefinition(var2), Is.True);
        That(translator.IsListDefinition(var3), Is.False);
    }

    [Test]
    public void ConvertMethodTest()
    {
        var translator = new TranslateClass.TranslateClass();
        translator.ConvertMethod("a.append(11)");
        That(translator.GetText(), Is.EqualTo("a.Add(11);"));
        translator.ClearText();
        translator.ConvertMethod("s.find(x)");
        That(translator.GetText(), Is.EqualTo("s.IndexOf(x);"));
    }

    [Test]
    public void IsMethodTest()
    {
        var translator = new TranslateClass.TranslateClass();
        translator.AddVar("a = []");
        string var1 = "a.append(11)";
        string var2 = "a.find(11)";
        string var3 = "s = \"a.append(11)\"";
        That(translator.IsMethod(var1), Is.True);
        That(translator.IsMethod(var2), Is.True);
        That(translator.IsMethod(var3), Is.False);
    }

    [Test]
    public void ConvertIfPatternTest()
    {
        var translator = new TranslateClass.TranslateClass();
        translator.AddVar("b = [12]");
        string[] vars =
        {
            "a = 12", "c = input()", "a = 18 + 11", "if a == b[0]:", "elif a - b[0] == 1:",
            "else:", "while a != b[0]:", "for i in range(12):", "for i in b:", "not_available"
        };
        string[] ans =
        {
            "a = 12;", "c = Console.ReadLine();", "a = 18 + 11;", "if (a == b[0])\r\n{",
            "else if (a - b[0] == 1)\r\n{", "else\r\n{", "while (a != b[0])\r\n{", "for (int i = 0; i < 12; i+=1)\r\n{",
            "foreach (var i in b)\r\n{", "//not_available"
        };
        translator.ConvertIfPattern("b.append(12)");
        That(translator.GetText(), Is.EqualTo("b.Add(12);"));
        for (int i = 0; i < vars.Length; ++i)
        {
            var translator1 = new TranslateClass.TranslateClass();
            translator1.ConvertIfPattern(vars[i]);
            That(translator1.GetText(), Is.EqualTo(ans[i]));
        }
    }

    [Test]
    public void TranslateTextTest()
    {
        var translator = new TranslateClass.TranslateClass();
        string text =
            "a = int(input())\r\nb = []\r\nfor i in range(a):\r\n\tx = int(input())\r\n\tb.append(x)\r\ns = 0\r\nfor i in b:\r\n\ts = s + 1\r\nprint(s)";
        string[] ansStrings =
        {
            "public static void Main(string[] args)", "{", "\tint a;", "\tint x;", "\tint s;",
            "\tList<int> b = new List<int>();", "\ta = Convert.ToInt32(Console.ReadLine());", "\t",
            "\tfor (int i = 0; i < a; i+=1)", "\t{", "\t\tx = Convert.ToInt32(Console.ReadLine());", "\t\tb.Add(x);",
            "\t}", "\ts = 0;", "\tforeach (var i in b)", "\t{", "\t\ts = s + 1;", "\t}", "\tConsole.WriteLine(s);",
            "}\r\n"
        };
        string ans = String.Join("\r\n", ansStrings);
        char c = ans[136];
        That(translator.TranslateText(text), Is.EqualTo(ans));
    }
}