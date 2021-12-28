using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AdventOfCode.Common;

namespace AdventOfCode.Puzzles._2021.Day24;

public class Day24 : AdventDay
{
    private static readonly AdventDataSource RealInput = AdventDataSource.FromFile("Day24/day24.txt");

    private static readonly AdventDataSource TestInput1 = AdventDataSource.FromRaw(@"inp x
mul x -1");

    private static readonly AdventDataSource TestInput2 = AdventDataSource.FromRaw(@"inp z
inp x
mul z 3
eql z x");

    private static readonly AdventDataSource TestInput3 = AdventDataSource.FromRaw(@"inp w
add z w
mod z 2
div w 2
add y w
mod y 2
div w 2
add x w
mod x 2
div w 2
mod w 2");

    public Day24()
        : base(24, AdventDayImplementation.Build(RealInput, Parse, PartOne))
    { }

    private static Instruction[] Parse(string input) => input.Split(Environment.NewLine).Select(Instruction.Parse).ToArray();

    private static string PartOne(Instruction[] data)
    {
        var alu = new ALU();

        for (var modelNumber = 99999999999999; modelNumber > 9999999999999; modelNumber--)
        {
            string modelNumberString = modelNumber.ToString();
            if (modelNumberString.Contains('0')) continue;

            Console.WriteLine($"Trying: {modelNumberString}");

            if (Matches(modelNumber))
            {
                return $"Found: {modelNumberString}";
            }
        }

        return "No valid model number found.";
        //return $"W: {result[InstructionVariable.W]}, X: {result[InstructionVariable.X]}, Y: {result[InstructionVariable.Y]}, Z: {result[InstructionVariable.Z]}";
    }

    private static string PartOneCompile(Instruction[] data)
    {
        var expression = InstructionCompiler.CreateExpression(data);

        var compiled = expression.Compile();

        var input = new AluRawInputStream(92793949489995);
        var result = compiled(input);
        if (result == 0)
        {
            return $"Found!";
        }

        return "Nope";
    }

    //private static string PartOneDigits(Instruction[] data)
    //{
    //    var res = Rec(13, 0).ToArray();
    //    return res.OrderByDescending(x => x).First();
    //}

    //private static IEnumerable<string> Rec(int index, int requiredZ)
    //{
    //    if (index == -1) {
    //        yield break;
    //    }

    //    var possibleZs = DigitsThatReachZ(index, requiredZ);

    //    foreach (var possibleZ in possibleZs)
    //    {
    //        foreach(var subPossibilities in Rec(index - 1, possibleZ))
    //        {
    //            yield return $"{subPossibilities}{possibleZ}";
    //        }
    //    }
    //}

    private static string PartTwo(Instruction[] data) => data.ToString();

    private static readonly int[] PossibleDigits = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    private static readonly int[] ZDivs = new[] { 1, 1, 1, 1, 26, 26, 1, 26, 26, 26, 1, 26, 1, 26, };
    private static readonly int[] XPlusses = new[] { 1, 1, 1, 1, 26, 26, 1, 26, 26, 26, 1, 26, 1, 26, };
    private static readonly int[] YPlusses = new[] { 14, 8, 4, 10, 14, 10, 4, 14, 1, 6, 0, 9, 13, 12, };

    private static bool Matches(long input)
    {
        var Z = 0L;

        for (var i = 13; i >= 0; i--) 
        {
            var zDiv = ZDivs[i];
            var xPlus = XPlusses[i];
            var yPlus = YPlusses[i];

            var W = input % 10;
            input /= 10;

            var X = 0L;
            var Y = 25L;

            X += Z;
            X %= 26L;
            Z /= zDiv;
            X += xPlus;
            X = (X != W) ? 1L : 0L;
            Y *= X;
            Y += 1L;
            Z *= Y;

            var tY = W;
            tY += yPlus;
            tY *= X;
            Z += tY;
        }

        return Z == 0;
    }

    private static IEnumerable<(int Index, int Digit, long Z)> CalculatePossibilities(int index, long Z)
    {
        var zDiv = ZDivs[index];
        var xPlus = XPlusses[index];
        var yPlus = YPlusses[index];

        foreach (var possible in PossibleDigits)
        {
            var W = possible;
            var X = 0L;
            var Y = 25L;

            X += Z;
            X %= 26L;
            Z /= zDiv;
            X += xPlus;
            X = (X != W) ? 1L : 0L;
            Y *= X;
            Y += 1L;
            Z *= Y;
            Y = W;
            Y += yPlus;
            Y *= X;
            Z += Y;

            yield return (index, possible, Z);
        }
    }
}

public enum InstructionType
{
    Inp,
    Add,
    Mul,
    Div,
    Mod,
    Eql,
}

public enum InstructionVariable
{
    W,
    X,
    Y,
    Z,
}

public readonly record struct InstructionParameter(InstructionVariable? InstructionVariable, long? Value)
{
    public static InstructionParameter Parse(string input)
    {
        return int.TryParse(input, out var value)
            ? (new(null, value))
            : (new(Enum.Parse<InstructionVariable>(input, true), null));
    }
}

public readonly record struct Instruction
{
    private Instruction(InstructionType type, InstructionVariable variable, InstructionParameter? parameter)
    {
        Type = type;
        Variable = variable;
        Parameter = parameter;
    }

    public InstructionType Type { get; }
    public InstructionVariable Variable { get; }
    public InstructionParameter? Parameter { get; }

    public static Instruction Parse(string input)
    {
        var split = input.Split(' ');
        var type = Enum.Parse<InstructionType>(split[0], true);
        var variable = Enum.Parse<InstructionVariable>(split[1], true);
        InstructionParameter? param = null;
        if (split.Length > 2)
        {
            param = InstructionParameter.Parse(split[2]);
        }

        return new Instruction(type, variable, param);
    }
}

public interface IAluInputStream
{
    public long Read();
}

public class AluRawInputStream : IAluInputStream
{
    private long _source;
    private int _index;

    public AluRawInputStream(long source)
    {
        _source = source;
        _index = source.ToString().Length - 1; // hehe
    }

    public long Read()
    {
        var temp = Math.Floor(_source / Math.Pow(10, _index));
        _index--;
        return Convert.ToByte(temp % 10);
    }
}

public class ALU
{
    public IReadOnlyDictionary<InstructionVariable, long> Execute(Instruction[] instructions, IAluInputStream input)
    {
        var registers = new[] { InstructionVariable.W, InstructionVariable.X, InstructionVariable.Y, InstructionVariable.Z }.ToDictionary(x => x, x => 0L);

        foreach (var instruction in instructions)
        {
            switch (instruction.Type)
            {
                case InstructionType.Inp:
                    registers[instruction.Variable] = input.Read();
                    break;
                case InstructionType.Add:
                    registers[instruction.Variable] += instruction.Parameter!.Value.InstructionVariable.HasValue
                        ? registers[instruction.Parameter!.Value.InstructionVariable.Value]
                        : instruction.Parameter!.Value.Value!.Value;
                    break;
                case InstructionType.Mul:
                    registers[instruction.Variable] *= instruction.Parameter!.Value.InstructionVariable.HasValue
                        ? registers[instruction.Parameter!.Value.InstructionVariable.Value]
                        : instruction.Parameter!.Value.Value!.Value;
                    break;
                case InstructionType.Div:
                    registers[instruction.Variable] /= instruction.Parameter!.Value.InstructionVariable.HasValue
                        ? registers[instruction.Parameter!.Value.InstructionVariable.Value]
                        : instruction.Parameter!.Value.Value!.Value;
                    break;
                case InstructionType.Mod:
                    registers[instruction.Variable] %= instruction.Parameter!.Value.InstructionVariable.HasValue
                        ? registers[instruction.Parameter!.Value.InstructionVariable.Value]
                        : instruction.Parameter!.Value.Value!.Value;
                    break;
                case InstructionType.Eql:
                    registers[instruction.Variable] = registers[instruction.Variable] == (instruction.Parameter!.Value.InstructionVariable.HasValue
                        ? registers[instruction.Parameter!.Value.InstructionVariable.Value]
                        : instruction.Parameter!.Value.Value!.Value) ? 1 : 0;
                    break;
            }
        }

        return registers;
    }
}

public class InstructionCompiler
{
    public static Expression<Func<IAluInputStream, long>> CreateExpression(Instruction[] instructions)
    {
        var variables = Enum.GetValues<InstructionVariable>().ToDictionary(x => x, x => Expression.Variable(typeof(long), x.ToString("G")));

        var expressions = new List<Expression>();

        var param = Expression.Parameter(typeof(IAluInputStream), "inputStream");

        var methodInfo = typeof(IAluInputStream).GetMethod(nameof(IAluInputStream.Read));

        var readnext = Expression.Call(param, methodInfo);

        foreach (var instruction in instructions)
        {
            switch (instruction.Type)
            {
                case InstructionType.Inp:
                    expressions.Add(Expression.Assign(variables[instruction.Variable], readnext));
                    break;
                case InstructionType.Add:
                    {
                        Expression target;
                        if (instruction.Parameter!.Value.Value.HasValue)
                        {
                            target = Expression.Constant(instruction.Parameter.Value.Value.Value);
                        }
                        else
                        {
                            target = variables[instruction.Parameter.Value.InstructionVariable!.Value];
                        }
                        expressions.Add(Expression.AddAssign(variables[instruction.Variable], target));
                        break;
                    }
                case InstructionType.Mul:
                    {
                        Expression target;
                        if (instruction.Parameter!.Value.Value.HasValue)
                        {
                            target = Expression.Constant(instruction.Parameter.Value.Value.Value);
                        }
                        else
                        {
                            target = variables[instruction.Parameter.Value.InstructionVariable!.Value];
                        }
                        expressions.Add(Expression.MultiplyAssign(variables[instruction.Variable], target));
                        break;
                    }
                case InstructionType.Div:
                    {
                        Expression target;
                        if (instruction.Parameter!.Value.Value.HasValue)
                        {
                            target = Expression.Constant(instruction.Parameter.Value.Value.Value);
                        }
                        else
                        {
                            target = variables[instruction.Parameter.Value.InstructionVariable!.Value];
                        }
                        expressions.Add(Expression.DivideAssign(variables[instruction.Variable], target));
                        break;
                    }
                case InstructionType.Mod:
                    {
                        Expression target;
                        if (instruction.Parameter!.Value.Value.HasValue)
                        {
                            target = Expression.Constant(instruction.Parameter.Value.Value.Value);
                        }
                        else
                        {
                            target = variables[instruction.Parameter.Value.InstructionVariable!.Value];
                        }
                        expressions.Add(Expression.ModuloAssign(variables[instruction.Variable], target));
                        break;
                    }
                case InstructionType.Eql:
                    {
                        Expression target;
                        if (instruction.Parameter!.Value.Value.HasValue)
                        {
                            target = Expression.Constant(instruction.Parameter.Value.Value.Value);
                        }
                        else
                        {
                            target = variables[instruction.Parameter.Value.InstructionVariable!.Value];
                        }

                        var equalExpression = Expression.Equal(variables[instruction.Variable], target);

                        var result = Expression.Condition(equalExpression, Expression.Constant(1L), Expression.Constant(0L));

                        expressions.Add(Expression.Assign(variables[instruction.Variable], result));
                        break;
                    }
            }
        }

        var returnTarget = Expression.Label(typeof(long));
        var returnExpression = Expression.Label(returnTarget, variables[InstructionVariable.Z]);

        expressions.Add(returnExpression);

        var block = Expression.Block(variables.Values, expressions);

        var lambda = Expression.Lambda<Func<IAluInputStream, long>>(block, param);

        return lambda;
    }
}