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
    let findEnergy() =
        beginAction creep
        |> pickupDroppedResources
        |> harvestEnergySources
        |> endAction memory

    let transfer() = 
        beginAction creep
        |> transferEnergy
        |> upgradeController
        |> endAction memory

    match ((creepEnergy creep), memory.lastAction) with
    | (Energy _, lastaction) ->
        match lastaction with
        | Harvesting -> findEnergy()
        | Transferring -> transfer()
        | Upgrading -> transfer()
        | Moving action ->
            match action with
            | Harvesting -> findEnergy()
            | Transferring -> transfer()
            | Upgrading -> transfer()
            | _ -> findEnergy()
        | _ -> findEnergy()
    | (Empty, _)    -> findEnergy()
    | (Full, _)     -> transfer() // only takes a single tick to transfer

