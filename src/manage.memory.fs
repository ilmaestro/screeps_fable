module MemoryManager
open System
open System.Collections.Generic
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Helpers
(*
    Todos:
    - check for and set global priority flags such as:
        - attacked structures
        - ??
*)
[<Emit("delete Memory.creeps[$0]")>]
let deleteCreepMemory name = jsNative

let logDelete name =
    printfn "deleting %s" name
    name

let clearDeadCreepMems (creepKeys: ResizeArray<string>) =
    // check for dead creep memories..
    getKeys Globals.Memory.creeps
    |> List.filter (creepKeys.Contains >> not)
    |> List.iter (logDelete >> deleteCreepMemory)

let setCurrentCreepCount (creepKeys: ResizeArray<string>) =
    setGameMemory({ gameMemory() with creepCount = creepKeys.Count})

let run () =
    let creepKeys = new ResizeArray<string> (getKeys Globals.Game.creeps)
    clearDeadCreepMems creepKeys
    setCurrentCreepCount creepKeys