using cc65Wrapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Shouldly;
using System.IO;

namespace cc65Wrapper.Tests
{
    [TestClass()]
    public class Cc65EmulatorTests
    {
        [TestMethod()]
        public void NewInstance_WithDefaults_ShouldSucceed()
        {
            // Act ...
            var result = new Cc65Emulators();

            // Assert ...
            result.ShouldNotBeNull();
            result.c64Path.ShouldBeEmpty();
            result.c128Path.ShouldBeEmpty();
            result.petPath.ShouldBeEmpty(); 
        }

        [TestMethod()]
        public void AsJson_WithUnpopulatedInstanced_ShouldSucceed()
        {
            // Arrange ...
            var emulator = new Cc65Emulators();

            // Act ...
            var result = emulator.AsJson();

            // Assert ...
            result.ShouldNotBeNullOrEmpty();
            var fromResult = JsonConvert.DeserializeObject<Cc65Emulators>(result);
            fromResult.c64Path.ShouldBe(emulator.c64Path);
            fromResult.c128Path.ShouldBe(emulator.c128Path);    
            fromResult.petPath.ShouldBe(emulator.petPath);
        }

        [TestMethod()]
        public void FromJson_WithUnpopulatedJSON_ShouldFail()
        {
            // Arrange ...
            var json = string.Empty;

            // Act ...
            var result = Cc65Emulators.FromJson(json);

            // Assert ...
            result.ShouldBeNull();
        }

        [TestMethod()]
        public void FromJson_WithPopulatedJSON_ShouldSucceed()
        {
            // Arrange ...
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), @"Test Files");
            filepath = Path.Combine(filepath, "emulators.json");
            var json = File.ReadAllText(filepath);

            // Act ...
            var result = Cc65Emulators.FromJson(json);

            // Assert ...
            result.ShouldNotBeNull();
            result.c64Path.ShouldContain("x64sc.exe");
            result.c128Path.ShouldContain("x128.exe");
            result.petPath.ShouldContain("xpet.exe");            
        }
    }
}
