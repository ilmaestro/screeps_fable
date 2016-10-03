module Upgrade
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Helpers
open Harvest

let upgradeController (creep: Creep) =
    let memory = (creepMemory creep)
    let controller = unbox<Controller> (Globals.Game.getObjectById(memory.controllerId))
    match (creep.upgradeController(controller)) with
    | r when r = Globals.ERR_NOT_IN_RANGE ->
        creep.moveTo(U2.Case2 (box controller)) |> ignore
        Moving (Upgrade)
    | _ -> Upgrading

let run(creep: Creep, memory: CreepMemory) =
    let setLastAction action =
        setCreepMemory creep { memory with lastAction = action }
    let findEnergy() = findEnergy creep |> setLastAction
    let upgrade() = upgradeController creep |> setLastAction

    match ((creepEnergy creep), memory.lastAction) with
    | (Energy _, Upgrading)         -> upgrade()
    | (Energy _, Moving Upgrade)    -> upgrade()
    | (Energy _, Harvesting)        -> findEnergy()
    | (Energy _, Moving Harvest)    -> findEnergy()
    | (Empty, _)                    -> findEnergy()
    | (Full, _)                     -> upgrade()
    | (_, lastaction)    ->
        //printfn "%s ?? %A" creep.name lastaction 
        findEnergy()
