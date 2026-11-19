using System.Linq;
using FluentAssertions;
using Xunit;

namespace NvxEpi.Tests;

// MinimumEssentialsFrameworkVersion and TypeNames are assigned at runtime in the factory
// constructor, which MetadataLoadContext cannot execute - source scanning is the only way
// to verify these without spinning up the Essentials runtime.
public class FactoryMetadataTests
{
    private const string ExpectedMinimumEssentialsFrameworkVersion = "3.0.0";

    [Theory]
    [InlineData("Nvx35XDeviceFactory")]
    [InlineData("Nvx36XDeviceFactory")]
    [InlineData("Nvx38XDeviceFactory")]
    [InlineData("NvxD3XDeviceFactory")]
    [InlineData("NvxE20DeviceFactory")]
    [InlineData("NvxE3XDeviceFactory")]
    [InlineData("NvxMockDeviceFactory")]
    public void Factory_Source_Sets_MinimumEssentialsFrameworkVersion_Via_Shared_Const(string factoryClassName)
    {
        var source = AssemblyFixture.FindSourceForClass(factoryClassName);
        source.Should().NotBeNull();
        source!.Should().Contain("MinimumEssentialsFrameworkVersion = MinumumEssentialsVersion");
    }

    [Fact]
    public void NvxDirectorFactory_Sets_MinimumEssentialsFrameworkVersion_Via_Shared_Const()
    {
        var source = AssemblyFixture.FindSourceForClass("NvxDirectorFactory");
        source.Should().NotBeNull();
        source!.Should().Contain("MinimumEssentialsFrameworkVersion = NvxBaseDeviceFactory<NvxXioDirector>.MinumumEssentialsVersion");
    }

    [Fact]
    public void SharedMinimumEssentialsVersionConst_Matches_Expected()
    {
        var source = AssemblyFixture.FindSourceForClass("NvxBaseDeviceFactory");
        source.Should().NotBeNull();
        source!.Should().Contain($"MinumumEssentialsVersion = \"{ExpectedMinimumEssentialsFrameworkVersion}\"");
    }

    [Theory]
    [InlineData("Nvx35XDeviceFactory", "dmnvx350")]
    [InlineData("Nvx36XDeviceFactory", "dmnvx360")]
    [InlineData("Nvx38XDeviceFactory", "dmnvx384")]
    [InlineData("Nvx38XDeviceFactory", "dmnvx384c")]
    [InlineData("Nvx38XDeviceFactory", "dmnvx385")]
    [InlineData("Nvx38XDeviceFactory", "dmnvx385c")]
    [InlineData("NvxD3XDeviceFactory", "dmnvxd30")]
    [InlineData("NvxE20DeviceFactory", "dmnvxe20")]
    [InlineData("NvxE3XDeviceFactory", "dmnvxe30")]
    [InlineData("NvxMockDeviceFactory", "MockNvxDevice")]
    [InlineData("NvxDirectorFactory", "xiodirector")]
    [InlineData("NvxApplicationFactory", "dynnvx")]
    public void Factory_Source_Contains_TypeName(string factoryClassName, string typeName)
    {
        var source = AssemblyFixture.FindSourceForClass(factoryClassName);
        source.Should().NotBeNull();
        source!.Should().Contain($"\"{typeName}\"");
    }

    // TypeNames and the hardware switch must stay in sync: a type listed in a factory but
    // missing from NvxBaseDeviceFactory.GetDeviceBuildAction throws NotSupportedException at load.
    [Theory]
    [InlineData("dmnvx384")]
    [InlineData("dmnvx384c")]
    [InlineData("dmnvx385")]
    [InlineData("dmnvx385c")]
    public void BaseFactory_Source_Has_Active_Case_For_TypeName(string typeName)
    {
        var source = AssemblyFixture.FindSourceForClass("NvxBaseDeviceFactory");
        source.Should().NotBeNull();
        source!.Split('\n')
            .Select(line => line.Trim())
            .Should().Contain($"case \"{typeName}\":");
    }
}
