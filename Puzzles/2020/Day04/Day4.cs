using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace AdventOfCode.Puzzles._2020.Day4;

public class Day4 : AdventDay
{
    private static readonly AdventDataSource RealInput = AdventDataSource.ForThisDay();

    private static readonly AdventDataSource TestInput = AdventDataSource.FromRaw(@"ecl:gry pid:860033327 eyr:2020 hcl:#fffffd
byr:1937 iyr:2017 cid:147 hgt:183cm

iyr:2013 ecl:amb cid:350 eyr:2023 pid:028048884
hcl:#cfa07d byr:1929

hcl:#ae17e1 iyr:2013
eyr:2024
ecl:brn pid:760753108 byr:1931
hgt:179cm

hcl:#cfa07d eyr:2025 pid:166559648
iyr:2011 ecl:brn hgt:59in");

    private static readonly AdventDataSource TestInput2 = AdventDataSource.FromRaw(@"eyr:1972 cid:100
hcl:#18171d ecl:amb hgt:170 pid:186cm iyr:2018 byr:1926

iyr:2019
hcl:#602927 eyr:1967 hgt:170cm
ecl:grn pid:012533040 byr:1946

hcl:dab227 iyr:2012
ecl:brn hgt:182cm pid:021572410 eyr:2020 byr:1992 cid:277

hgt:59cm ecl:zzz
eyr:2038 hcl:74454a iyr:2023
pid:3556412378 byr:2007");

    private static readonly AdventDataSource TestInput3 = AdventDataSource.FromRaw(@"pid:087499704 hgt:74in ecl:grn iyr:2012 eyr:2030 byr:1980
hcl:#623a2f

eyr:2029 ecl:blu cid:129 byr:1989
iyr:2014 pid:896056539 hcl:#a97842 hgt:165cm

hcl:#888785
hgt:164cm byr:2001 iyr:2015 cid:88
pid:545766238 ecl:hzl
eyr:2022

iyr:2010 hgt:158cm hcl:#b6652a ecl:blu byr:1944 eyr:2021 pid:093154719");

    public Day4()
        : base(AdventDayImplementation.Build(RealInput, Parse, PartOne, PartTwo))
    { }

    private static List<Dictionary<string, string>> Parse(string input)
    {
        var passports = new List<Dictionary<string, string>>();
        var currentPassport = new Dictionary<string, string>();

        foreach (var line in input.Split(Environment.NewLine))
        {
            if (string.IsNullOrEmpty(line))
            {
                passports.Add(currentPassport);
                currentPassport = new Dictionary<string, string>();
                continue;
            }

            var values = line.Split(StringConstants.Space).Select(x => x.Split(":"));

            foreach (var value in values)
            {
                currentPassport.Add(value[0], value[1]);
            }
        }

        passports.Add(currentPassport);
        return passports;
    }

    private static readonly Regex HairColourRegex = new Regex(@"\A#[0-9a-f]{6}\Z");
    private static readonly string[] AllowedEyeColours = { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };
    private static readonly Regex PassportIdRegex = new Regex(@"\A\d{9}\Z");

    private static readonly Dictionary<string, Func<string, bool>> MandatoryFields = new()
    {
        { "byr", x => x.Length == 4 && int.TryParse(x, out var year) && year is <= 2002 and >= 1920 },
        { "iyr", x => x.Length == 4 && int.TryParse(x, out var year) && year is <= 2020 and >= 2010 },
        { "eyr", x => x.Length == 4 && int.TryParse(x, out var year) && year is <= 2030 and >= 2020 },
        { "hgt", x => int.TryParse(x[..^2], out var height) && x.Contains("cm", StringComparison.InvariantCultureIgnoreCase) ? height is <= 193 and >= 150 : height is <= 76 and >= 59 },
        { "hcl", x => HairColourRegex.IsMatch(x) },
        { "ecl", x => AllowedEyeColours.Contains(x) },
        { "pid", x => PassportIdRegex.IsMatch(x) }
    };

    private static string PartOne(List<Dictionary<string, string>> data)
    {
        var countCorrect = data.Count(passport => MandatoryFields.Keys.All(passport.ContainsKey));

        return countCorrect.ToString();
    }

    private static string PartTwo(List<Dictionary<string, string>> data)
    {
        var correctPassports = 0;

        foreach (var passport in data)
        {
            var allCorrect = MandatoryFields.All(field =>
            {
                if (!passport.TryGetValue(field.Key, out var value))
                {
                    return false;
                }

                var isValid = field.Value(value);

                return isValid;
            });
            if (allCorrect)
            {
                correctPassports++;
            }
        }

        return correctPassports.ToString();
    }
}