module Action.Build
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Model.Domain
open Manage.Memory.MemoryInCreep
open Action.Helpers

(*
    Todos:
    - find energy from alternate sources
    - immediately repair an item that's just been built - X
*)

let run(creep: Creep, memory: CreepMemory) =
    let harvest() =
        beginAction creep
        |> pickupDroppedResources
        |> dropNon Globals.RESOURCE_ENERGY
        |> harvestEnergySources
        |> endAction memory

    let build() = 
        beginAction creep
        |> build    
        |> repairStructures
        |> upgradeController
        |> endAction memory

    let repairNewStructure (existingAction) =
        (Success (creep, existingAction))
        |> quickRepair
        |> endAction memory

    match ((creepEnergy creep), memory.lastAction) with
    | (Empty, _)                    -> harvest()
    | (Full, _)                     -> build()
    | (Energy _, lastAction)        ->
        match lastAction with
        | Building pos -> 
            // if the thing we were just building is no longer a construction site, attempt a quick repair on any structures that were built
            let thing = creep.room.lookForAt<ConstructionSite>(Globals.LOOK_CONSTRUCTION_SITES, pos.x, pos.y)
            if thing.Count > 0
            then build() 
            else repairNewStructure(lastAction)
        | Harvesting -> harvest()
        | Moving action ->
            match action with
            | Building _ -> build()
            | _ -> harvest()
        | _ -> build()
