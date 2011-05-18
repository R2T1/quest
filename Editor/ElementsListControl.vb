﻿<ControlType("elementslist")> _
Public Class ElementsListControl
    Implements IElementEditorControl
    Implements IResizableElementEditorControl
    Implements IListEditorDelegate

    Private WithEvents m_controller As EditorController
    Private m_data As IEditorData
    Private m_controlData As IEditorControl
    Private m_elementType As String
    Private m_objectType As String
    Private m_typeDesc As String
    Private m_filter As String

    Public ReadOnly Property AttributeName As String Implements IElementEditorControl.AttributeName
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property Control As System.Windows.Forms.Control Implements IElementEditorControl.Control
        Get
            Return Me
        End Get
    End Property

    Public Property Controller As EditorController Implements IElementEditorControl.Controller
        Get
            Return m_controller
        End Get
        Set(value As EditorController)
            m_controller = value
        End Set
    End Property

    Public Event Dirty(sender As Object, args As DataModifiedEventArgs) Implements IElementEditorControl.Dirty
    Public Event RequestParentElementEditorSave() Implements IElementEditorControl.RequestParentElementEditorSave

    Public Sub Initialise(controller As EditorController, controlData As IEditorControl) Implements IElementEditorControl.Initialise
        ctlListEditor.EditorDelegate = Me
        ctlListEditor.Style = ListEditor.ColumnStyle.OneColumn
        ctlListEditor.UpdateList(Nothing)
        m_controller = controller
        m_controlData = controlData

        m_elementType = controlData.GetString("elementtype")
        m_objectType = controlData.GetString("objecttype")
        m_filter = controlData.GetString("listfilter")

        If m_filter IsNot Nothing Then
            m_typeDesc = m_filter
        ElseIf m_objectType IsNot Nothing Then
            m_typeDesc = m_objectType
        Else
            m_typeDesc = m_elementType
        End If
    End Sub

    Public Sub Populate(data As IEditorData) Implements IElementEditorControl.Populate
        ' TO DO: Implement filtering, so we can place this control on e.g. an object editor to have it show
        ' just the sub-objects of that object.
        'm_data = data
        'If data IsNot Nothing Then
        '    m_elementName = data.Name
        'Else
        '    m_elementName = Nothing
        'End If

        Dim elements As IEnumerable(Of String)
        If m_elementType = "object" Then
            elements = Controller.GetObjectNames(m_objectType, False)
        Else
            elements = Controller.GetElementNames(m_elementType, False)
        End If

        Dim listItems As New Dictionary(Of String, String)

        For Each element In elements.Where(Function(e) Filter(e))
            listItems.Add(element, Controller.GetDisplayName(element))
        Next

        ctlListEditor.UpdateList(listItems)

    End Sub

    Private Function Filter(element As String) As Boolean
        If m_filter Is Nothing Then Return True
        Return Controller.ElementIsVerb(element) = (m_filter = "verb")
    End Function

    Public Sub Save(data As IEditorData) Implements IElementEditorControl.Save

    End Sub

    Public Property Value As Object Implements IElementEditorControl.Value
        Get
            Return Nothing
        End Get
        Set(value As Object)
        End Set
    End Property

    Public Sub DoAdd() Implements IListEditorDelegate.DoAdd
        Controller.UIRequestAddElement(m_elementType, m_objectType, m_filter)
    End Sub

    Public Sub DoEdit(key As String, index As Integer) Implements IListEditorDelegate.DoEdit
        Controller.UIRequestEditElement(key)
    End Sub

    Public Sub DoRemove(keys() As String) Implements IListEditorDelegate.DoRemove
        Controller.StartTransaction(String.Format("Delete {0} {1}s", keys.Length, m_typeDesc))

        For Each key In keys
            ' Deleting an element will delete any children, so we need to check that we've not already
            ' deleted the element if the user selected multiple elements to delete
            If Controller.ElementExists(key) Then
                Controller.DeleteElement(key, False)
            End If
        Next

        Controller.EndTransaction()
    End Sub

    Private Sub ctlListEditor_ToolbarClicked() Handles ctlListEditor.ToolbarClicked
        RaiseEvent RequestParentElementEditorSave()
    End Sub

    Public ReadOnly Property ExpectedType As System.Type Implements IElementEditorControl.ExpectedType
        Get
            Return Nothing
        End Get
    End Property

    Private Sub m_controller_ElementsUpdated() Handles m_controller.ElementsUpdated
        Populate(m_data)
    End Sub
End Class