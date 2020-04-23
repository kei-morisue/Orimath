﻿namespace Orimath.Plugins
open Orimath.Core

type SelectorTool(workspace: IWorkspace) =
    abstract member Name : string
    abstract member ShortcutKey : string
    abstract member OnClick : target: DisplayTarget * point: Point * modifier: OperationModifier -> unit
    abstract member BeginDrag : target: DisplayTarget * point: Point * modifier: OperationModifier -> bool
    abstract member DragEnter : target: DisplayTarget * point: Point * modifier: OperationModifier -> bool
    abstract member DragLeave : target: DisplayTarget * point: Point * modifier: OperationModifier -> bool
    abstract member DragOver : target: DisplayTarget * point: Point * modifier: OperationModifier -> bool
    abstract member Drop : target: DisplayTarget * point: Point * modifier: OperationModifier -> unit

    default __.Name = "選択"
    default __.ShortcutKey = ""
    default __.BeginDrag(_, _, _) = false
    default __.DragEnter(_, _, _) = false
    default __.DragLeave(_, _, _) = false
    default __.DragOver(_, _, _) = false
    default __.Drop(_, _, _) = ()
    default __.OnClick(target, _, modifier) =
        if modifier = OperationModifier.None then
            match target with
            |DisplayTarget.None ->
                workspace.SelectedLayers <- Array.empty
                workspace.SelectedEdges <- Array.empty
                workspace.SelectedLines <- Array.empty
                workspace.SelectedPoints <- Array.empty
            |DisplayTarget.Layer(l) ->
                workspace.SelectedLayers <- [| l |]
                workspace.SelectedEdges <- Array.empty
                workspace.SelectedLines <- Array.empty
                workspace.SelectedPoints <- Array.empty
            |DisplayTarget.Edge(e) ->
                workspace.SelectedLayers <- Array.empty
                workspace.SelectedEdges <- [| e |]
                workspace.SelectedLines <- Array.empty
                workspace.SelectedPoints <- Array.empty
            |DisplayTarget.Line(l) ->
                workspace.SelectedLayers <- Array.empty
                workspace.SelectedEdges <- Array.empty
                workspace.SelectedLines <- [| l |]
                workspace.SelectedPoints <- Array.empty
            |DisplayTarget.Point(p) ->
                workspace.SelectedLayers <- Array.empty
                workspace.SelectedEdges <- Array.empty
                workspace.SelectedLines <- Array.empty
                workspace.SelectedPoints <- [| p |]

    interface ITool with
        member this.Name = this.Name
        member this.ShortcutKey = this.ShortcutKey
        member this.OnClick(target, point, modifier) = this.OnClick(target, point, modifier)
        member this.BeginDrag(target, point, modifier) = this.BeginDrag(target, point, modifier)
        member this.DragEnter(target, point, modifier) = this.DragEnter(target, point, modifier)
        member this.DragLeave(target, point, modifier) = this.DragLeave(target, point, modifier)
        member this.DragOver(target, point, modifier) = this.DragOver(target, point, modifier)
        member this.Drop(target, point, modifier) = this.Drop(target, point, modifier)