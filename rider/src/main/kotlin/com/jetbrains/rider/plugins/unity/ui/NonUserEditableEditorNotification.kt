package com.jetbrains.rider.plugins.unity.ui

import com.intellij.openapi.fileEditor.FileEditor
import com.intellij.openapi.project.DumbAware
import com.intellij.openapi.project.Project
import com.intellij.openapi.util.Key
import com.intellij.openapi.vfs.VirtualFile
import com.intellij.ui.EditorNotificationPanel
import com.intellij.ui.EditorNotifications
import com.intellij.util.ui.UIUtil
import com.jetbrains.rd.platform.util.lifetime
import com.jetbrains.rd.util.lifetime.Lifetime
import com.jetbrains.rider.plugins.unity.model.frontendBackend.frontendBackendModel
import com.jetbrains.rider.plugins.unity.actions.ShowFileInUnityAction
import com.jetbrains.rider.plugins.unity.isConnectedToEditor
import com.jetbrains.rider.plugins.unity.isUnityProject
import com.jetbrains.rider.plugins.unity.util.isNonEditableUnityFile
import com.jetbrains.rider.projectView.solution

class NonUserEditableEditorNotification : EditorNotifications.Provider<EditorNotificationPanel>(), DumbAware {

    companion object {
        val KEY = Key.create<EditorNotificationPanel>("non-user.editable.source.file.editing.notification.panel")
    }

    override fun getKey(): Key<EditorNotificationPanel> = KEY

    override fun createNotificationPanel(file: VirtualFile, fileEditor: FileEditor, project: Project): EditorNotificationPanel? {

        if (project.isUnityProject() && isNonEditableUnityFile(file)) {
            val panel = EditorNotificationPanel()
            panel.text = "This file is internal to Unity and should not be edited manually."
            if (!file.extension.equals("meta", true)) {
                UIUtil.invokeLaterIfNeeded {
                    addShowInUnityAction(project.lifetime, panel, file, project)
                }
            }
            return panel
        }

        return null
    }

    private fun addShowInUnityAction(lifetime : Lifetime, panel: EditorNotificationPanel, file: VirtualFile, project: Project) {

        val model = project.solution.frontendBackendModel

        val link = panel.createActionLabel("Show in Unity") {
            ShowFileInUnityAction.execute(project, file)
        }

        link.isVisible = project.isConnectedToEditor()

        model.unityEditorConnected.advise(lifetime) { link.isVisible = it }
    }
}

