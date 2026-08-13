using FluentAssertions;
using Xunit;

namespace NvxEpi.Tests;

public class FactoryDiscoveryTests
{
    [Fact]
    public void Assembly_Loads_Successfully()
    {
        AssemblyFixture.PluginAssembly.Should().NotBeNull();
    }

    [Fact]
    public void Assembly_Name_Matches_Expected()
    {
        AssemblyFixture.PluginAssembly.GetName().Name
            .Should().Be("PepperDash.Essentials.Plugins.Crestron.Nvx");
    }

    [Fact]
    public void Factory_Count_Matches_Expected()
    {
        AssemblyFixture.FindFactoryTypes().Should().HaveCount(9);
    }

    [Theory]
    [InlineData("Nvx35XDeviceFactory")]
    [InlineData("Nvx36XDeviceFactory")]
    [InlineData("Nvx38XDeviceFactory")]
    [InlineData("NvxD3XDeviceFactory")]
    [InlineData("NvxE20DeviceFactory")]
    [InlineData("NvxE3XDeviceFactory")]
    [InlineData("NvxMockDeviceFactory")]
    [InlineData("NvxDirectorFactory")]
    [InlineData("NvxApplicationFactory")]
    public void Factory_Exists_ByName(string factoryClassName)
    {
        var factories = AssemblyFixture.FindFactoryTypes();
        factories.Should().Contain(t => t.Name == factoryClassName);
    }

    [Fact]
    public void All_Factories_Have_Parameterless_Constructor()
    {
        foreach (var factory in AssemblyFixture.FindFactoryTypes())
        {
            factory.GetConstructor(Type.EmptyTypes)
                .Should().NotBeNull($"factory {factory.Name} must have a parameterless constructor for plugin discovery");
        }
    }
}
