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

let clearDeadCreepMems () =
    // check for dead creep memories..
    let creepKeys = new ResizeArray<string> (getKeys Globals.Game.creeps)
    getKeys Globals.Memory.creeps
    |> List.filter (creepKeys.Contains >> not)
    |> List.iter deleteCreepMemory

let run () =
    clearDeadCreepMems()