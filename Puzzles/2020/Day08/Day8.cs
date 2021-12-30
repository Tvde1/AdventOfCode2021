using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdventOfCode.Puzzles._2021.Day21;

namespace AdventOfCode.Puzzles._2020.Day08;

public class Day8 : AdventDay
{
    private static readonly AdventDataSource RealInput = AdventDataSource.ForThisDay();

    private static readonly AdventDataSource TestInput = AdventDataSource.FromRaw(@"nop +0
acc +1
jmp +4
acc +3
jmp -3
acc -99
acc +1
jmp -4
acc +6");

    public Day8()
        : base(AdventDayImplementation.Build(RealInput, Parse, PartOne, PartTwo))
    { }

    private static Instruction[] Parse(string input) => input.Split(Environment.NewLine).Select(Instruction.Parse).ToArray();

    private static string PartOne(Instruction[] data)
    {
        var (duplicateLineNo, register) = Computer.RunUntilLoop(data);

        if (!duplicateLineNo.HasValue)
        {
            throw new ArgumentException("Program does not loop.");
        }

        return register.ToString();
    }

    private static string PartTwo(Instruction[] data)
    {
        for (var i = 0; i < data.Length; i++)
        {
            var (instructionType, _) = data[i];
            if (instructionType is not InstructionType.Jmp and not InstructionType.Nop)
            {
                continue;
            }

            var newInstructions = new Instruction[data.Length];
            data.CopyTo(newInstructions, 0);

            newInstructions[i] = newInstructions[i] with
            {
                Type = instructionType == InstructionType.Jmp ? InstructionType.Nop : InstructionType.Jmp
            };

            var (duplicateLineNo, register) = Computer.RunUntilLoop(newInstructions);

            if (duplicateLineNo.HasValue)
            {
                continue;
            }

            return register.ToString();
        }

        return "No operation possible to make the program not loop.";
    }
}

public enum InstructionType
{
    Acc,
    Jmp,
    Nop,
}

public readonly record struct Instruction(InstructionType Type, int Value)
{
    public static Instruction Parse(string input)
    {
        var sp = input.Split(StringConstants.Space);
        return new Instruction(Enum.Parse<InstructionType>(sp[0], true), int.Parse(sp[1]));
    }
}

public class Computer
{
    private static IEnumerable<(int LineNo, int Register)> BuildRunner(IReadOnlyList<Instruction> instructions)
    {
        var register = 0;
        var current = 0;

        while (true)
        {
            yield return (current, register);

            if (current >= instructions.Count)
            {
                yield break;
            }

            var instruction = instructions[current];

            switch (instruction.Type)
            {
                case InstructionType.Acc:
                    register += instruction.Value;
                    current += 1;
                    break;
                case InstructionType.Jmp:
                    current += instruction.Value;
                    break;
                case InstructionType.Nop:
                    current += 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public static (int? DuplicateLineNo, int Register) RunUntilLoop(IReadOnlyList<Instruction> instructions)
    {
        var process = BuildRunner(instructions);

        var visitedLines = new HashSet<int>();

        (int LineNo, int Register) currentStep = (0, 0);

        foreach (var step in process)
        {
            currentStep = step;

            if (!visitedLines.Add(step.LineNo))
            {
                return step;
            }
        }

        return (null, currentStep.Register);
    }
}