using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Shouldly;
using System.IO;

namespace cc65Wrapper.Tests
{
    [TestClass()]
    public class Cc65BuildTests
    {
        [TestMethod()]
        public void Compile_WithValidProject_ShouldSucceed()
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
    }
}