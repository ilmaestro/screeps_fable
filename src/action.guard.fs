module Action.Guard
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Model.Domain
open Manage.Memory.MemoryInCreep
open Action.Helpers

(*
    Guard goals:
    body: ranged attack, heal, tough, move
    purpose: patrol the perimeter and look for and attack hostile creeps
    states: attack hostiles, heal friends, patrol the perimeter
*)

let run(creep: Creep, memory: CreepMemory) =
    let guard() =
        beginAction creep
        |> defendHostiles
//        |> healFriends
//        |> patrol
        |> endAction memory

    match memory.lastAction with
    | _ -> guard()