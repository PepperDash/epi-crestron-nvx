using FluentAssertions;
using Xunit;

namespace NvxEpi.Tests;

// POCO configs (no [JsonProperty] attributes) - Newtonsoft's default case-insensitive
// contract is relied on, so these verify class shape rather than attribute contracts.
public class ConfigDeserializationTests
{
    [Theory]
    [InlineData("NvxDeviceProperties")]
    [InlineData("NvxDirectorConfig")]
    [InlineData("NvxMockDeviceProperties")]
    public void Config_Class_Exists(string className)
    {
        AssemblyFixture.PluginAssembly.GetType($"NvxEpi.Features.Config.{className}")
            .Should().NotBeNull();
    }

    [Theory]
    [InlineData("NvxDeviceProperties")]
    [InlineData("NvxDirectorConfig")]
    [InlineData("NvxMockDeviceProperties")]
    public void Config_Has_Parameterless_Constructor(string className)
    {
        var type = AssemblyFixture.PluginAssembly.GetType($"NvxEpi.Features.Config.{className}");
        type!.GetConstructor(Type.EmptyTypes).Should().NotBeNull();
    }

    [Theory]
    [InlineData("NvxDeviceProperties", "DeviceId")]
    [InlineData("NvxDeviceProperties", "Control")]
    [InlineData("NvxDeviceProperties", "Mode")]
    [InlineData("NvxDeviceProperties", "MulticastVideoAddress")]
    [InlineData("NvxDeviceProperties", "MulticastAudioAddress")]
    [InlineData("NvxDeviceProperties", "EnableAutoRoute")]
    [InlineData("NvxDirectorConfig", "Control")]
    [InlineData("NvxDirectorConfig", "NumberOfDomains")]
    public void Config_Has_Expected_Property(string className, string propertyName)
    {
        var type = AssemblyFixture.PluginAssembly.GetType($"NvxEpi.Features.Config.{className}");
        type!.GetProperty(propertyName).Should().NotBeNull();
    }
}
