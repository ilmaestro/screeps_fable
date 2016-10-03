module Build
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Helpers
open Harvest

let build (creep: Creep) =
    match creep.pos.findClosestByPath(Globals.FIND_CONSTRUCTION_SITES) with
    | Some target ->
        match (creep.build(target)) with
        | r when r = Globals.OK -> Building
        | r when r = Globals.ERR_NOT_IN_RANGE ->
            creep.moveTo(U2.Case2 (box target)) |> ignore
            Moving (Build)
        | r -> (failwith "unhandled error code")
    | None -> Fail

let run(creep: Creep, memory: CreepMemory) =
    let setLastAction action =
        printfn "%s is %A " creep.name action 
        setCreepMemory creep { memory with lastAction = action }
    let findEnergy() = findEnergy creep |> setLastAction
    let build() = build creep |> setLastAction

    match ((creepEnergy creep), memory.lastAction) with
    | (Energy _, Building)         -> build()
    | (Energy _, Moving Build)    -> build()
    | (Energy _, Harvesting)        -> findEnergy()
    | (Energy _, Moving Harvest)    -> findEnergy()
    | (Empty, _)                    -> findEnergy()
    | (Full, _)                     -> build()
    | (_, lastaction)    ->
        printfn "%s ?? %A" creep.name lastaction 
        findEnergy()
