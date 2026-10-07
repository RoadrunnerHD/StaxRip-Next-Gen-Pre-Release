
Imports System.Threading
Imports System.Threading.Tasks

Imports StaxRip.UI

Friend Class JobsForm
    Inherits DialogBase

#Region " Designer "
    Friend WithEvents bnStart As StaxRip.UI.ButtonEx
    Friend WithEvents bnDown As StaxRip.UI.ButtonEx
    Friend WithEvents bnUp As StaxRip.UI.ButtonEx
    Friend WithEvents bnLoad As StaxRip.UI.ButtonEx
    Friend WithEvents stb As StaxRip.SearchTextBox
    Friend WithEvents lv As ListViewEx
    Friend WithEvents tlpMain As TableLayoutPanel
    Friend WithEvents tlpButtonsLeft As TableLayoutPanel
    Friend WithEvents tlpButtonsRight As TableLayoutPanel
    Friend WithEvents bnMenu As ButtonEx
    Friend WithEvents bnRemove As ButtonEx
    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Sub InitializeComponent()
        Me.bnDown = New StaxRip.UI.ButtonEx()
        Me.bnUp = New StaxRip.UI.ButtonEx()
        Me.bnStart = New StaxRip.UI.ButtonEx()
        Me.bnLoad = New StaxRip.UI.ButtonEx()
        Me.lv = New StaxRip.UI.ListViewEx()
        Me.stb = New StaxRip.SearchTextBox()
        Me.tlpMain = New System.Windows.Forms.TableLayoutPanel()
        Me.tlpButtonsRight = New System.Windows.Forms.TableLayoutPanel()
        Me.bnMenu = New StaxRip.UI.ButtonEx()
        Me.bnRemove = New StaxRip.UI.ButtonEx()
        Me.tlpButtonsLeft = New System.Windows.Forms.TableLayoutPanel()
        Me.tlpMain.SuspendLayout()
        Me.tlpButtonsRight.SuspendLayout()
        Me.tlpButtonsLeft.SuspendLayout()
        Me.SuspendLayout()
        '
        'bnDown
        '
        Me.bnDown.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.bnDown.Enabled = False
        Me.bnDown.Location = New System.Drawing.Point(8, 0)
        Me.bnDown.Margin = New System.Windows.Forms.Padding(8, 0, 0, 0)
        Me.bnDown.Size = New System.Drawing.Size(100, 70)
        Me.bnDown.TextAlign = ContentAlignment.MiddleCenter
        '
        'bnUp
        '
        Me.bnUp.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.bnUp.Enabled = False
        Me.bnUp.Location = New System.Drawing.Point(726, 0)
        Me.bnUp.Margin = New System.Windows.Forms.Padding(0, 0, 8, 0)
        Me.bnUp.Size = New System.Drawing.Size(100, 70)
        Me.bnUp.TextAlign = ContentAlignment.MiddleCenter
        '
        'bnStart
        '
        Me.bnStart.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.bnStart.Location = New System.Drawing.Point(0, 0)
        Me.bnStart.Margin = New System.Windows.Forms.Padding(0)
        Me.bnStart.Size = New System.Drawing.Size(250, 70)
        Me.bnStart.Text = "Start"
        '
        'bnLoad
        '
        Me.bnLoad.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.bnLoad.Location = New System.Drawing.Point(584, 0)
        Me.bnLoad.Margin = New System.Windows.Forms.Padding(0)
        Me.bnLoad.Size = New System.Drawing.Size(250, 70)
        Me.bnLoad.Text = "Load"
        '
        'stb
        '
        Me.stb.Anchor = CType((AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Top), AnchorStyles)
        Me.stb.Location = New System.Drawing.Point(10, 15)
        Me.stb.Margin = New Padding(0, 0, 0, 15)
        Me.stb.Name = "stb"
        Me.stb.Size = New System.Drawing.Size(453, 70)
        Me.stb.TabIndex = 7
        '
        'lv
        '
        Me.lv.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lv.HideSelection = False
        Me.lv.Location = New System.Drawing.Point(15, 15)
        Me.lv.Margin = New System.Windows.Forms.Padding(0, 0, 0, 14)
        Me.lv.Name = "lv"
        Me.lv.Size = New System.Drawing.Size(1668, 594)
        Me.lv.TabIndex = 8
        Me.lv.UseCompatibleStateImageBehavior = False
        '
        'tlpMain
        '
        Me.tlpMain.SetColumnSpan(Me.stb, 2)
        Me.tlpMain.SetColumnSpan(Me.lv, 2)
        Me.tlpMain.ColumnCount = 2
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpMain.Controls.Add(Me.stb, 0, 0)
        Me.tlpMain.Controls.Add(Me.lv, 0, 1)
        Me.tlpMain.Controls.Add(Me.tlpButtonsLeft, 0, 2)
        Me.tlpMain.Controls.Add(Me.tlpButtonsRight, 1, 2)
        Me.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpMain.Location = New System.Drawing.Point(0, 0)
        Me.tlpMain.Name = "tlpMain"
        Me.tlpMain.Padding = New System.Windows.Forms.Padding(15)
        Me.tlpMain.RowCount = 3
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 66.66666!))
        Me.tlpMain.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.tlpMain.Size = New System.Drawing.Size(1698, 708)
        Me.tlpMain.TabIndex = 15
        '
        'tlpButtonsRight
        '
        Me.tlpButtonsRight.AutoSize = True
        Me.tlpButtonsRight.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpButtonsRight.ColumnCount = 5
        Me.tlpButtonsRight.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.tlpButtonsRight.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpButtonsRight.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.tlpButtonsRight.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.tlpButtonsRight.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.tlpButtonsRight.Controls.Add(Me.bnMenu, 2, 0)
        Me.tlpButtonsRight.Controls.Add(Me.bnLoad, 4, 0)
        Me.tlpButtonsRight.Controls.Add(Me.bnRemove, 3, 0)
        Me.tlpButtonsRight.Controls.Add(Me.bnDown, 0, 0)
        Me.tlpButtonsRight.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpButtonsRight.Location = New System.Drawing.Point(849, 623)
        Me.tlpButtonsRight.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpButtonsRight.Name = "tlpButtonsRight"
        Me.tlpButtonsRight.RowCount = 1
        Me.tlpButtonsRight.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpButtonsRight.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70.0!))
        Me.tlpButtonsRight.Size = New System.Drawing.Size(834, 70)
        Me.tlpButtonsRight.TabIndex = 10
        '
        'bnMenu
        '
        Me.bnMenu.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.bnMenu.Location = New System.Drawing.Point(204, 0)
        Me.bnMenu.Margin = New System.Windows.Forms.Padding(0)
        Me.bnMenu.ShowMenuSymbol = True
        Me.bnMenu.Size = New System.Drawing.Size(100, 70)
        '
        'bnRemove
        '
        Me.bnRemove.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.bnRemove.Location = New System.Drawing.Point(319, 0)
        Me.bnRemove.Margin = New System.Windows.Forms.Padding(15, 0, 15, 0)
        Me.bnRemove.Size = New System.Drawing.Size(250, 70)
        Me.bnRemove.Text = "Remove"
        '
        'tlpButtonsLeft
        '
        Me.tlpButtonsLeft.AutoSize = True
        Me.tlpButtonsLeft.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.tlpButtonsLeft.ColumnCount = 2
        Me.tlpButtonsLeft.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpButtonsLeft.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.tlpButtonsLeft.Controls.Add(Me.bnStart, 0, 0)
        Me.tlpButtonsLeft.Controls.Add(Me.bnUp, 1, 0)
        Me.tlpButtonsLeft.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tlpButtonsLeft.Location = New System.Drawing.Point(15, 623)
        Me.tlpButtonsLeft.Margin = New System.Windows.Forms.Padding(0)
        Me.tlpButtonsLeft.Name = "tlpButtonsLeft"
        Me.tlpButtonsLeft.RowCount = 1
        Me.tlpButtonsLeft.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tlpButtonsLeft.Size = New System.Drawing.Size(834, 70)
        Me.tlpButtonsLeft.TabIndex = 9
        '
        'JobsForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(288.0!, 288.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(1698, 708)
        Me.Controls.Add(Me.tlpMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable
        Me.KeyPreview = True
        Me.Margin = New System.Windows.Forms.Padding(0, 2, 0, 2)
        Me.MinimumSize = New System.Drawing.Size(323, 204)
        Me.Name = "JobsForm"
        Me.Text = $"Jobs - {g.DefaultCommands.GetApplicationDetails()}"
        Me.tlpMain.ResumeLayout(False)
        Me.tlpMain.PerformLayout()
        Me.tlpButtonsRight.ResumeLayout(False)
        Me.tlpButtonsLeft.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private FileWatcher As New FileSystemWatcher
    Private IsLoading As Boolean
    Private BlockSave As Boolean
    Private AllJobs As List(Of Job)

    Private ReadOnly Property FilteredJobs As List(Of Job)
        Get
            Dim lowerText = stb.Text.ToLowerEx()

            If String.IsNullOrEmpty(lowerText) Then
                Return AllJobs.ToList()
            ElseIf lowerText = "<active>" Then
                Return AllJobs.Where(Function(x) x.Active).ToList()
            Else
                Return AllJobs.Where(Function(x) x.Name.ToLower().Contains(lowerText)).ToList()
            End If
        End Get
    End Property

    Sub New()
        MyClass.New(ThemeManager.CurrentTheme)
    End Sub

    Sub New(theme As Theme)
        InitializeComponent()
        RestoreClientSize(55, 35)

        SaveAndLoadSize = True
        AllJobs = JobManager.GetJobs()

        lv.UpButton = bnUp
        lv.DownButton = bnDown
        lv.RemoveButton = bnRemove
        lv.RightClickOnlyForMenu = True

        lv.SingleSelectionButtons = {bnLoad}
        lv.CheckBoxes = True
        lv.EnableListBoxMode()
        lv.ItemCheckProperty = NameOf(Job.Active)
        lv.AddItems(AllJobs)
        lv.SelectFirst()

        SetTitle()

        Dim cms As New ContextMenuStripEx()
        cms.Form = Me
        bnMenu.ContextMenuStrip = cms
        lv.ContextMenuStrip = cms

        AddHandler Disposed, Sub() cms.Dispose()

        AddHandler lv.ItemRemoved, Sub(item)
                                       Dim job = DirectCast(item.Tag, Job)
                                       Dim fp = job.Path

                                       AllJobs.Remove(job)
                                       SaveJobs()

                                       If fp.StartsWith(Path.Combine(Folder.Settings, "Batch Projects") + Path.DirectorySeparatorChar) Then
                                           FileHelp.Delete(fp)
                                       End If
                                   End Sub

        cms.Add("Select All", Sub() SelectAll(), Keys.Control Or Keys.S, Function() lv.Items.Count > lv.SelectedItems.Count)
        cms.Add("Select None", Sub() SelectNone(), Keys.Shift Or Keys.S, Function() lv.SelectedItems.Count > 0)
        cms.Add("-")
        cms.Add("Check Selection", Sub() CheckSelection(), Keys.Shift Or Keys.C, Function() lv.SelectedItems.Count > lv.CheckedItems.OfType(Of ListViewItem).Where(Function(item) item.Checked).Count).ShortcutKeyDisplayString = "Space"
        cms.Add("Check All", Sub() CheckAll(), Keys.Control Or Keys.C, Function() lv.Items.Count > lv.CheckedItems.Count)
        cms.Add("-")
        cms.Add("Uncheck Selection", Sub() UncheckSelection(), Keys.Shift Or Keys.U, Function() lv.SelectedItems.OfType(Of ListViewItem).Where(Function(item) item.Checked).Count > 0).ShortcutKeyDisplayString = "Space"
        cms.Add("Uncheck All", Sub() UncheckAll(), Keys.Control Or Keys.U, Function() lv.CheckedItems.Count > 0)
        cms.Add("-")
        cms.Add("Move Selection Up", Sub() bnUp.PerformClick(), Keys.Control Or Keys.Up, Function() lv.CanMoveUp).SetImage(Symbol.Up)
        cms.Add("Move Selection Down", Sub() bnDown.PerformClick(), Keys.Control Or Keys.Down, Function() lv.CanMoveDown).SetImage(Symbol.Down)
        cms.Add("-")
        cms.Add("Move Selection To Top", Sub() lv.MoveSelectionTop(), Keys.Control Or Keys.Home, Function() lv.CanMoveUp)
        cms.Add("Move Selection To Bottom", Sub() lv.MoveSelectionBottom(), Keys.Control Or Keys.End, Function() lv.CanMoveDown)
        cms.Add("-")
        cms.Add("Sort Alphabetically", Sub() lv.SortItems(), Keys.Control Or Keys.Shift Or Keys.S, Function() lv.Items.Count > 1).SetImage(Symbol.Sort)
        cms.Add("Remove Selection", Sub() bnRemove.PerformClick(), Keys.Delete, Function() lv.SelectedItems.Count > 0).SetImage(Symbol.Remove)
        cms.Add("Load Selection", Sub() bnLoad.PerformClick(), Keys.Control Or Keys.L, Function() lv.SelectedItems.Count = 1)

        bnDown.Symbol = Symbol.Down
        bnUp.Symbol = Symbol.Up

        bnDown.SetFontSize(12)
        bnUp.SetFontSize(12)

        ApplyTheme()
        AddHandler ThemeManager.CurrentThemeChanged, AddressOf OnThemeChanged
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        RemoveHandler ThemeManager.CurrentThemeChanged, AddressOf OnThemeChanged
        components?.Dispose()
        MyBase.Dispose(disposing)
    End Sub

    Sub OnThemeChanged(theme As Theme)
        ApplyTheme(theme)
    End Sub

    Sub ApplyTheme()
        ApplyTheme(ThemeManager.CurrentTheme)
    End Sub

    Sub ApplyTheme(theme As Theme)
        If DesignHelp.IsDesignMode Then Exit Sub

        BackColor = theme.General.BackColor
    End Sub

    Sub UncheckAll()
        BlockSave = True

        For Each item As ListViewItem In lv.Items
            item.Checked = False
        Next

        BlockSave = False
        HandleItemsChanged()
    End Sub

    Sub UncheckSelection()
        BlockSave = True

        For Each item As ListViewItem In lv.SelectedItems
            item.Checked = False
        Next

        BlockSave = False
        HandleItemsChanged()
    End Sub

    Sub CheckAll()
        BlockSave = True

        For Each item As ListViewItem In lv.Items
            item.Checked = True
        Next

        BlockSave = False
        HandleItemsChanged()
    End Sub

    Sub CheckSelection()
        BlockSave = True

        For Each item As ListViewItem In lv.SelectedItems
            item.Checked = True
        Next

        BlockSave = False
        HandleItemsChanged()
    End Sub

    Sub SelectNone()
        BlockSave = True

        For Each item As ListViewItem In lv.Items
            item.Selected = False
        Next

        HandleItemsChanged()
        BlockSave = False
    End Sub

    Sub SelectAll()
        BlockSave = True

        For Each item As ListViewItem In lv.Items
            item.Selected = True
        Next

        HandleItemsChanged()
        BlockSave = False
    End Sub

    Sub HandleItemsChanged(ParamArray indices As Integer())
        If stb.Text = "" AndAlso AllJobs.Count = lv.Items.Count Then
            If indices?.Any() Then
                Dim orderedIndices = indices.OrderByDescending(Function(x) x)

                For Each index As Integer In orderedIndices
                    AllJobs.RemoveAt(index)
                    AllJobs.Insert(index, DirectCast(lv.Items(index).Tag, Job))
                Next
            Else
                AllJobs.Clear()
                AllJobs.AddRange(lv.Items.OfType(Of ListViewItem).Select(Function(x) DirectCast(x.Tag, Job)))
            End If

            If Not BlockSave Then SaveJobs()
        End If

        UpdateControls()
    End Sub

    Sub Reload(sender As Object, e As FileSystemEventArgs)
        AllJobs = JobManager.GetJobs()
        Invoke(Sub()
                   If Not Disposing AndAlso Not IsDisposed Then
                       IsLoading = True
                       lv.BeginUpdate()
                       lv.Items.Clear()
                       lv.AddItems(FilteredJobs)
                       lv.EndUpdate()
                       lv.SelectFirst()
                       UpdateControls()
                       IsLoading = False
                   End If
               End Sub)
    End Sub

    Sub UpdateControls()
        bnStart.Enabled = AllJobs.Any(Function(x) x.Active)
        SetTitle()
    End Sub

    Sub SetTitle()
        Dim all = AllJobs.Count
        Dim filtered = FilteredJobs.Count

        Me.Text = $"Jobs ( {filtered} / {all} ) - {g.DefaultCommands.GetApplicationDetails()}"
    End Sub

    Sub SaveJobs()
        If IsLoading Then Exit Sub

        FileWatcher.EnableRaisingEvents = False
        JobManager.SaveJobs(AllJobs)
        FileWatcher.EnableRaisingEvents = True
    End Sub

    Sub bnStart_Click(sender As Object, e As EventArgs) Handles bnStart.Click
        If Not g.VerifyRequirements Then Exit Sub

        Close()

        If g.IsJobProcessing Then
            g.ShellExecute(Application.ExecutablePath, "-StartJobs -NoFocus")
        Else
            Task.Run(Sub()
                         Thread.Sleep(500)
                         g.MainForm.Invoke(Sub() g.ProcessJobs())
                     End Sub)
        End If
    End Sub

    Sub bnLoad_Click(sender As Object, e As EventArgs) Handles bnLoad.Click
        Dim job = DirectCast(lv.SelectedItem, Job)

        If g.MainForm.LoadProject(job.Path) Then
            Close()
        End If
    End Sub

    Sub SearchTextBox_TextChanged() Handles stb.TextChanged
        BlockSave = True
        lv.BeginUpdate()
        lv.Items.Clear()
        lv.AddItems(FilteredJobs)
        lv.EndUpdate()
        BlockSave = False
        SetTitle()
    End Sub

    Protected Overrides Sub OnActivated(e As EventArgs)
        MyBase.OnActivated(e)
        UpdateControls()
        If Not ProcController.BlockActivation Then
            ProcController.SetLastActivation()
            ProcController.BlockActivation = False
        End If
    End Sub

    Protected Overrides Sub OnFormClosing(args As FormClosingEventArgs)
        MyBase.OnFormClosing(args)
        RemoveHandler FileWatcher.Changed, AddressOf Reload
        RemoveHandler FileWatcher.Created, AddressOf Reload
        FileWatcher.Dispose()
        RemoveHandler lv.ItemsChanged, AddressOf HandleItemsChanged
    End Sub

    Protected Overrides Sub OnLoad(args As EventArgs)
        MyBase.OnLoad(args)

        FileWatcher.Path = Folder.Settings
        FileWatcher.NotifyFilter = NotifyFilters.LastWrite Or NotifyFilters.CreationTime
        FileWatcher.Filter = "Jobs.dat"
        FileWatcher.EnableRaisingEvents = True

        AddHandler FileWatcher.Changed, AddressOf Reload
        AddHandler FileWatcher.Created, AddressOf Reload
        AddHandler lv.ItemsChanged, AddressOf HandleItemsChanged

        UpdateControls()
    End Sub

    Sub ShowForm()
        Using form As New JobsForm()
            form.ShowDialog()
        End Using
    End Sub
End Class
