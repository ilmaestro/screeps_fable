module Action.Harvest
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Helpers
open Action.Helpers
(*
    TODOs:
    - WATCH OUT for hostile creeps, make sure to stay of ranged attacks
*)

let run(creep: Creep, memory: CreepMemory) =
    let harvest() =
        beginAction creep
        |> pickupDroppedResources
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


