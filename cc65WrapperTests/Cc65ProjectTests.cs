using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Shouldly;

namespace cc65Wrapper.Tests
{
    [TestClass()]
    public class Cc65ProjectTests
    {
        [TestInitialize]

        [TestMethod()]
        public void NewProject_WithDefaultSetting_ShouldSucceed()
        {
            // Act ...
            var result = new Cc65Project();

            // Assert ...
            result.ShouldNotBeNull();
            result.WorkingDirectory.ShouldBeNullOrEmpty();
            result.TargetPlatform.ShouldBe("pet");
            result.InputFiles.Count.ShouldBe(0);
            result.OutputFile.ShouldBeNullOrEmpty();
            result.OptimiseCode.ShouldBeFalse();
            result.Version.ShouldBe(Cc65Project.VERSION);
        }

        [TestMethod()]
        public void AsJsonTest_WithUnpopulatedProject_ShouldSucceed()
        {
            // Arrange ...
            var project = new Cc65Project();

            // Act ...
            var result = project.AsJson();

            // Assert ...
            result.ShouldNotBeNullOrEmpty();
            var fromResult = JsonConvert.DeserializeObject<Cc65Project>(result);
            fromResult.WorkingDirectory.ShouldBe(project.WorkingDirectory);
            fromResult.TargetPlatform.ShouldBe(project.TargetPlatform);
            fromResult.InputFiles.ShouldBe(project.InputFiles);
            fromResult.OutputFile.ShouldBe(project.OutputFile);
            fromResult.OptimiseCode.ShouldBe(project.OptimiseCode);
            fromResult.Version.ShouldBe(project.Version);
        }

        [TestMethod]
        public void FullOutputFilePath_ShouldSucceed()
        {
            // Arrange ...
            var project = new Cc65Project();
            project.WorkingDirectory = @"C:\TMP";
            project.OutputFile = @"fred";
            var expected = System.IO.Path.Combine(project.WorkingDirectory, project.OutputFile);

            // Act ...
            var result = project.FullOutputFilePath;

            // Assert ...
            result.ShouldNotBeNullOrEmpty();
            result.ShouldBe(expected);
        }
    }
}