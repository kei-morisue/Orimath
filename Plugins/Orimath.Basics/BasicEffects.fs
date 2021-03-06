﻿namespace Orimath.Basics
open System.Reflection
open Orimath.Plugins

type UndoEffect(workspace: IWorkspace) =
    interface IEffect with
        member val MenuHieralchy = [| "編集" |]
        member __.Name = "元に戻す"
        member __.ShortcutKey = "Ctrl+Z"
        member __.Icon = Assembly.GetExecutingAssembly().GetManifestResourceStream("Orimath.Basics.Icons.undo.png")
        member __.CanExecute() = workspace.Paper.CanUndo
        member __.Execute() = workspace.Paper.Undo()
        [<CLIEvent>]
        member __.CanExecuteChanged = workspace.Paper.CanUndoChanged

type RedoEffect(workspace: IWorkspace) =
    interface IEffect with
        member val MenuHieralchy = [| "編集" |]
        member __.Name = "やり直し"
        member __.ShortcutKey = "Ctrl+Y"
        member __.Icon = Assembly.GetExecutingAssembly().GetManifestResourceStream("Orimath.Basics.Icons.redo.png")
        member __.CanExecute() = workspace.Paper.CanRedo
        member __.Execute() = workspace.Paper.Redo()
        [<CLIEvent>]
        member __.CanExecuteChanged = workspace.Paper.CanUndoChanged
