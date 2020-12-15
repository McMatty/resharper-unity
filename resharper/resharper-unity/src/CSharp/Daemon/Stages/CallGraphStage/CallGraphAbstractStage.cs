using System;
using System.Collections.Generic;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Daemon.CSharp.CallGraph;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Feature.Services.CSharp.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Plugins.Unity.CSharp.Daemon.Stages.ContextSystem;
using JetBrains.ReSharper.Plugins.Unity.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace JetBrains.ReSharper.Plugins.Unity.CSharp.Daemon.Stages.CallGraphStage
{
    public abstract class CallGraphAbstractStage : CSharpDaemonStageBase
    {
        private readonly CallGraphSwaExtensionProvider mySwaExtensionProvider;
        private readonly IEnumerable<ICallGraphContextProvider> myContextProviders;
        private readonly IEnumerable<ICallGraphProblemAnalyzer> myProblemAnalyzers;
        private readonly ILogger myLogger;

        protected CallGraphAbstractStage(
            CallGraphSwaExtensionProvider swaExtensionProvider,
            IEnumerable<ICallGraphContextProvider> contextProviders,
            IEnumerable<ICallGraphProblemAnalyzer> problemAnalyzers,
            ILogger logger)
        {
            mySwaExtensionProvider = swaExtensionProvider;
            myContextProviders = contextProviders;
            myProblemAnalyzers = problemAnalyzers;
            myLogger = logger;
        }

        protected override IDaemonStageProcess CreateProcess(IDaemonProcess process,
            IContextBoundSettingsStore settings,
            DaemonProcessKind processKind, ICSharpFile file)
        {
            var sourceFile = file.GetSourceFile();

            if (!file.GetProject().IsUnityProject() || !mySwaExtensionProvider.IsApplicable(sourceFile))
                return null;

            return new CallGraphProcess(process, processKind, file, myLogger, myContextProviders, myProblemAnalyzers);
        }
    }

    public class CallGraphProcess : CSharpDaemonStageProcessBase
    {
        private readonly DaemonProcessKind myProcessKind;
        private readonly ILogger myLogger;
        private readonly IEnumerable<ICallGraphContextProvider> myContextProviders;
        private readonly IEnumerable<ICallGraphProblemAnalyzer> myProblemAnalyzers;
        private readonly CallGraphContext myContext = new CallGraphContext();

        public CallGraphProcess(
            IDaemonProcess process,
            DaemonProcessKind processKind,
            ICSharpFile file,
            ILogger logger,
            IEnumerable<ICallGraphContextProvider> contextProviders,
            IEnumerable<ICallGraphProblemAnalyzer> problemAnalyzers)
            : base(process, file)
        {
            myProcessKind = processKind;
            myLogger = logger;
            myContextProviders = contextProviders;
            myProblemAnalyzers = problemAnalyzers;
        }

        public override void Execute(Action<DaemonStageResult> committer)
        {
            var highlightingConsumer = new FilteringHighlightingConsumer(DaemonProcess.SourceFile, File,
                DaemonProcess.ContextBoundSettingsStore);

            File.ProcessThisAndDescendants(this, highlightingConsumer);

            committer(new DaemonStageResult(highlightingConsumer.Highlightings));
        }

        public override void ProcessBeforeInterior(ITreeNode element, IHighlightingConsumer consumer)
        {
            myContext.AdvanceContext(element, myProcessKind, myContextProviders);

            try
            {
                foreach (var problemAnalyzer in myProblemAnalyzers)
                    problemAnalyzer.RunInspection(element, DaemonProcess, myProcessKind, consumer, myContext);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                myLogger.Error(exception, "An exception occured during call graph problem analyzer execution");
            }
        }

        public override void ProcessAfterInterior(ITreeNode element, IHighlightingConsumer consumer)
        {
            base.ProcessAfterInterior(element, consumer);

            myContext.Rollback(element);
        }
    }
}