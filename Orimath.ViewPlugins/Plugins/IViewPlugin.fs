﻿namespace Orimath.Plugins

[<NoComparison; ReferenceEquality>]
type ViewPluginArgs =
    {
        Workspace: IWorkspace
        Messenger: IMessenger
        Dispatcher: IDispatcher
        PointConverter: IViewPointConverter
    }

type IViewPlugin =
    abstract member Execute : args: ViewPluginArgs -> unit
