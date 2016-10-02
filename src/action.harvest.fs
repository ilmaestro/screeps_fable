module Harvest
open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

type HarvestState =
    | Empty
    | SomeEnergy of float
    | Full

type HarvestResult =
    | Moving
    | Harvesting
    | Transferring
    | Fail

let creepState (creep: Creep) =
    match creep.carry.energy with
    | Some energy when energy = creep.carryCapacity -> Full
    | Some energy when energy > 0. -> SomeEnergy(energy)
    | _ -> Empty

let findEnergy (creep: Creep) =
    match (creep.pos.findClosestByPath(Globals.FIND_SOURCES_ACTIVE)) with
    | Some target ->
        match (creep.harvest(target)) with
        | r when r = Globals.OK -> 
            Harvesting
        | r when r = Globals.ERR_NOT_IN_RANGE -> 
            creep.moveTo(U2.Case2 (box target)) |> ignore
            Moving
        | _ -> Fail
    | None -> Fail

let transferEnergy (creep: Creep) =
    match (creep.pos.findClosestByPath(Globals.FIND_MY_SPAWNS)) with
    | Some spawn ->
        match (creep.transfer(spawn, Globals.RESOURCE_ENERGY)) with
        | r when r = Globals.OK -> Transferring
        | r when r = Globals.ERR_NOT_IN_RANGE ->
            creep.moveTo(U2.Case2 (box spawn)) |> ignore
            Moving
        | _ -> Fail
    | None -> Fail

let run(name: string) =

    match unbox Globals.Game.creeps?(name) with
    | Some c -> 
        let creep = c :> Creep
        let state = creepState creep
        match state with
        | Empty -> (findEnergy creep) |> ignore
        | SomeEnergy _ -> (findEnergy creep) |> ignore
        | Full -> (transferEnergy creep) |> ignore
    | None -> ()
