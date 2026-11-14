using MathLock.Models;
using Xunit;

namespace MathLock.Tests;

public class AppSettingsTests
{
    [Fact]
    public void DefaultPin_VerifiesCorrectly()
    {
        var settings = new AppSettings();
        Assert.True(settings.VerifyPin("1234"));
        Assert.False(settings.VerifyPin("0000"));
        Assert.False(settings.VerifyPin("1235"));
    }

    [Fact]
    public void CustomPin_ComputesAndVerifiesCorrectly()
    {
        var settings = new AppSettings
        {
            ParentPinSha256 = AppSettings.ComputeSha256("9876")
        };

        Assert.True(settings.VerifyPin("9876"));
        Assert.False(settings.VerifyPin("1234"));
    }
}
