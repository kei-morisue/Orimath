﻿module Orimath.Plugins.Folding
open Geometry
open NearlyEquatable

[<CompiledName("FromAxiom1")>]
let axiom1 p1 p2 =
    if p1 = p2 then None
    else Some(Line.Create(p1.Y - p2.Y, p2.X - p1.X, p1.X * p2.Y - p2.X * p1.Y))

[<CompiledName("FromAxiom2")>]
let axiom2 p1 p2 =
    if p1 = p2 then None
    else Some(Line.Create(p1.X - p2.X, p1.Y - p2.Y, (p2.X * p2.X + p2.Y * p2.Y - p1.X * p1.X - p1.Y * p1.Y) / 2.0))

[<CompiledName("FromAxiom3")>]
let axiom3 (line1: Line) (line2: Line) =
    let isParallel = line1.Slope = line2.Slope
    if isParallel && line1.Intercept = line2.Intercept then []
    else
        let result1 = Line.Create(line1.A + line2.A, line1.B + line2.B, line1.C + line2.C)
        if isParallel then
            [result1]
        else
            [
                result1
                Line.Create(line1.A - line2.A, line1.B - line2.B, line1.C - line2.C)
            ]

[<CompiledName("FromAxiom4")>]
let axiom4 point line = Line.Create(-line.B, line.A, line.B * point.X - line.A * point.Y)

[<CompiledName("FromAxiom5")>]
let axiom5 ontoLine passPoint ontoPoint =
    if containsPoint ontoLine ontoPoint then []
    else
        let n = (ontoLine.A * passPoint.X + ontoLine.B * passPoint.Y + ontoLine.C)
        let dist = (passPoint.X - ontoPoint.X) * (passPoint.X - ontoPoint.X) + (passPoint.Y - ontoPoint.Y) * (passPoint.Y - ontoPoint.Y)
        let delta = dist - n * n
        if delta < 0.0 then
            []
        elif delta = 0.0 then
            [ axiom2 ontoPoint { X = passPoint.X - ontoLine.A * n; Y = passPoint.Y - ontoLine.B * n } ]
            |> List.choose id
        else
            let s = (sqrt delta)
            [
                axiom2 ontoPoint { X = passPoint.X - (ontoLine.A * n + ontoLine.B * s); Y = passPoint.Y - (ontoLine.B * n - ontoLine.A * s) }
                axiom2 ontoPoint { X = passPoint.X - (ontoLine.A * n - ontoLine.B * s); Y = passPoint.Y - (ontoLine.B * n + ontoLine.A * s) }
            ]
            |> List.choose id


[<CompiledName("FromAxiom6")>]
let axiom6 (line1: Line) (point1: Point) (line2: Line) (point2: Point) =
    let cbrt x = if x < 0.0 then -(-x ** (1.0 / 3.0)) else x ** (1.0 / 3.0)
    let hxrt x = if x < 0.0 then -(-x ** (1.0 / 6.0)) else x ** (1.0 / 6.0)
    let solveEquation a b c d =
        let p = (3.0 * a * c - b * b) / (3.0 * a * a)
        let q = (2.0 * b * b * b - 9.0 * a * b * c + 27.0 * a * a * d) / (27.0 * a * a * a)
        let ys =
            if p = 0.0 && q = 0.0 then
                [ 0.0 ]
            else
                let delta = (27.0 * q * q + 4.0 * p * p * p) / 108.0
                if delta > 0.0 then
                    let s = sqrt delta
                    [ cbrt (-q / 2.0 + s) + cbrt (-q / 2.0 - s) ]
                elif delta = 0.0 then
                    let s = cbrt (q / 2.0)
                    [ -2.0 * s; s ]
                else
                    let α = -q / 2.0
                    let β = sqrt -delta
                    let s = 2.0 * hxrt (α * α - delta)
                    let t = atan2 β α / 3.0
                    let u = System.Math.PI / 1.5
                    [ s * cos t; s * cos (t + u); s * cos (t + u * 2.0) ]
        ys |> List.map(fun y -> y - b / (3.0 * a))

    let rec getFactors a1 b1 c1 x1 y1 a2 b2 c2 x2 y2 rev =
        let n1 = a1 * x1 + b1 * y1 + c1
        let n2 = a2 * x2 + b2 * y2 + c2
        let x = x1 - x2
        let y = y1 - y2
        let d = a1 * n2 - a2 * n1
        let e = b1 * n2 - b2 * n1
        let f = 2.0 * (a1 * b2 + a2 * b1)
        let a = 2.0 * a1 * a2
        let b = 2.0 * b1 * b2
        let t1 = d + a * x
        if t1 =~~ 0.0 then
            if not rev then
                getFactors b1 a1 c1 y1 x1 b2 a2 c2 y2 x2 true
            else
                // 縦・横どちらにも傾き∞を含む場合、その2つのみが解となる
                [
                    Line.Create(1.0, 0.0, -x1 + n1 / (2.0 * a1))
                    Line.Create(0.0, 1.0, -y1 + n1 / (2.0 * b1))
                ]
        else
            let t2 = e + f * x + a * y
            let t3 = d + f * y + b * x
            let t4 = e + b * y
            System.Diagnostics.Debug.Print(System.String.Format("{0:0.####} | {1:0.####} | {2:0.####} | {3:0.####}", t1, t2, t3, t4))
            let xfs = solveEquation t1 t2 t3 t4
            System.Diagnostics.Debug.Print(System.String.Join(" ; ", xfs))
            let yf = 1.0
            xfs |> List.map(fun xf ->
                let c = -(x1 * xf) - y1 + (n1 * (xf * xf + 1.0)) / (2.0 * (a1 * xf + b1))
                if rev then Line.Create(yf, xf, c) else Line.Create(xf, yf, c))
    let result = getFactors line1.A line1.B line1.C point1.X point1.Y line2.A line2.B line2.C point2.X point2.Y false
    result

[<CompiledName("FromAxiom7")>]
let axiom7 passLine ontoLine ontoPoint =
    if containsPoint ontoLine ontoPoint then None
    else
        let c = passLine.B * ontoPoint.X - passLine.A * ontoPoint.Y -
                (ontoLine.A * ontoPoint.X + ontoLine.B * ontoPoint.Y + ontoLine.C) / (2.0 * (ontoLine.A * passLine.B - passLine.A * ontoLine.B))
        Some(Line.Create(-passLine.B, passLine.A, c))
