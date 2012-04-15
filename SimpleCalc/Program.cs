using System;
using System.Collections.Generic;
using System.Text;

abstract class Val
{
    public abstract int Value { get; }
}

class Num : Val
{
    private int num;
    public Num(int n) { num = n; }
    public override int Value { get { return num; } }
}

abstract class Opr : Val
{
    protected Val a, b;
    public Opr(Val a, Val b)
    {
        this.a = a;
        this.b = b;
    }
}

class Add : Opr
{
    public Add(Val a, Val b) : base(a, b) { }
    public override int Value { get { return a.Value + b.Value; } }
}

class Sub : Opr
{
    public Sub(Val a, Val b) : base(a, b) { }
    public override int Value { get { return a.Value - b.Value; } }
}

class Mul : Opr
{
    public Mul(Val a, Val b) : base(a, b) { }
    public override int Value { get { return a.Value * b.Value; } }
}

class Div : Opr
{
    public Div(Val a, Val b) : base(a, b) { }
    public override int Value { get { return a.Value / b.Value; } }
}

class None : Val
{
    public override int Value { get { return 0; } }
}

class Lexer
{
    private string src;
    private int pos;

    private int value;
    public int Value { get { return value; } }

    private string token;
    public string Token { get { return token; } }

    public Lexer(string src)
    {
        this.src = src;
    }

    public Lexer Read()
    {
        while (pos < src.Length)
        {
            var ch = src[pos];
            if (ch == ' ')
                pos++;
            else if ("+-*/()".IndexOf(ch) >= 0)
            {
                token = src.Substring(pos, 1);
                pos++;
                break;
            }
            else if (char.IsNumber(ch))
            {
                var p = pos;
                value = 0;
                while (pos < src.Length && char.IsNumber(src[pos]))
                    value = (value * 10) + (src[pos++] - '0');
                token = src.Substring(p, pos - p);
                break;
            }
            else
            {
                Console.WriteLine("invalid char: {0}", ch);
                token = null;
                break;
            }
        }
        return this;
    }
}

class Calc
{
    // expr = term {('+'|'-') term}*
    static Val Expr(Lexer lexer)
    {
        var ret = Term(lexer);
        while (!(ret is None))
        {
            if (lexer.Token == "+")
                ret = new Add(ret, Term(lexer.Read()));
            else if (lexer.Token == "-")
                ret = new Sub(ret, Term(lexer.Read()));
            else
                break;
        }
        return ret;
    }

    // term = factor {('*'|'/') factor}*
    static Val Term(Lexer lexer)
    {
        var ret = Factor(lexer);
        while (!(ret is None))
        {
            if (lexer.Token == "*")
                ret = new Mul(ret, Factor(lexer.Read()));
            else if (lexer.Token == "/")
                ret = new Div(ret, Factor(lexer.Read()));
            else
                break;
        }
        return ret;
    }

    // factor = '(' expr ')' | nat
    static Val Factor(Lexer lexer)
    {
        if (lexer.Token == "(")
        {
            var ret = Expr(lexer.Read());
            if (lexer.Token != ")")
            {
                Console.WriteLine("required: ')'");
                return null;
            }
            lexer.Read();
            return ret;
        }
        else if (lexer.Token != null)
        {
            var ret = new Num(lexer.Value);
            lexer.Read();
            return ret;
        }
        else
            return new None();
    }

    static void Main()
    {
        string line;
        while ((line = Console.ReadLine()) != null)
        {
            var lexer = new Lexer(line);
            lexer.Read();
            var expr = Expr(lexer);
            if (!(expr is None)) Console.WriteLine(expr.Value);
        }
    }
}
