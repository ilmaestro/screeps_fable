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
    //printfn "%s is defending" creep.name 
    beginAction creep
    |> defendHostiles
    |> healFriends
    |> healSelf
    |> patrol
    |> endAction memory
    //printfn "%s finished defending" creep.name 
    