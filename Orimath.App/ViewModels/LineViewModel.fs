﻿namespace Orimath.ViewModels
open System
open Orimath.Core
open Orimath.Core.NearlyEquatable
open Orimath.Plugins

type LineViewModel(line: LineSegment, pointConverter: PointConverter) =
    inherit NotifyPropertyChanged()
    let screenPoint1 = pointConverter.ModelToScreen(line.Point1)
    let screenPoint2 = pointConverter.ModelToScreen(line.Point2)
    
    member __.Source = line
    member __.X1 = screenPoint1.X
    member __.Y1 = screenPoint1.Y
    member __.X2 = screenPoint2.X
    member __.Y2 = screenPoint2.Y
    member __.XFactor = line.Line.XFactor
    member __.YFactor = line.Line.YFactor
    member __.Intercept = line.Line.Intercept
    member this.Slope = !+(this.XFactor / !-this.YFactor)
    member this.Angle = !+(atan2 this.XFactor !-this.YFactor / Math.PI * 180.0) % 180.0
   
    override this.ToString() =
        line.Line.ToString() +
        String.Format("\r\n傾き:{0:0.#####} 角度:{1:0.#####}°", this.Slope, this.Angle)
