using System.Collections;
using System.Linq;
using AdventOfCode.Common;
using AdventOfCode.Common.Models;
using AdventOfCode.Puzzles.Day20;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class Day20Test
    {
        [TestMethod]
        public void GrowBoardTest()
        {
            const bool o = false;
            const bool X = true;

            // Given
            var board = new[,]
            {
                {X, o, o,},
                {o, X, X,},
                {X, X, o,},
            };

            var expectedBoard = new[,]
            {
                {o, o, o, o, o, o, o,},
                {o, o, o, o, o, o, o,},
                {o, o, X, o, o, o, o,},
                {o, o, o, X, X, o, o,},
                {o, o, X, X, o, o, o,},
                {o, o, o, o, o, o, o,},
                {o, o, o, o, o, o, o,},
            };

            var scannerData = new ScannerData(new BitArray(0), board, false);

            var increment = 2;

            // When
            var grownBoard = Day20.GrowBoard(scannerData, increment, out var newWidth, out var newHeight);

            // Then
            Assert.AreEqual(7, newWidth);
            Assert.AreEqual(7, newHeight);

            CollectionAssert.AreEqual(expectedBoard, grownBoard.Input);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void CalculateEnhanceSpotTest(bool outside)
        {
            const bool o = false;
            const bool X = true;

            var midPoint = new Point2D(1, 1);

            // Given
            var board = new[,]
            {
                {X, o, o,},
                {o, X, X,},
                {X, X, o,},
            }.Flip();

            var expected = 0b100_011_110;

            // When
            var spot = Day20.CalculateEnhanceSpot(board, outside, midPoint);

            // Then
            Assert.AreEqual(expected, spot);
        }

        [TestMethod]
        public void CalculateEnhanceSpotTest2()
        {
            const bool o = false;
            const bool X = true;

            var midPoint = new Point2D(1, 1);

            // Given
            var board = new[,]
            {
                {o, o, o,},
                {X, o, o,},
                {o, X, o,},
            }.Flip();

            var expected = 34;

            // When
            var spot = Day20.CalculateEnhanceSpot(board, false, midPoint);

            // Then
            Assert.AreEqual(expected, spot);
        }

        [TestMethod]
        public void GetSurroundingTest()
        {
            // Given
            var point = new Point2D(1, 1);

            var expectedSurrounding = new Point2D[]
            {
                new(0, 0), new(1, 0), new(2, 0),
                new(0, 1), new(1, 1), new(2, 1),
                new(0, 2), new(1, 2), new(2, 2)
            };
            
            // Then

            var surrounding = point.GetSurrounding().ToArray();

            CollectionAssert.AreEqual(expectedSurrounding, surrounding);
        }

        [TestMethod]
        public void ScannerData_EnhanceAlgorithmTest()
        {
            var input = @"..#.#..#####.#.#.#.###.##.....###.##.#..###.####..#####..#....#..#..##..###..######.###...####..#..#####..##..#.#####...##.#.#..#.##..#.#......#.###.######.###.####...#.##.##..#..#..#####.....#.#....###..#.##......#.....#..#..#..##..#...##.######.####.####.#.#...#.......#..#.#.#...####.##.#......#..#...##.#.##..#...##.#.##..###.#......#.#.......#.#.#.####.###.##...#.....####.#..#..#.##.#....##..#.####....##...##..#...#......#.#.......#.......##..####..#...#.#.#...##..#.#..###..#####........#..####......#..#

#..#.
#....
##..#
..#..
..###";

            var scannerData = ScannerData.Parse(input);

            Assert.AreEqual(true, scannerData.EnhanceAlgorithm.Get(31));
            Assert.AreEqual(true, scannerData.EnhanceAlgorithm.Get(32));
            Assert.AreEqual(false, scannerData.EnhanceAlgorithm.Get(33));
            Assert.AreEqual(true, scannerData.EnhanceAlgorithm.Get(34));
            Assert.AreEqual(true, scannerData.EnhanceAlgorithm.Get(35));
            Assert.AreEqual(false, scannerData.EnhanceAlgorithm.Get(36));
            Assert.AreEqual(true, scannerData.EnhanceAlgorithm.Get(37));
            Assert.AreEqual(false, scannerData.EnhanceAlgorithm.Get(38));
        }
    }
}