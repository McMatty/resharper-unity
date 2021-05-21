package model.debuggerWorker

import com.jetbrains.rd.generator.nova.*
import com.jetbrains.rd.generator.nova.PredefinedType.*
import com.jetbrains.rd.generator.nova.csharp.CSharp50Generator
import com.jetbrains.rd.generator.nova.kotlin.Kotlin11Generator
import com.jetbrains.rider.model.nova.debugger.main.DebuggerWorkerModel

@Suppress("unused")
object UnityDebuggerWorkerModel : Ext(DebuggerWorkerModel) {

    // Not used in this model, but referenced via debuggerStartInfoBase. Serialisers will be registered along with this
    // model (directly via UnityDebuggerWorkerModel.RegisterDeclaredTypesSerializers() or indirectly via creating a new
    // UnityDebuggerWorkerModel)
    private val unityStartInfoBase = basestruct extends DebuggerWorkerModel.debuggerStartInfoBase {
        field("monoAddress", string.nullable)
        field("monoPort", int)
        field("listenForConnections", bool)
    }

    private val unityIosUsbStartInfo = structdef extends unityStartInfoBase {
        field("iosSupportPath", string.nullable)
        field("iosDeviceId", string.nullable)
    }

    init {
        setting(Kotlin11Generator.Namespace, "com.jetbrains.rider.model.unity.debuggerWorker")
        setting(CSharp50Generator.Namespace, "JetBrains.Rider.Model.Unity.DebuggerWorker")

        property("showCustomRenderers", bool)
    }
}