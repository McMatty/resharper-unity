using JetBrains.Application;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Implementation;
using JetBrains.ReSharper.Plugins.Unity.ShaderLab.Psi;
using JetBrains.ReSharper.Psi.PerformanceThreshold.Settings;
using JetBrains.Util;

namespace JetBrains.ReSharper.Plugins.Unity.Settings
{
    [ShellComponent]
    public class UnityDefaultPerformanceThresholdSettingsProvider : HaveDefaultSettings
    {
        public UnityDefaultPerformanceThresholdSettingsProvider(ILogger logger, ISettingsSchema settingsSchema)
            : base(logger, settingsSchema) { }

        public override void InitDefaultSettings(ISettingsStorageMountPoint mountPoint)
        {
            SetIndexedValue(mountPoint, PerformanceThresholdSettingsAccessor.AnalysisFileSizeThreshold, ShaderLabLanguage.Name, 300000);
            SetIndexedValue(mountPoint, PerformanceThresholdSettingsAccessor.BuildPsiFileSizeThreshold, ShaderLabLanguage.Name, 4096000);
        }

        public override string Name => "UnityDefaultPerformanceThresholdSettingsProvider";
    }
}