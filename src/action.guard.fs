module Action.Guard
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Model.Domain
open Action.Helpers
open Manage.Memory
(*
    Guard goals:
    body: ranged attack, heal, tough, move
    purpose: patrol the perimeter and look for and attack hostile creeps
    states: attack hostiles, heal friends, patrol the perimeter

    TODO: 
    - fix healing!
    - what happens if a flag is removed an creep still has flag assigned.
*)

let run(creep: Creep, memory: CreepMemory) =
    match memory.actionFlag with
    | Some flagName ->
        let flag = unbox<Flag>(Globals.Game.flags?(flagName))
        let flagMem = MemoryInFlag.get flag

        beginAction creep
        |> healSelf
        |> defendHostiles
        //|> healFriends
        |> locateFlag flag flagMem.actionRadius
        |> endAction memory
    | None ->
        //printfn "%s is defending" creep.name 
        beginAction creep
        |> healSelf
        |> defendHostiles
        //|> healFriends
        |> patrol
        |> endAction memory
        //printfn "%s finished defending" creep.name 
    