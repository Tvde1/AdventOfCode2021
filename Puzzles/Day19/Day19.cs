using AdventOfCode.Common;
using AdventOfCode.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Puzzles.Day19;

public class Day19 : AdventDay
{
    private const string InputFile = "Day19/day19.txt";

    private const string TestInput = @"--- scanner 0 ---
404,-588,-901
528,-643,409
-838,591,734
390,-675,-793
-537,-823,-458
-485,-357,347
-345,-311,381
-661,-816,-575
-876,649,763
-618,-824,-621
553,345,-567
474,580,667
-447,-329,318
-584,868,-557
544,-627,-890
564,392,-477
455,729,728
-892,524,684
-689,845,-530
423,-701,434
7,-33,-71
630,319,-379
443,580,662
-789,900,-551
459,-707,401

--- scanner 1 ---
686,422,578
605,423,415
515,917,-361
-336,658,858
95,138,22
-476,619,847
-340,-569,-846
567,-361,727
-460,603,-452
669,-402,600
729,430,532
-500,-761,534
-322,571,750
-466,-666,-811
-429,-592,574
-355,545,-477
703,-491,-529
-328,-685,520
413,935,-424
-391,539,-444
586,-435,557
-364,-763,-893
807,-499,-711
755,-354,-619
553,889,-390

--- scanner 2 ---
649,640,665
682,-795,504
-784,533,-524
-644,584,-595
-588,-843,648
-30,6,44
-674,560,763
500,723,-460
609,671,-379
-555,-800,653
-675,-892,-343
697,-426,-610
578,704,681
493,664,-388
-671,-858,530
-667,343,800
571,-461,-707
-138,-166,112
-889,563,-600
646,-828,498
640,759,510
-630,509,768
-681,-892,-333
673,-379,-804
-742,-814,-386
577,-820,562

--- scanner 3 ---
-589,542,597
605,-692,669
-500,565,-823
-660,373,557
-458,-679,-417
-488,449,543
-626,468,-788
338,-750,-386
528,-832,-391
562,-778,733
-938,-730,414
543,643,-506
-524,371,-870
407,773,750
-104,29,83
378,-903,-323
-778,-728,485
426,699,580
-438,-605,-362
-469,-447,-387
509,732,623
647,635,-688
-868,-804,481
614,-800,639
595,780,-596

--- scanner 4 ---
727,592,562
-293,-554,779
441,611,-461
-714,465,-776
-743,427,-804
-660,-479,-426
832,-632,460
927,-485,-438
408,393,-506
466,436,-512
110,16,151
-258,-428,682
-393,719,612
-211,-452,876
808,-476,-593
-575,615,604
-485,667,467
-680,325,-822
-627,-443,-432
872,-547,-609
833,512,582
807,604,487
839,-516,451
891,-625,532
-652,-548,-490
30,-46,-14";

    public Day19()
        : base(19, AdventDayImplementation.Build(AdventDataSource.FromRaw(TestInput), Parse, PartOne))
    { }

    private readonly record struct ScannerData(int ScannerNumber, HashSet<Vector3D> Beacons);

    private static ScannerData[] Parse(string input)
    {
        List<ScannerData> scannerDatas = new();

        ScannerData? currentScanner = null;
        foreach (var line in input.Split(Environment.NewLine))
        {
            if (currentScanner == null)
            {
                currentScanner = new ScannerData(int.Parse(line[12..^3]), new HashSet<Vector3D>());
                continue;
            }

            if (string.IsNullOrEmpty(line))
            {
                scannerDatas.Add(currentScanner.Value);
                currentScanner = null;
                continue;
            }

            currentScanner.Value.Beacons.Add(Vector3D.Parse(line));
        }

        return scannerDatas.ToArray();
    }

    private static string PartOne(ScannerData[] data)
    {
        var scanners = new Dictionary<int, ScannerData> {
            { 0, data.Single(x => x.ScannerNumber == 0) },
        };

        var remainingScanners = new Queue<ScannerData>(data.Where(x => x.ScannerNumber != 0));

        foreach (var mappedScanner in scanners.Values)
        {
            while(remainingScanners.TryDequeue(out ScannerData unmappedScanner))
            {
                var mapping = FindMatchingPoints(mappedScanner, unmappedScanner, 12);

                if (mapping.HasValue)
                {
                throw new NotImplementedException();
                    //var totalTranslation = 

                    //scanners.Add((unmappedScanner, ))
                }
            }
        }

        throw new NotImplementedException();
    }

    private static string PartTwo(string data) => data;

    public static Func<Vector3D, Vector3D>[] Rotations = new[] {
        new Func<Vector3D, Vector3D>(v => new Vector3D(v.X, v.Y, v.Z )),
        new Func<Vector3D, Vector3D>(v => new Vector3D(v.X, v.Z, -v.Y )),
        new Func<Vector3D, Vector3D>(v => new Vector3D(v.X, -v.Y, -v.Z )),
        new Func<Vector3D, Vector3D>(v => new Vector3D(v.X, -v.Z, v.Y )),
        new Func<Vector3D, Vector3D>(v => new Vector3D(v.Y, v.X, -v.Z )),
        new Func<Vector3D, Vector3D>(v => new Vector3D(v.Y, v.Z, v.X )),
        new Func<Vector3D, Vector3D>(v => new Vector3D(v.Y, -v.X, v.Z )),
        new Func<Vector3D, Vector3D>(v => new Vector3D(v.Y, -v.Z, -v.X )),
        new Func<Vector3D, Vector3D>(v => new Vector3D(v.Z, v.X, v.Y )),
        new Func<Vector3D, Vector3D>(v => new Vector3D(v.Z, v.Y, -v.X )),
        new Func<Vector3D, Vector3D>(v => new Vector3D(v.Z, -v.X, -v.Y )),
        new Func<Vector3D, Vector3D>(v => new Vector3D(v.Z, -v.Y, v.X )),
        new Func<Vector3D, Vector3D>(v => new Vector3D( -v.X, v.Y, -v.Z )),
        new Func<Vector3D, Vector3D>(v => new Vector3D( -v.X, v.Z, v.Y )),
        new Func<Vector3D, Vector3D>(v => new Vector3D( -v.X, -v.Y, v.Z )),
        new Func<Vector3D, Vector3D>(v => new Vector3D( -v.X, -v.Z, -v.Y )),
        new Func<Vector3D, Vector3D>(v => new Vector3D( -v.Y, v.X, v.Z )),
        new Func<Vector3D, Vector3D>(v => new Vector3D( -v.Y, v.Z, -v.X )),
        new Func<Vector3D, Vector3D>(v => new Vector3D( -v.Y, -v.X, -v.Z )),
        new Func<Vector3D, Vector3D>(v => new Vector3D( -v.Y, -v.Z, v.X )),
        new Func<Vector3D, Vector3D>(v => new Vector3D( -v.Z, v.X, -v.Y )),
        new Func<Vector3D, Vector3D>(v => new Vector3D( -v.Z, v.Y, v.X )),
        new Func<Vector3D, Vector3D>(v => new Vector3D( -v.Z, -v.X, v.Y )),
        new Func<Vector3D, Vector3D>(v => new Vector3D( -v.Z, -v.Y, -v.X )),
    };

    private static (int FirstScanner, int SecondScanner, Vector3D Translation, Vector3D Rotation)? FindMatchingPoints(ScannerData baseScanner, ScannerData secondScanner, int amountMatch)
    {
        foreach(var rotator in Rotations)
        {
            var baseMappingsToBeacons = baseScanner.Beacons.Select(x => rotator(x));

            if (secondScanner.Beacons.Where(a => baseMappingsToBeacons.Contains(a)).Skip(amountMatch - 1).Any())
            {
                throw new NotImplementedException();
                //var secondScannerToCommonBeacon = Point3D.CalculatePointTranslation(new Point3D(0, 0, 0), secondScannerBeacon);
                //var commonBeaconToFirstScanner = Point3D.CalculatePointTranslation(baseBeacon, new Point3D(0, 0, 0));

            }
        }





        //foreach (var baseBeacon in baseScanner.Beacons)
        //{
        //    var baseTranslations = baseScanner.Beacons.Select(x => Point3D.CalculatePointTranslation(baseBeacon, x)).ToHashSet();

        //    foreach (var secondScannerBeacon in secondScanner.Beacons)
        //    {
        //        foreach (var rotationTranslation in Rotations)
        //        {
        //            var secondScannerBeaconTranslations = secondScanner.Beacons.Select(x => rotationTranslation(Point3D.CalculatePointTranslation(secondScannerBeacon, x)));

        //            var twelfthTranslation = secondScannerBeaconTranslations.Where(x => baseTranslations.Contains(x)).Skip(amountMatch - 1).Any();

        //            if (twelfthTranslation)
        //            {
        //                var scannerToBeacon = Point3D.CalculatePointTranslation(new Point3D(0, 0, 0), baseBeacon);

        //                var secondScannerToCommonBeacon = Point3D.CalculatePointTranslation(new Point3D(0, 0, 0), secondScannerBeacon);
        //                var commonBeaconToFirstScanner = Point3D.CalculatePointTranslation(baseBeacon, new Point3D(0, 0, 0));

        //                var totalTranslation = secondScannerToCommonBeacon + commonBeaconToFirstScanner;

        //                return (baseScanner.ScannerNumber, secondScanner.ScannerNumber, totalTranslation, rotationTranslation);
        //            }
        //        }
        //    }
        //}

        return null;
    }
}