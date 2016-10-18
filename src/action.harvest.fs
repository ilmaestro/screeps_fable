module Action.Harvest
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Model.Domain
open Manage.Memory
open Action.Helpers
(*
    TODOs:
    - WATCH OUT for hostile creeps, make sure to stay of ranged attacks
*)

let run(creep: Creep, memory: CreepMemory) =
    let ifEmergency condition action lastresult =
        if condition() then (action lastresult)
        else lastresult

    let goToFlagIfInstructed lastresult =
        match memory.actionFlag with
        | Some flagName ->
            let flag = unbox<Flag>(Globals.Game.flags?(flagName))
            let flagMem = MemoryInFlag.get flag
            locateFlag flag flagMem.actionRadius lastresult
        | None -> lastresult

    let homeSpawn = unbox<Spawn>(Globals.Game.getObjectById(memory.spawnId))

    let harvest() =
        beginAction creep
        |> goToFlagIfInstructed
        |> dropNon Globals.RESOURCE_ENERGY
        |> pickupDroppedResources
        // |> ifEmergency (fun _ -> true) withdrawEnergyFromContainer
        |> harvestEnergySources
        |> endAction memory

    let transfer() = 
        beginAction creep
        |> locateSpawnRoom homeSpawn
        |> transferEnergyToStructures
        |> transferEnergyToContainers 20000.
        |> upgradeController
        |> endAction memory

    match ((MemoryInCreep.creepEnergy creep), memory.lastAction) with
    | (Empty, _)    -> harvest()
    | (Full, _)     -> transfer() // only takes a single tick to transfer
    | (Energy _, lastaction) ->
        match lastaction with
        | Harvesting -> harvest()
        | Moving action ->
            match action with
            | Harvesting -> harvest()
            | _ -> transfer()
        | _ -> transfer()


