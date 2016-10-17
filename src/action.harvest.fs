module Action.Harvest
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Model.Domain
open Manage.Memory.MemoryInCreep
open Action.Helpers
(*
    TODOs:
    - WATCH OUT for hostile creeps, make sure to stay of ranged attacks
*)

let run(creep: Creep, memory: CreepMemory) =
    let ifEmergency condition action lastresult =
        if condition() then (action lastresult)
        else lastresult

    let harvest() =
        beginAction creep
        |> pickupDroppedResources
        // |> ifEmergency (fun _ -> true) withdrawEnergyFromContainer
        |> harvestEnergySources
        |> endAction memory

    let transfer() = 
        beginAction creep
        |> transferEnergyToStructures
        |> transferEnergyToContainers
        |> upgradeController
        |> endAction memory

    match ((creepEnergy creep), memory.lastAction) with
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


