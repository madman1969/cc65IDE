using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.Collections.Generic;
using System.IO;

namespace cc65Wrapper.Tests
{
    [TestClass()]
    public class Cc65BuildTests
    {
        [TestMethod()]
        public void Compile_WithValidSourceFile_ShouldSucceed()
        {
            // Arrange ...
            var inputFiles = new List<string>
            {
                "inflate.c"
            };

            var project = new Cc65Project
            {
                TargetPlatform = "pet",
                OptimiseCode = true,
                WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), @"Test Files"),
                InputFiles = inputFiles,
                OutputFile = "inflate"
            };

            // Act ...
            var result = Cc65Build.Compile(project).Result;

            // Assert ...
            result.ShouldNotBeNull();
            result.ExitCode.ShouldBe(0);
            result.StandardError.ShouldNotBeNullOrEmpty();
        }

        [TestMethod()]
        public void Compile_WithSourceFileWithErrors_ShouldError()
        {
            // Arrange ...
            var inputFiles = new List<string>
            {
                "haserrors.c"
            };

            var project = new Cc65Project
            {
                TargetPlatform = "pet",
                OptimiseCode = true,
                WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), @"Test Files"),
                InputFiles = inputFiles,
                OutputFile = "haserrors"
            };

            // Act ...
            var result = Cc65Build.Compile(project).Result;

            // Assert ...
            result.ShouldNotBeNull();
            result.ExitCode.ShouldBe(1);
            result.StandardError.ShouldNotBeNullOrEmpty();
            result.StandardError.ShouldContain("Error");
            var errorsList = Cc65Build.ErrorsAsList(result);
            errorsList.Count.ShouldBe(4);
        }
    }
}