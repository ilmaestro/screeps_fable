module Harvest
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Helpers

let findEnergy (creep: Creep) =
    match (creep.pos.findClosestByPath(Globals.FIND_SOURCES_ACTIVE)) with
    | Some target ->
        match (creep.harvest(target)) with
        | r when r = Globals.OK -> 
            Harvesting
        | r when r = Globals.ERR_NOT_IN_RANGE -> 
            creep.moveTo(U2.Case2 (box target)) |> ignore
            Moving (Harvest)
        | _ -> Fail
    | None -> Fail

let transferEnergy (creep: Creep) =
    match (creep.pos.findClosestByPath(Globals.FIND_MY_SPAWNS)) with
    | Some spawn ->
        match (creep.transfer(spawn, Globals.RESOURCE_ENERGY)) with
        | r when r = Globals.OK -> Transferring
        | r when r = Globals.ERR_NOT_IN_RANGE ->
            creep.moveTo(U2.Case2 (box spawn)) |> ignore
            Moving (Harvest)
        | _ -> Fail
    | None -> Fail

let run(creep: Creep, memory: CreepMemory) =
    let setLastAction action = setCreepMemory creep { memory with lastAction = action }
    let findEnergy() = findEnergy creep |> setLastAction
    let transfer() = transferEnergy creep |> setLastAction

    match ((creepEnergy creep), memory.lastAction) with
    | (Empty, _) -> 
        findEnergy()
    | (Full, _) -> 
        transfer() // only takes a single tick to transfer
    | (_, lastaction)    ->
        //printfn "%s ?? %A" creep.name lastaction 
        findEnergy()

