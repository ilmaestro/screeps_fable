module Action.Pioneer
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Model.Domain
open Manage.Memory.MemoryInCreep
open Action.Helpers

let primaryTargetRoomName = 
    { x = 40.; 
        y = 14.;
        roomName = "E69S52";}

/// Goal 1: head to the target room and start building
let run(creep: Creep, memory: CreepMemory) =
    let harvest() =
        beginAction creep
        |> locateRoom primaryTargetRoomName
        |> pickupDroppedResources
        |> harvestEnergySources
        |> endAction memory

    let build() = 
        beginAction creep
        |> locateRoom primaryTargetRoomName
        |> build
        |> repairStructures
        |> upgradeController
        |> endAction memory

    match ((creepEnergy creep), memory.lastAction) with
    | (Empty, _)                    -> harvest()
    | (Full, _)                     -> build()
    | (Energy _, lastAction)        ->
        match lastAction with
        | Building pos -> build()
        | Harvesting -> harvest()
        | Moving action ->
            match action with
            | Building _ -> build()
            | _ -> harvest()
        | _ -> build()
